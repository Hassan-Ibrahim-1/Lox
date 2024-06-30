namespace Lox;

public class Scanner {
    private readonly string _source;
    private readonly List<Token> _tokens = new List<Token>();
    private int _start = 0; // first char of the lexeme being scanned
    private int _current = 0; // current char of the lexeme being scanned
    private int _line = 1; // Used to figure out the exact location of the token

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
            case '(': AddToken(TokenType.Right_Paren); break;
            case ')': AddToken(TokenType.Left_Paren); break;
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
                    while (peek() != '\n' && !IsAtEnd()) Next();
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

            default:
                Lox.Error(_line, "Unexpected Character");
                break;
        }
    }

    // Only consume a character if its the expected value
    private bool Match(char expected) {
        if (IsAtEnd()) return false;
        if (_source[_current] != expected) return false;

        _current++;
        return true;
    }
    
    private char peek() {
        if (IsAtEnd()) return '\0';
        return _source[_current];
    }

    private bool IsAtEnd() {
        return _current >= _source.Length;
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
