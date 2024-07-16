using Lox.Collections;

namespace Lox;

public class Resolver : IVisitor<object>, IStmtVisitor<object> {
    private enum FunctionType {
        None,
        Function,
        Initializer,
        Method
    }

    private enum ClassType {
        None,
        Class
    }

    private enum VarState {
        Declared,
        Defined,
        Read
    }

    private FunctionType _currentFunction = FunctionType.None;
    private ClassType _currentClass = ClassType.None;

    private readonly Interpreter _interpreter;
    // A stack of scopes
    private readonly Stack<HashMap<string, VarState>> scopes = new Stack<HashMap<string, VarState>>();

    public Resolver(Interpreter interpreter) {
        this._interpreter = interpreter;
    }

    public void Resolve(List<Stmt> stmts) {
        foreach (Stmt stmt in stmts) {
            Resolve(stmt);
        }
    }

    private void Resolve(Stmt stmt) {
        stmt.Accept(this);
    }

    private void Resolve(Expr expr) {
        expr.Accept(this);
    }

    private void ResolveFunction(FunctionExpr expr, FunctionType functionType) {
        FunctionType enclosingFunction = _currentFunction;
        _currentFunction = functionType; 

        BeginScope();
        foreach(Token param in expr.parameters) {
            Declare(param);
            Define(param);
        }

        Resolve(expr.body);
        EndScope();

        _currentFunction = enclosingFunction;
    }

    // Used for binding variable names to their values when the variable is being used
    // Depth is how far back the scope in which the variable is declared in compared to the current scope
    private void ResolveLocal(Expr expr, Token name) {
        for (int depth = scopes.Count - 1; depth >= 0; depth--) {
            HashMap<string, VarState> scope = scopes.ElementAt(depth);
            if (scope.ContainsKey(name.lexeme)) {
                _interpreter.Resolve(expr, depth, scope.GetIndex(name.lexeme));
                scope[name.lexeme] = VarState.Read;
            }
        }
    }

    private void BeginScope() {
        scopes.Push(new HashMap<string, VarState>());
    }

    private void EndScope() {
        HashMap<string, VarState> scope = scopes.Peek();

        foreach (string key in scope.Keys) {
            if (scope[key] != VarState.Read) {
                CPrint.Print($"Warning: Local variable '{key}' never used.", ConsoleColor.Yellow);
            }
        }

        scopes.Pop();
    }

    private void Declare(Token name) {
        if (scopes.Count == 0) return; // global
        if (scopes.Peek().ContainsKey(name.lexeme)) {
           Lox.Error(name, $"'{name.lexeme}' has already been defined");
        }
        scopes.Peek()[name.lexeme] = VarState.Declared;
    }

    private void Define(Token name) {
        if (scopes.Count == 0) return; // global
        scopes.Peek()[name.lexeme] = VarState.Defined;
    }
    
    public object VisitBlockStmt(Block stmt) {
        BeginScope();
        Resolve(stmt.statements);
        EndScope();
        return null!;
    }

    public object VisitClassStmt(Class stmt) {
        Declare(stmt.name);
        Define(stmt.name);
        // The entire class has its own separate scope
        // this is declared as a class-wide variable
        BeginScope();
        ClassType enclosingClass = _currentClass;
        _currentClass = ClassType.Class;
        scopes.Peek().Put("this", VarState.Read);

        foreach (Function method in stmt.methods) {
            FunctionType declaration = method.name.lexeme == "init"
                ? FunctionType.Initializer
                : FunctionType.Method;

            ResolveFunction(method.functionExpr, declaration);
        }

        EndScope();
        _currentClass = enclosingClass;
        return null!;
    }

    public object VisitVarStmt(Var stmt) {
        Declare(stmt.name);
        if (stmt.initializer != null) {
            Resolve(stmt.initializer);
        }
        Define(stmt.name);
        return null!;
    }

    public object VisitFunctionStmt(Function stmt) {
        Declare(stmt.name);
        Define(stmt.name);
        Resolve(stmt.functionExpr);
        return null!;
    }

    public object VisitExpressionStmt(Expression stmt) {
        Resolve(stmt.expression);
        return null!;
    }

    public object VisitPrintStmt(Print stmt) {
        Resolve(stmt.expression);
        return null!;
    }

    public object VisitIfStmt(If stmt) {
        Resolve(stmt.condition);
        Resolve(stmt.thenBranch);
        if (stmt.elseBranch != null) {
            Resolve(stmt.elseBranch);
        }
        return null!;
    }
    
    public object VisitWhileStmt(While stmt) {
        Resolve(stmt.condition);
        Resolve(stmt.body);
        return null!;
    }

    public object VisitBreakStmt(Break stmt) {
        return null!;
    }

    public object VisitReturnStmt(Return stmt) {
        if (_currentFunction == FunctionType.None) {
            Lox.Error(stmt.keyword, "A return statement cannot be outside a function body.");
        }
        if (stmt.value != null) {
            if (_currentFunction == FunctionType.Initializer) {
                Lox.Error(stmt.keyword, "A return statement cannot be inside a class initalizer");
            }

            Resolve(stmt.value);
        }

        return null!;
    }

    public object VisitFunctionExpr(FunctionExpr expr) {
        ResolveFunction(expr, FunctionType.Function);
        return null!;
    }

    public object VisitAssignmentExpr(Assignment expr) {
        Resolve(expr.value);
        ResolveLocal(expr, expr.name);
        return null!;
    }

    public object VisitVariableExpr(Variable expr) {
        if (scopes.Count != 0 && scopes.Peek().ContainsKey(expr.name.lexeme)) {
            if (scopes.Peek()[expr.name.lexeme] == VarState.Declared) {
                Lox.Error(expr.name, "Can't read local variable in it's own initializer.");
            }
        }
        ResolveLocal(expr, expr.name);
        return null!;
    }

    public object VisitBinaryExpr(Binary expr) {
        Resolve(expr.left);
        Resolve(expr.right);
        return null!;
    }

    public object VisitGroupingExpr(Grouping expr) {
        Resolve(expr.expression);
        return null!;
    }

    public object VisitLiteralExpr(Literal expr) {
        return null!;
    }

    public object VisitUnaryExpr(Unary expr) {
        Resolve(expr.right);
        return null!;
    }

    public object VisitLogicExpr(Logic expr) {
        Resolve(expr.left);
        Resolve(expr.right);
        return null!;
    }

    public object VisitTernaryExpr(Ternary expr) {
        Resolve(expr.conditional);
        Resolve(expr.thenBranch);
        Resolve(expr.elseBranch);
        return null!;
    }
    
    public object VisitCallExpr(Call expr) {
        // Callee is usually a variable expr
        Resolve(expr.callee);
        foreach(Expr arg in expr.arguments) {
            Resolve(arg);
        }
        return null!;
    }

    public object VisitGetExpr(Get expr) {
        // properties don't get resolved because they're looked up dynamically
        Resolve(expr.obj);
        return null!;
    }

    public object VisitSetExpr(Set expr) {
        Resolve(expr.obj);
        Resolve(expr.value);
        return null!;
    }

    public object VisitThisExpr(This expr) {
        if (_currentClass == ClassType.None) {
            Lox.Error(expr.keyword, "'this' statement cannot be outside a class");
            return null!;
        }

        ResolveLocal(expr, expr.keyword);
        return null!;
    }
}
