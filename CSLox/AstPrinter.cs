using System.Text;

namespace Lox;

public class AstPrinter : IVisitor<string> {

    public string Print(Expr expr) {
        // expr can be any child
        return expr.Accept(this);
    }

    public string VisitVariableExpr(Variable expr) {
        return Parenthesize("Var", expr);
    }

    public string VisitBinaryExpr(Binary expr) {
        return Parenthesize(expr.op.lexeme, expr.left, expr.right);
    }

    public string VisitGroupingExpr(Grouping expr) {
        return Parenthesize("group", expr.expression);
    }

    public string VisitLiteralExpr(Literal expr) {
        if (expr.value == null) return "nil";
        return expr.value.ToString()!;
    }

    public string VisitUnaryExpr(Unary expr) {
        return Parenthesize(expr.op.lexeme, expr.right);
    } 

    private string Parenthesize(string name, params Expr[] exprs) {
        StringBuilder builder = new StringBuilder();

        builder.Append("(").Append(name);
        
        foreach (Expr expr in exprs) {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }
}
