namespace Lox;

public class Environment {
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();

    public void Define(string name, object value) {
        values.Add(name, value);
    }

    public object Get(Token token) {
        if (values.ContainsKey(token.lexeme)) {
            return values[token.lexeme];
        }
        throw new RuntimeError(token, $"Undefined variable '{token.lexeme}'.");
    }
}
