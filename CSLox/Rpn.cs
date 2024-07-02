namespace Lox;

public class Rpn : IVisitor<string> {

    public string Print(Expr expr) {
        return expr.Accept(this);
    }

    public string VisitBinaryExpr(Binary expr) {
        return $"{expr.left.Accept(this)} {expr.right.Accept(this)} {expr.op.lexeme}";
    }
    
    public string VisitGroupingExpr(Grouping expr) {
        return $"( {expr.expression.Accept(this)} )";
    }
    
    public string VisitLiteralExpr(Literal expr) {
        if (expr.value == null) return "nil";
        return expr.value.ToString()!;
    }

    public string VisitUnaryExpr(Unary expr) {
        return $"{expr.right.Accept(this)} {expr.op.lexeme}";
    }
}
