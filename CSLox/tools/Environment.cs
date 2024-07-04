namespace Lox;

public class Environment {
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();

    public void Define(Token token, object value) {
        if (values.ContainsKey(token.lexeme)) {
            throw new RuntimeError(token, $"Variable '{token.lexeme}' has already been defined");
        }
        values.Add(token.lexeme, value);
    }

    public object Get(Token token) {
        if (values.ContainsKey(token.lexeme)) {
            return values[token.lexeme];
        }
        throw new RuntimeError(token, $"Undefined variable '{token.lexeme}'.");
    }
}
