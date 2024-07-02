namespace Lox;

public class Scanner {
    private readonly string _source;
    private readonly List<Token> _tokens = new List<Token>();
    private int _start = 0; // first char of the lexeme being scanned
    private int _current = 0; // current char of the lexeme being scanned
    private int _line = 1; // Used to figure out the exact location of the token

    private static readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>() {
        { "and",    TokenType.And },
        { "or",     TokenType.Or },
        { "if",     TokenType.If },
        { "else",   TokenType.Else },
        { "true",   TokenType.True },
        { "false",  TokenType.False },
        { "class",  TokenType.Class },
        { "fun",    TokenType.Fun },
        { "for",    TokenType.For },
        { "nil",    TokenType.Nil },
        { "print",  TokenType.Print },
        { "return", TokenType.Return },
        { "super",  TokenType.Super },
        { "this",   TokenType.This },
        { "var",    TokenType.Var },
        { "while",  TokenType.While },
    };

    public Scanner (string source) {
        this._source = source;
    }

    public List<Token> ScanTokens() {
        while (!IsAtEnd()) {
            _start = _current;
            ScanToken();
        }
        _tokens.Add(new Token(TokenType.EOF, "", null!, _line));
        return _tokens;
    }

    private void ScanToken() {
        char c = Next();

        switch (c) {
            case '(': AddToken(TokenType.Left_Paren); break;
            case ')': AddToken(TokenType.Right_Paren); break;
            case '{': AddToken(TokenType.Left_Brace); break;
            case '}': AddToken(TokenType.Right_Brace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '+': AddToken(TokenType.Plus); break;
            case '-': AddToken(TokenType.Minus); break;
            case '*': AddToken(TokenType.Star); break;
            case ';': AddToken(TokenType.SemiColon); break;

            case '!': 
                AddToken(Match('=') ? TokenType.Bang_Equal : TokenType.Bang);
                break;
            case '=': 
                AddToken(Match('=') ? TokenType.Equal_Equal : TokenType.Equal);
                break;
            case '<': 
                AddToken(Match('=') ? TokenType.Less_Equal: TokenType.Less);
                break;
            case '>': 
                AddToken(Match('=') ? TokenType.Greater_Equal: TokenType.Greater);
                break;

            case '/':
                if (Match('/')) {
                    // comment is not a token
                    // Not using match because it increments when encountering a new line
                    while (Peek() != '\n' && !IsAtEnd()) Next();
                }
                else if (Match('*')) {
                    MultiLineComment();
                }
                else {
                    AddToken(TokenType.Slash);
                }
                break;

            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                _line++;
                break;

            // Literals
            case '"':
                AddString();
                break;

            default:
                if (IsDigit(c)) {
                    AddNumber();
                }
                else if (IsAlpha(c)) {
                    AddIdentifier();
                }
                else {
                    Lox.Error(_line, "Unexpected Character");
                }
                break;
        }
    }

    private void AddString() {
        while (Peek() != '"' && !IsAtEnd()) {
            if (Peek() == '\n') {
                _line++;
            }
            Next();
        }
        if (IsAtEnd()) {
            Lox.Error(_line, "String not terminated.");
            return;
        }

        Next(); // move on from the ending "
        
        // Trim quotes
        string literal = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.String, literal);
    }

    private void AddNumber() {
        while (IsDigit(Peek())) Next();

        if (Peek() == '.' && IsDigit(PeekNext())) {
            Next(); // Consume the .

            while (IsDigit(Peek())) Next();
        }
        AddToken(TokenType.Number, Double.Parse(_source.Substring(_start, _current - _start)));
    }

    private void AddIdentifier() {
        while (IsAlphaNumeric(Peek())) Next();

        string lexeme = _source.Substring(_start, _current - _start);
        TokenType type;

        if (_keywords.ContainsKey(lexeme)) {
            type = _keywords[lexeme];
        }
        else {
            // User defined identifer
            type = TokenType.Identifier;
        }

        AddToken(type);
    }

    private void MultiLineComment() {
        int nestLevel = 1;

        while (!IsAtEnd() && nestLevel > 0) {
            if (Peek() == '\n') {
                Console.Write("new line");
                _line++;
            }
            else if (Peek() == '/' && MatchNext('*')) {
                Console.Write(Next());
                nestLevel++;
            }
            else if (Peek() == '*' && MatchNext('/')) {
                Next();
                nestLevel--;
            }
            Next();
        }
        if (IsAtEnd()) {
            Lox.Error(_line, "Multiline comment not closed.");
        }
    }

    // Only consume a character if its the expected value
    private bool Match(char expected) {
        if (IsAtEnd()) return false;
        if (_source[_current] != expected) return false;

        _current++;
        return true;
    }

    private bool MatchNext(char expected) {
        if (_current + 1 >= _source.Length) return false;
        if (_source[_current+1] != expected) return false;

        _current++;
        return true;
    }
    
    private char Peek() {
        if (IsAtEnd()) return '\0';
        return _source[_current];
    }

    private char PeekNext() {
        if (_current + 1 >= _source.Length) return '\0';
        return _source[_current+1];
    }

    private bool IsAtEnd() {
        return _current >= _source.Length;
    }

    private bool IsDigit(char c) {
        return c >= '0' && c <= '9';
    }

    private bool IsAlpha(char c) {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               (c == '_');
    }

    private bool IsAlphaNumeric(char c) {
        return IsDigit(c) || IsAlpha(c);
    }

    private char Next() {
        return _source[_current++];
    }

    private void AddToken(TokenType type) {
        AddToken(type, null!);
    }

    private void AddToken(TokenType type, object literal) {
        String lexeme = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, lexeme, literal, _line));
    }
}
