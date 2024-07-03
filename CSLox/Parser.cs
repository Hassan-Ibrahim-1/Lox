namespace Lox;

public class Parser {
    private class ParseError : Exception {}

    private List<Token> _tokens;
    private int _current = 0;
    

    public Parser(List<Token> tokens) {
        this._tokens = tokens;
    }

    public Expr Parse() {
        try {
            // Starts parsing the epxression
            return Expression();
        }
        catch(ParseError) {
            return null!;
        }
    }

    private Expr Expression() {
        return Comma();
    }

    private Expr Comma() {
        Expr expr = Equality();

        while(Match(TokenType.Comma)) {
            Token oper = Previous();
            Expr right = Equality();
            expr = new Binary(expr, oper, right);
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
        Expr expr = Unary();
        
        while (Match(TokenType.Slash, TokenType.Star)) {
            Token oper = Previous();
            Expr right = Unary();
            expr = new Binary(expr, oper, right);
        }
        return expr;
    }

    private Expr Unary() {
        if (Match(TokenType.Bang, TokenType.Minus)) {
            Token oper = Previous();
            Expr right = Unary();
            return new Unary(oper, right);
        }
        return Primary();
    }

    private Expr Primary() {
        if (Match(TokenType.False)) return new Literal(false);
        if (Match(TokenType.True)) return new Literal(true);
        if (Match(TokenType.Nil)) return new Literal(null!);
        if (Match(TokenType.String, TokenType.Number)) {
            return new Literal(Previous().literal);
        }
        if (Match(TokenType.Left_Paren)) {
            Expr expr = Expression();
            Consume(TokenType.Right_Paren, "Expect ')' after expression.");
            return new Grouping(expr);
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
