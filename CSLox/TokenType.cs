namespace Lox;

public enum TokenType {
    // Single character tokens
    Left_Paren, Right_Paren, Left_Brace, Right_Brace,
    Comma, Dot, Minus, Plus, SemiColon, Slash, Star,

    // One or two character tokens
    Bang, Bang_Equal, Equal, Equal_Equal,
    Greater, Greater_Equal, Less, Less_Equal,
    Question, Colon, Plus_Plus, Minus_Minus,

    // Identifiers
    Identifier, String, Number,

    // Keywords
    And, Class, Else, False, Fun, For, If, Nil, Or,
    Print, Return, Super, This, True, Var, While,
    Break, Static,

    EOF
}
