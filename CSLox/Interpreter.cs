using Lox.Collections;

namespace Lox;

// IStmtVisitor here just returns null all the time
public class Interpreter : IVisitor<object>, IStmtVisitor<object> {
    // current environment, not necessarily global
    private readonly HashMap<string, object> _globals = new HashMap<string, object>();
    // TODO: Does this have to be initialized?
    private Environment _environment = null!;

    // Contains distance to local variable
    private readonly HashMap<Expr, int?> _locals = new HashMap<Expr, int?>();
    // Contains the indices for where local variables are stored in an environment
    private readonly HashMap<Expr, int> _localIndices = new HashMap<Expr, int>();

    public Interpreter() {
        // TODO: Figure this out
    
        // _environment = _globals;
        
        _globals.Add("clock", new Clock());
        _globals.Add("exit", new Exit());
    }

   public void Interpret(List<Stmt> stmts) {
        try {
            foreach (Stmt stmt in stmts) {
                Execute(stmt);
            }
        }
        catch (RuntimeError e) {
            Lox.RuntimeError(e);
        }
    }
   
    public object VisitExpressionStmt(Expression stmt) {
        Evaluate(stmt.expression);
        return null!;
    }

    public object VisitPrintStmt(Print stmt) {
        object value = Evaluate(stmt.expression);
        Console.WriteLine(Stringify(value));
        return null!;
    }

    // Definition
    public object VisitVarStmt(Var stmt) {
        // TODO: Figure this out.
        // If current env == null?
        object value = null!;
        if (stmt.initializer != null) {
            value = Evaluate(stmt.initializer);
        }
        if (_environment == null) {
            _globals.Add(stmt.name.lexeme, value);
        }

        else {
            _environment.Define(value);
        }
        return null!;
    }

    public object VisitIfStmt(If stmt) {
        if (IsTrue(Evaluate(stmt.condition))) {
            Execute(stmt.thenBranch);
        }
        else if (stmt.elseBranch != null) {
            Execute(stmt.elseBranch);
        }

        return null!;
    }

    public object VisitWhileStmt(While stmt) {
        while (IsTrue(Evaluate(stmt.condition))) {
            try {
                Execute(stmt.body);
            }
            catch (BreakStmt) {
                break;
            }
        }
        return null!;
    }

    public object VisitBreakStmt(Break stmt) {
        throw new BreakStmt();
    }

    public object VisitBlockStmt(Block stmt) {
        ExecuteBlock(stmt.statements, new Environment(_environment));
        return null!;
    }

    public object VisitFunctionStmt(Function stmt) {
        var function = new LoxFunction(stmt, _environment);
        if (_environment == null) {
            _globals.Add(stmt.name.lexeme, function);
        }
        else {
            _environment.Define(function);
        }
        return null!;
    }

    public object VisitReturnStmt(Return stmt) {
        object value = new Nil();

        if (stmt.value != null) {
            value = Evaluate(stmt.value);
        }

        throw new ReturnException(value);
    }

    public object VisitAssignmentExpr(Assignment expr) {
        object value = Evaluate(expr.value);
        if (_locals.ContainsKey(expr)) {
            _environment.AssignAt(_localIndices[expr], value, _locals[expr]);
        }
        // globals
        else {
            _globals[expr.name.lexeme] = value;
        }

        return value;
    }

    public object VisitTernaryExpr(Ternary expr) {
        if (IsTrue(Evaluate(expr.conditional))) {
            return Evaluate(expr.thenBranch);
        }
        return Evaluate(expr.elseBranch);
    }

    // Doesn't return true or false
    public object VisitLogicExpr(Logic expr) {
        object left = Evaluate(expr.left);

        if (expr.op.type == TokenType.Or) {
            if (IsTrue(left)) return left;
        }
        else if (!IsTrue(left)) return left;
        
        return Evaluate(expr.right);
    }

    // Using the variable in an expression
    public object VisitVariableExpr(Variable expr) {
        return LookUpVariable(expr.name, expr);
    }

    private object LookUpVariable(Token name, Variable expr) {
        int? distance = _locals.Get(expr);

        if (distance != null) {
            return _environment.GetAt(_localIndices[expr], distance);
        }
        // global
        else {
            return _globals[name.lexeme];
        }
    }

    public object VisitLiteralExpr(Literal expr) {
        return expr.value;
    }

    public object VisitGroupingExpr(Grouping expr) {
        return Evaluate(expr.expression);
    }

