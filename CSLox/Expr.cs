namespace Lox;

public interface IVisitor<R> {
    R VisitBinaryExpr(Binary expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitUnaryExpr(Unary expr);
    R VisitVariableExpr(Variable expr);
    R VisitAssignmentExpr(Assignment expr);
    R VisitLogicExpr(Logic expr);
    R VisitTernaryExpr(Ternary expr);
}

public abstract class Expr {
    public abstract R Accept<R>(IVisitor<R> visitor);
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
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitBinaryExpr(this);
    }
}
public class Grouping : Expr {
    public readonly Expr expression;

    public Grouping(Expr expression) {
        this.expression = expression;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitGroupingExpr(this);
    }
}
public class Literal : Expr {
    public readonly object value;

    public Literal(object value) {
        this.value = value;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
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
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitUnaryExpr(this);
    }
}
public class Variable : Expr {
    public readonly Token name;

    public Variable(Token name) {
        this.name = name;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitVariableExpr(this);
    }
}
public class Assignment : Expr {
    public readonly Token name;
    public readonly Expr value;

    public Assignment(Token name, Expr value) {
        this.name = name;
        this.value = value;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitAssignmentExpr(this);
    }
}
public class Logic : Expr {
    public readonly Expr left;
    public readonly Token op;
    public readonly Expr right;

    public Logic(Expr left, Token op, Expr right) {
        this.left = left;
        this.op = op;
        this.right = right;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitLogicExpr(this);
    }
}
public class Ternary : Expr {
    public readonly Expr conditional;
    public readonly Expr thenBranch;
    public readonly Expr elseBranch;

    public Ternary(Expr conditional, Expr thenBranch, Expr elseBranch) {
        this.conditional = conditional;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitTernaryExpr(this);
    }
}
