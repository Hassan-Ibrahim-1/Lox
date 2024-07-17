namespace Lox;

public class Parser {
    private class ParseError : Exception {}

    private readonly List<Token> _tokens;
    private int _current = 0;
    private readonly bool _repl;

    private int _loopNestLevel = 0;

    public Parser(List<Token> tokens, bool _repl) {
        this._tokens = tokens;
        this._repl = _repl;
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
            if (Match(TokenType.Class)) return ClassDeclaration();
            if (Match(TokenType.Var)) return VarDeclaration();
            if (Check(TokenType.Fun) && CheckNext(TokenType.Identifier)) {
                Consume(TokenType.Fun, null!);
                return Function("function");
            }

            return Statement();
        }
        catch (ParseError) {
            Synchronize();
            return null!;
        }
    }

    private Stmt ClassDeclaration() {
        Token name = Consume(TokenType.Identifier, "Expect class name.");

        Variable superclass = null!;

        if (Match(TokenType.Less)) {
            Consume(TokenType.Identifier, "Expect superclass name.");
            superclass = new Variable(Previous());
        }

        Consume(TokenType.Left_Brace, "Expect '{' after class name.");
        
        List<Function> methods = new List<Function>();
        List<Getter> getters = new List<Getter>();
        
        while (!Check(TokenType.Right_Brace) && !IsAtEnd()) {
            Stmt stmt = Method("method");
            if (stmt is Function e) {
                methods.Add(e);
            }
            else {
                getters.Add((Getter)stmt);
            }
        }

        Consume(TokenType.Right_Brace, "Expect '}' after class body.");
        return new Class(name, superclass, methods, getters);
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

    private Stmt Function(string kind) {
        Token name = Consume(TokenType.Identifier, $"Expect {kind} name.");
        return new Function(name, FunctionBody(kind));
    }

    private Stmt Method(string kind) {
        bool isStatic = Match(TokenType.Static);
        Token name = Consume(TokenType.Identifier, $"Expect {kind} name.");
        if (Peek().type == TokenType.Left_Paren) {
            return new Function(name, FunctionBody(kind), isStatic);
        }
        if (isStatic) {
            Error(name, $"A getter cannot be declared as static.");
        }
        return Getter();
    }

    private Stmt Getter() {
        Token name = Previous();
        Consume(TokenType.Left_Brace, "Expect '{' after getter name.");
        List<Stmt> statements = Block();
        return new Getter(name, statements);
    }

    private Stmt Statement() {
        if (Match(TokenType.If)) return IfStatement();
        if (Match(TokenType.Print)) return PrintStatement();
        if (Match(TokenType.Left_Brace)) return new Block(Block());
        if (Match(TokenType.While)) return WhileStatement();
        if (Match(TokenType.For)) return ForStatement();
        if (Match(TokenType.Break)) return BreakStatement();
        if (Match(TokenType.Return)) return ReturnStatement();
        return ExpressionStatement();
    }

    private Stmt BreakStatement() {
        if (_loopNestLevel == 0) {
            Error(Previous(), "'break' cannot be outside a loop.");
        }
        Consume(TokenType.SemiColon, "Expect ';' after break");
        return new Break();
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

    private Stmt WhileStatement() {
        Consume(TokenType.Left_Paren, "Expect '(' after 'while'.");
        Expr condition = Expression();
        Consume(TokenType.Right_Paren, "Expect ')' after while condition");
        try {
            _loopNestLevel++;
            Stmt body = Statement();
            return new While(condition, body);
        }
        finally {
            _loopNestLevel--;
        }
    }

    private Stmt ForStatement() {
        // Constructed by desugaring
        // Doesn't have its own syntax node
        Consume(TokenType.Left_Paren, "Expect '(' after for statement.");

        // Not an expression because it can be a variable declaration
        Stmt initializer;

        if (Match(TokenType.SemiColon)) {
            initializer = null!;
        }
        else if (Match(TokenType.Var)) {
            initializer = VarDeclaration();
        }
        else {
            initializer = ExpressionStatement();
        }

        Expr condition = null!;

        if (!Check(TokenType.SemiColon)) {
            condition = Expression();
        }
        Consume(TokenType.SemiColon, "Expect ';' after loop condition.");

        Expr increment = null!;

        if (!Check(TokenType.Right_Paren)) {
            increment = Expression();
        }
        Consume(TokenType.Right_Paren, "Expect ')' after for clauses.");
        try {
            _loopNestLevel++;
            Stmt body = Statement();

            // Converting the for loop to a while loop
            if (increment != null) {
                var stmts = new List<Stmt>() {
                    body,
                        new Expression(increment)
                };
                body = new Block(stmts);
            }

            if (condition == null) {
                condition = new Literal(true);
            }
            body = new While(condition, body);

            if (initializer != null) {
                var stmts = new List<Stmt>() {
                    initializer,
                        body
                };
                body = new Block(stmts);
            }
            return body;
        }
        finally {
            _loopNestLevel--;
        }
    }

    private Stmt PrintStatement() {
        Expr value = Expression();
        Consume(TokenType.SemiColon, "Expect ';' after expression.");
        return new Print(value);
    }

    private Stmt ReturnStatement() {
        Token keyword = Previous();
        Expr value = null!;

        if (!Check(TokenType.SemiColon)) {
            value = Expression();
        }
        Consume(TokenType.SemiColon, "Expect ';' after return statement");
        return new Return(keyword, value);
    }

    private Stmt ExpressionStatement() {
         Expr expr = Expression();
         // Allow expression only statements in the repl
         if (!_repl) {
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
    
    private FunctionExpr FunctionBody(string kind) {
        Consume(TokenType.Left_Paren, $"Expect '(' after {kind} name");

        var parameters = new List<Token>();
        if (!Check(TokenType.Right_Paren)) {
            do {
                if (parameters.Count >= 255) {
                    Error(Peek(), $"A {kind} cannot have more than 255 parameters");
                }
                parameters.Add(Consume(TokenType.Identifier, $"Expect parameter name"));
            } while (Match(TokenType.Comma));
        }

        Consume(TokenType.Right_Paren, "Expect '(' after parameters");
        Consume(TokenType.Left_Brace, $"Expect '{{' before {kind} body.");
        List<Stmt> body = Block();
        
        return new FunctionExpr(parameters, body);
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
            else if (expr is Get get) {
                return new Set(get.obj, get.name, value);
            }
            Error(equals, "Invalid assignment target.");
        }
        return expr;
    }

    private Expr Ternary() {
        if (_current > 0 && (Previous().type == TokenType.Bang ||
            Previous().type == TokenType.Minus)) {
            return Or();
        }
        Expr expr = Or();

        if (Match(TokenType.Question)) {
            Expr thenBranch = Ternary();
            Consume(TokenType.Colon, "Expect ':' after then branch.");
            Expr elseBranch = Ternary();
            return new Ternary(expr, thenBranch, elseBranch);
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
        return Call();
    }

    private Expr Call() {
        Expr expr = Primary();

        // Deals with the this keyword as well
        while (true) {
            if (Match(TokenType.Left_Paren)) {
                // Also handles methods
                expr = FinishCall(expr);
            }
            else if (Match(TokenType.Dot)) {
                Token name = Consume(TokenType.Identifier, "Expect identifier after '.'.");
                
                // expr is the object that we're getting the property of
                // name is the name of the property that we're accessing
                // Don't return to allow chaining of properties
                expr = new Get(expr, name); 
            }
            else {
                break;
            }
        }

        return expr; 
    }

    private Expr FinishCall(Expr expr) {
        var arguments = new List<Expr>();
        if (!Check(TokenType.Right_Paren)) {
            do {
                if (arguments.Count > 255) {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }
                arguments.Add(Expression());
            } while (Match(TokenType.Comma));
        }

        Token paren = Consume(TokenType.Right_Paren, "Expect ')' after arguments");
        return new Call(expr, paren, arguments);
    }

    private Expr Primary() {
        if (Match(TokenType.False)) return new Literal(false);
        if (Match(TokenType.True)) return new Literal(true);
        if (Match(TokenType.Nil)) return new Literal(new Nil());
        if (Match(TokenType.This)) return new This(Previous());
        if (Match(TokenType.String, TokenType.Number)) {
            return new Literal(Previous().literal);
        }
        if (Match(TokenType.Identifier)) {
            // The identifier that just got matched
            Token identifier = Previous();
            Variable variable = new Variable(identifier);

            // i++
            if (Match(TokenType.Plus_Plus)) {
                Token op = new Token(TokenType.Plus, "+", null!, Previous().line);
                // i + 1
                Binary expr = new Binary((Expr)variable, op, new Literal(1d));
                return new Assignment(variable.name, expr);
            }
            // i--
            if (Match(TokenType.Minus_Minus)) {
                Token op = new Token(TokenType.Minus, "-", null!, Previous().line);
                Binary expr = new Binary(variable, op, new Literal(1d));
                return new Assignment(variable.name, expr);
            }

            return variable;
        }
        // Anonymous function
        if (Match(TokenType.Fun)) {
            // TODO: maybe call this lambda?
            return FunctionBody("function");
        }
        if (Match(TokenType.Super)) {
            Token keyword = Previous();
            Consume(TokenType.Dot, "Expect '.' after 'super'.");
            Token method = Consume(TokenType.Identifier, "Expect superclass method name.");
            return new Super(keyword, method);
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

        if (Match(TokenType.Static)) {
            throw Error(Previous(), "The static keyword can only be used for class methods.");
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

    private bool CheckNext(TokenType type) {
        if (_current + 1 > _tokens.Count) return false;
        return _tokens[_current + 1].type == type;
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
                case TokenType.Return: case TokenType.While: case TokenType.Break:
                case TokenType.Static:
                    return;
            }
            Next();
        }
    }
}