    public object VisitUnaryExpr(Unary expr) {
        object right = Evaluate(expr.right);

        switch (expr.op.type) {
            case TokenType.Bang:
                return !IsTrue(right);
            case TokenType.Minus:
                CheckNumberOperand(expr.op, right); 
                return -(double)right;
        }

        return null!; // Never reached
    }

    public object VisitBinaryExpr(Binary expr) {
        object left = Evaluate(expr.left);
        object right = Evaluate(expr.right);

        switch(expr.op.type) {
            case TokenType.Minus:
                CheckNumberOperands(expr.op, left, right);
                return (double)left - (double)right;
            case TokenType.Plus:
                if (left is double && right is double) {
                    return (double)left + (double)right;
                }
                if (left is string || right is string) {
                    return left.ToString() + right.ToString();
                }
                // TODO: Change this error message
                throw new RuntimeError(expr.op, "Operands must be two numbers or two strings");
            case TokenType.Slash:
                CheckNumberOperands(expr.op, left, right);
                CheckDivisionByZero(expr.op, right);
                return (double)left / (double)right;
            case TokenType.Star:
                CheckNumberOperands(expr.op, left, right);
                return (double)left * (double)right;

            case TokenType.Greater:
                CheckNumberOperands(expr.op, left, right);
                return (double)left > (double)right;
            case TokenType.Greater_Equal:
                CheckNumberOperands(expr.op, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                CheckNumberOperands(expr.op, left, right);
                return (double)left < (double)right;
            case TokenType.Less_Equal:
                CheckNumberOperands(expr.op, left, right);
               return (double)left <= (double)right;
            case TokenType.Equal_Equal:
               return left.Equals(right);
            case TokenType.Bang_Equal:
               return !left.Equals(right);
        }

        return null!; // Unreachable
    }

    public object VisitCallExpr(Call expr) {
        object callee = Evaluate(expr.callee);
        var arguments = new List<object>();
        
        foreach (Expr argument in expr.arguments) {
            object value = Evaluate(argument);
            if (value == null) {
                throw new RuntimeError(expr.paren, "An argument provided is not set to an instance of an object");
            }
            arguments.Add(value!);
        }

        if (!(callee is LoxCallable)) {
            throw new RuntimeError(expr.paren, "Can only call functions and classes.");
        }

        LoxCallable function = (LoxCallable)callee;

        if (arguments.Count != function.Arity()) {
            throw new RuntimeError(expr.paren, $"Expected {function.Arity()} argument(s) but got {arguments.Count}.");
        }

        return function.Call(this, arguments);
    }

    public object VisitFunctionExpr(FunctionExpr expr) {
        Token name = new Token(TokenType.Nil, null!, null!, 0);
        Function function = new Function(name, expr);
        return new LoxFunction(function, _environment);
    }

    private static bool IsTrue(object obj) {
        if (obj is Nil) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }
    
    private static string Stringify(object obj) {
        if (obj is Nil) return "nil";

        if (obj is double) {
            string text = obj.ToString()!;
            if (text.EndsWith(".0")) {
                text = text.Substring(0, text.Length-2);
            }
            return text;
        }
        return obj.ToString()!.ToLower();
    }

    private static void CheckNumberOperand(Token op, object operand) {
        if (operand is double) return;
        throw new RuntimeError(op, "Operand must be a number.");
    }

    private static void CheckDivisionByZero(Token op, object right) {
        if (right is double n) {
            if (n == 0) {
                throw new RuntimeError(op, "Division by zero error.");
            }
        }
    }

    private static void CheckNumberOperands(Token op, object left, object right) {
        if (left is double && right is double) return;
        throw new RuntimeError(op, "Operands must be numbers");
    }

    private object Evaluate(Expr expr) {
        return expr.Accept(this);
    }

    private void Execute(Stmt stmt) {
        stmt.Accept(this);
    }

    // Environment represents the environment to switch to
    public void ExecuteBlock(List<Stmt> statements, Environment environment) {
        Environment previous = this._environment;

        try {
            // Change environment for every method so that variable declarations only exist in that environment
            this._environment = environment;
            
            foreach(Stmt stmt in statements) {
                Execute(stmt);
            }
        }
        finally {
            this._environment = previous; // Eventually gets set back to the global environment
        }
    }

    public void Resolve(Expr expr, int depth, int localIndex) {
        _locals.Put(expr, depth);
        _localIndices.Put(expr, localIndex);
    }
}
