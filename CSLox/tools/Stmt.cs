namespace Lox;

public interface IStmtVisitor<R> {
    R VisitExpressionStmt(Expression stmt);
    R VisitPrintStmt(Print stmt);
}

public abstract class Stmt {
    public abstract R Accept<R>(IStmtVisitor<R> visitor);
}

public class Expression : Stmt {
    public readonly Expr expression;

    public Expression(Expr expression) {
        this.expression = expression;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitExpressionStmt(this);
    }
}
public class Print : Stmt {
    public readonly Expr expression;

    public Print(Expr expression) {
        this.expression = expression;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitPrintStmt(this);
    }
}
