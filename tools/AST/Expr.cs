using Lox;

public interface IVisitor<R> {
    R VisitBinaryExpr(Binary expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitUnaryExpr(Unary expr);
}

public abstract class Expr {
    public abstract R accept<R>(IVisitor<R> visitor);
}

public class Binary : Expr {
    public readonly Expr left;
    public readonly Token op;
    public readonly Expr right;

    public Binary(Expr left, Token op, Expr right) {
        this.left = left;
        this.op = op;
        this.right = right;
    }
    public override R accept<R>(IVisitor<R> visitor) {
        return visitor.VisitBinaryExpr(this);
    }
}
public class Grouping : Expr {
    public readonly Expr expression;

    public Grouping(Expr expression) {
        this.expression = expression;
    }
    public override R accept<R>(IVisitor<R> visitor) {
        return visitor.VisitGroupingExpr(this);
    }
}
public class Literal : Expr {
    public readonly object value;

    public Literal(object value) {
        this.value = value;
    }
    public override R accept<R>(IVisitor<R> visitor) {
        return visitor.VisitLiteralExpr(this);
    }
}
public class Unary : Expr {
    public readonly Token op;
    public readonly Expr right;

    public Unary(Token op, Expr right) {
        this.op = op;
        this.right = right;
    }
    public override R accept<R>(IVisitor<R> visitor) {
        return visitor.VisitUnaryExpr(this);
    }
}
