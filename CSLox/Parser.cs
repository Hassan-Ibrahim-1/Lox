namespace Lox;

public class Parser {
    private class ParseError : Exception {}

    private readonly List<Token> _tokens;
    private int _current = 0;
    private bool repl;
    

    public Parser(List<Token> tokens, bool repl) {
        this._tokens = tokens;
        this.repl = repl;
    }

    public List<Stmt> Parse() {
        List<Stmt> statements = new List<Stmt>();

        while (!IsAtEnd()) {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt Declaration() {
        try {
            if (Match(TokenType.Var)) return VarDeclaration();
            return Statement();
        }
        catch (ParseError) {
            Synchronize();
            return null!;
        }
    }

    private Stmt VarDeclaration() {
        Token name = Consume(TokenType.Identifier, "Expect variable name.");

        Expr initializer = null!;
        if (Match(TokenType.Equal)) {
            initializer = Expression();
        }
        Consume(TokenType.SemiColon, "Expect ';' after variable declaration");
        return new Var(name, initializer);
    }

    private Stmt Statement() {
        if (Match(TokenType.If)) return IfStatement();
        if (Match(TokenType.Print)) return PrintStatement();
        if (Match(TokenType.Left_Brace)) return new Block(Block());
        return ExpressionStatement();
    }

    private Stmt IfStatement() {
        Consume(TokenType.Left_Paren, "Expect '(' after if");
        Expr condition = Expression();
        Consume(TokenType.Right_Paren, "Expect ')' after if condition");
        // Not Declaration because you can't declare in an if statement
        // Can declare in the block though
        Stmt thenBranch = Statement();
        Stmt elseBranch = null!;

        if (Match(TokenType.Else)) {
            elseBranch = Statement();
        }

        return new If(condition, thenBranch, elseBranch);
    }

    private List<Stmt> Block() {
        List<Stmt> statements = new List<Stmt>();

        while (!Check(TokenType.Right_Brace) && !IsAtEnd()) {
            statements.Add(Declaration());
        }
        Consume(TokenType.Right_Brace, "Expect '}' after block.");
        return statements;
    }

    private Stmt PrintStatement() {
        Expr value = Expression();
        Consume(TokenType.SemiColon, "Expect ';' after expression.");
        return new Print(value);
    }

    private Stmt ExpressionStatement() {
         Expr expr = Expression();
         // Allow expression only statements in the repl
         if (!repl) {
             Consume(TokenType.SemiColon, "Expect ';' after expression");
         }
         else {
             if (!Match(TokenType.SemiColon)) {
                 return new Print(expr);
             }
         }
         return new Expression(expr);
    }

    private Expr Expression() {
        return Assignment();
    }

    private Expr Assignment() {
        Expr expr = Ternary(); // variable name if an assignment expression

        // Not using a while loop becaues assignment is right associative
        if (Match(TokenType.Equal)) {
            Token equals = Previous(); // For error handling

            // The right side could be a non assignment expression or an assignment expression
            // Also why an if statement is used and not a while loop
            Expr value = Assignment();
            if (expr is Variable variable) {
                return new Assignment(variable.name, value);
            }
            Error(equals, "Invalid assignment target.");
        }
        return expr;
    }

    private Expr Ternary() {
        Expr expr = Or();
        while(Match(TokenType.Ternary_Question)) {
            Token ternaryOper = Previous();
            // Maybe call ternary instead
            Expr trueChoice = Equality();
            Consume(TokenType.Ternary_Colon, "Expect ':' after ternary conditional.");
            Token ternaryColon = Previous();
            Expr falseChoice = Equality();

            Expr right = new Binary(trueChoice, ternaryColon, falseChoice);
            expr = new Binary(new Grouping(expr), ternaryOper, right);
        }

        return expr;
    }

    private Expr Or() {
        Expr expr = And();
        while (Match(TokenType.Or)) {
            Token op = Previous();
            Expr right = And();
            return new Logic(expr, op, right);
        }
        return expr;
    }

    private Expr And() {
        Expr expr = Equality();
        while (Match(TokenType.And)) {
            Token op = Previous();
            Expr right = Equality();
            return new Logic(expr, op, right);
        }
        return expr;
    }

    private Expr Equality() {
        Expr expr = Compare(); // Expr on the left side of the equality operator

        // Tokens related to equality - while checking for equality
        // Allows for any number of consequent equality expressions
        while (Match(TokenType.Bang_Equal, TokenType.Equal_Equal)) {
            Token oper = Previous(); // operator - previous token because it gets consumed in match
            Expr right = Compare();
            expr = new Binary(expr, oper, right);
        }
        return expr;
    }

    private Expr Compare() {
        Expr expr = Term();
        
        while (Match(TokenType.Greater, TokenType.Greater_Equal, TokenType.Less, TokenType.Less_Equal)) {
            Token oper = Previous();
            Expr right = Term();
            expr = new Binary(expr, oper, right);
        }
        return expr;
    }

    private Expr Term() {
        Expr expr = Factor();

        while(Match(TokenType.Minus, TokenType.Plus)) {
            Token oper = Previous();
            Expr right = Factor();
            expr = new Binary(expr, oper, right);
        }
        return expr;
    }

    private Expr Factor() {
        Expr expr = UnaryExpr();
        
        while (Match(TokenType.Slash, TokenType.Star)) {
            Token oper = Previous();
            Expr right = UnaryExpr();
            expr = new Binary(expr, oper, right);
        }
        return expr;
    }

    private Expr UnaryExpr() {
        // Right associative
        if (Match(TokenType.Bang, TokenType.Minus)) {
            Token oper = Previous();
            Expr right = Expression();
            return new Unary(oper, right);
        }
        return Primary();
    }

    private Expr Primary() {
        if (Match(TokenType.False)) return new Literal(false);
        if (Match(TokenType.True)) return new Literal(true);
        if (Match(TokenType.Nil)) return new Literal(new Nil());
        if (Match(TokenType.String, TokenType.Number)) {
            return new Literal(Previous().literal);
        }
        if (Match(TokenType.Identifier)) {
            // The identifier that just got matched
            return new Variable(Previous());
        }
        if (Match(TokenType.Left_Paren)) {
            Expr expr = Expression();
            Consume(TokenType.Right_Paren, "Expect ')' after expression.");
            return new Grouping(expr);
        }
        if (Match(TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash)) {
            Token oper = Previous();
            try {
                Expression();
            }
            catch (ParseError) {}
            
            throw Error(oper, "Expect left operand");

        }
        // No valid expression found
        throw Error(Peek(), "Expect expression.");
    }

    private bool Match(params TokenType[] types) {
        foreach (TokenType type in types) {
            if (Check(type)) {
                Next();
                return true;
            }
        }
        return false;
    }

    private Token Consume(TokenType type, string message) {
        if (Check(type)) return Next();
        throw Error(Peek(), message);
    }


    private bool Check(TokenType type) {
        if (IsAtEnd()) return false;
        return Peek().type == type;
    }

    private Token Peek() {
        return _tokens[_current];
    }

    private bool IsAtEnd() {
        return Peek().type == TokenType.EOF;
    }

    private Token Next() {
        if (!IsAtEnd()) _current++; // Only add if not at end - that's why you can't directly return _current++
        return Previous();
    }

    private Token Previous() {
        return _tokens[_current - 1];
    }

    private ParseError Error(Token token, string message) {
        Lox.Error(token, message);
        return new ParseError();
    }

    private void Synchronize() {
        // Move onto the next statement
        Next();

        while (!IsAtEnd()) {
            if (Previous().type == TokenType.SemiColon) return;

            switch(Peek().type) {
                case TokenType.Class: case TokenType.For: case TokenType.Fun:
                case TokenType.If: case TokenType.Print: case TokenType.Var:
                case TokenType.Return: case TokenType.While:
                    return;
            }
            Next();
        }
    }
}
