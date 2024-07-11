namespace Lox;

public class Resolver : IVisitor<object>, IStmtVisitor<object> {
    private enum FunctionType {
        None,
        Function,
    }

    private readonly Interpreter _interpreter;
    // A stack of scopes
    private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
    private FunctionType _currentFunction = FunctionType.None;

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
    private void ResolveLocal(Expr expr, Token name) {
        for (int i = scopes.Count - 1; i >= 0; i--) {
            if (scopes.ElementAt(i).ContainsKey(name.lexeme)) {
                CPrint.Print($"Resolving {name.lexeme} when there are {scopes.Count} scopes. depth: {scopes.Count - 1 - i} i: {i}", ConsoleColor.Cyan);
                _interpreter.Resolve(expr, i);
            }
        } 
    }

    private void BeginScope() {
        scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope() {
        scopes.Pop();
    }

    private void Declare(Token name) {
        if (scopes.Count == 0) return;
        scopes.Peek()[name.lexeme] = false;
    }

    private void Define(Token name) {
        if (scopes.Count == 0) return;
        scopes.Peek()[name.lexeme] = true;
    }
    
    public object VisitBlockStmt(Block stmt) {
        BeginScope();
        Resolve(stmt.statements);
        EndScope();
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
        if (stmt.value != null) Resolve(stmt.value);
        return null!;
    }

    public object VisitFunctionExpr(FunctionExpr expr) {
        ResolveFunction(expr, FunctionType.Function);
        return null!;
    }

    public object VisitAssignmentExpr(Assignment expr) {
        Resolve(expr.value);
        ResolveLocal(expr, expr.name);
        Console.WriteLine($"Resolving {expr.name} from Assignment expr");
        return null!;
    }

    public object VisitVariableExpr(Variable expr) {
        if (scopes.Count != 0 && scopes.Peek().ContainsKey(expr.name.lexeme)) {
            if (!scopes.Peek()[expr.name.lexeme]) {
                Lox.Error(expr.name, "Can't read local variable in it's own initializer.");
            }
        }
        Console.WriteLine($"Resolving {expr.name} from Variable expr");
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
}
