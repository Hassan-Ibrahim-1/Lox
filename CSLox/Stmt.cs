namespace Lox;

public interface IStmtVisitor<R> {
    R VisitExpressionStmt(Expression stmt);
    R VisitBlockStmt(Block stmt);
    R VisitPrintStmt(Print stmt);
    R VisitVarStmt(Var stmt);
    R VisitIfStmt(If stmt);
    R VisitWhileStmt(While stmt);
    R VisitBreakStmt(Break stmt);
    R VisitFunctionStmt(Function stmt);
    R VisitGetterStmt(Getter stmt);
    R VisitReturnStmt(Return stmt);
    R VisitClassStmt(Class stmt);
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
public class Block : Stmt {
    public readonly List<Stmt> statements;

    public Block(List<Stmt> statements) {
        this.statements = statements;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitBlockStmt(this);
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
public class Var : Stmt {
    public readonly Token name;
    public readonly Expr initializer;

    public Var(Token name, Expr initializer) {
        this.name = name;
        this.initializer = initializer;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitVarStmt(this);
    }
}
public class If : Stmt {
    public readonly Expr condition;
    public readonly Stmt thenBranch;
    public readonly Stmt elseBranch;

    public If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitIfStmt(this);
    }
}
public class While : Stmt {
    public readonly Expr condition;
    public readonly Stmt body;

    public While(Expr condition, Stmt body) {
        this.condition = condition;
        this.body = body;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitWhileStmt(this);
    }
}
public class Break : Stmt {
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitBreakStmt(this);
    }
}
public class Function : Stmt {
    public readonly Token name;
    public readonly FunctionExpr functionExpr;
    public readonly bool isStatic;

    public Function(Token name, FunctionExpr functionExpr, bool isStatic = false) {
        this.name = name;
        this.functionExpr = functionExpr;
        this.isStatic = isStatic;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitFunctionStmt(this);
    }
}
public class Getter : Stmt {
    public readonly Token name;
    public readonly List<Stmt> statements;

    public Getter(Token name, List<Stmt> statements) {
        this.name = name;
        this.statements = statements;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitGetterStmt(this);
    }
}
public class Return : Stmt {
    public readonly Token keyword;
    public readonly Expr value;

    public Return(Token keyword, Expr value) {
        this.keyword = keyword;
        this.value = value;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitReturnStmt(this);
    }
}
public class Class : Stmt {
    public readonly Token name;
    public readonly Variable superclass;
    public readonly List<Function> methods;
    public readonly List<Getter> getters;

    public Class(Token name, Variable superclass, List<Function> methods, List<Getter> getters) {
        this.name = name;
        this.superclass = superclass;
        this.methods = methods;
        this.getters = getters;
    }
    public override R Accept<R>(IStmtVisitor<R> visitor) {
        return visitor.VisitClassStmt(this);
    }
}
