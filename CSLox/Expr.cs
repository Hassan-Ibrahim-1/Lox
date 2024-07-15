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
    R VisitCallExpr(Call expr);
    R VisitFunctionExpr(FunctionExpr expr);
    R VisitGetExpr(Get expr);
    R VisitSetExpr(Set expr);
    R VisitThisExpr(This expr);
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
public class Call : Expr {
    public readonly Expr callee;
    public readonly Token paren;
    public readonly List<Expr> arguments;

    public Call(Expr callee, Token paren, List<Expr> arguments) {
        this.callee = callee;
        this.paren = paren;
        this.arguments = arguments;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitCallExpr(this);
    }
}
public class FunctionExpr : Expr {
    public readonly List<Token> parameters;
    public readonly List<Stmt> body;

    public FunctionExpr(List<Token> parameters, List<Stmt> body) {
        this.parameters = parameters;
        this.body = body;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitFunctionExpr(this);
    }
}
public class Get : Expr {
    public readonly Expr obj;
    public readonly Token name;

    public Get(Expr obj, Token name) {
        this.obj = obj;
        this.name = name;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitGetExpr(this);
    }
}
public class Set : Expr {
    public readonly Expr obj;
    public readonly Token name;
    public readonly Expr value;

    public Set(Expr obj, Token name, Expr value) {
        this.obj = obj;
        this.name = name;
        this.value = value;
    }
    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitSetExpr(this);
    }
}

public class This : Expr {
    public readonly Token keyword;

    public This(Token keyword) {
        this.keyword = keyword;
    }

    public override R Accept<R>(IVisitor<R> visitor) {
        return visitor.VisitThisExpr(this);
    }
}

