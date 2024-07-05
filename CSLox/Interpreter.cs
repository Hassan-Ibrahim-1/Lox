namespace Lox;

// IStmtVisitor here just returns null all the time
public class Interpreter : IVisitor<object>, IStmtVisitor<object> {
    // current environment, not necessarily global
    private Environment environment = new Environment();

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
        object value = null!;
        if (stmt.initializer != null) {
            value = Evaluate(stmt.initializer);
        }
        environment.Define(stmt.name, value);
        return null!;
    }

    public object VisitBlockStmt(Block stmt) {
        ExecuteBlock(stmt.statements, new Environment(environment));
        return null!;
    }

    public object VisitAssignmentExpr(Assignment stmt) {
        object value = Evaluate(stmt.value);
        environment.Assign(stmt.name, value);
        return value;
    }

    // Using the variable in an expression
    public object VisitVariableExpr(Variable expr) {
        // Replace the name with the actual value
        return environment.Get(expr.name);
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

    private static bool IsTrue(object obj) {
        if (obj == null) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }
    
    private static string Stringify(object obj) {
        if (obj == null) return "nil";

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
    
    private void ExecuteBlock(List<Stmt> statements, Environment environment) {
        Environment previous = this.environment;

        try {
            // Change environment for every method so that variable declarations only exist in that environment
            this.environment = environment;
            
            foreach(Stmt stmt in statements) {
                Execute(stmt);
            }
        }
        finally {
            this.environment = previous; // Eventually gets set back to the global environment
        }
    }
}
