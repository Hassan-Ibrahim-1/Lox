namespace Lox;

public class Environment {
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();

    public void Define(Token name, object value) {
        if (values.ContainsKey(name.lexeme)) {
            throw new RuntimeError(name, $"Variable '{name.lexeme}' has already been defined");
        }
        values.Add(name.lexeme, value);
    }

    public object Get(Token name) {
        if (values.ContainsKey(name.lexeme)) {
            return values[name.lexeme];
        }
        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
    }

    public void Assign(Token name, object value) {
        if (values.ContainsKey(name.lexeme)) {
            values[name.lexeme] = value;
            return;
        }
        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
    }
}
