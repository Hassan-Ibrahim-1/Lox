namespace Lox;

public interface IVisitor<R> {
    R VisitExpressionStmt(Expression stmt);
    R VisitPrintStmt(Print stmt);
    R VisitVarStmt(Var stmt);
}

public abstract class Stmt {
    public abstract R accept<R>(IVisitor<R> visitor);
}

public class Expression : Stmt {
    public readonly Expr expression;

    public Expression(Expr expression) {
        this.expression = expression;
    }
    public override R accept<R>(IVisitor<R> visitor) {
        return visitor.VisitExpressionStmt(this);
    }
}
public class Print : Stmt {
    public readonly Expr expression;

    public Print(Expr expression) {
        this.expression = expression;
    }
    public override R accept<R>(IVisitor<R> visitor) {
        return visitor.VisitPrintStmt(this);
    }
}
public class Var : Stmt {
    public readonly Token name;
    public readonly Expr initializer;

    public Var(Token name, Expr initializer) {
        this.name = name;
        this.initializer = initializer;
    }
    public override R accept<R>(IVisitor<R> visitor) {
        return visitor.VisitVarStmt(this);
    }
}
