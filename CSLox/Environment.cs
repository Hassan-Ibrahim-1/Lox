namespace Lox;

public class Environment {
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();
    public readonly Environment enclosing;

    public Environment(Environment enclosing = null!) {
        this.enclosing = enclosing;
    }

    public void Define(Token name, object value) {
        if (values.ContainsKey(name.lexeme)) {
            throw new RuntimeError(name, $"Variable '{name.lexeme}' has already been defined");
        }
        values.Add(name.lexeme, value);
    }

    public object Get(Token name) {
        if (values.ContainsKey(name.lexeme)) {
            if (values[name.lexeme] != null) {
                return values[name.lexeme];
            }
            throw new RuntimeError(name, $"Variable '{name.lexeme}' has not been assigned to any value.");
        }
        
        // When it reaches global scope and the variable isn't found the runtime error is thrown
        if (enclosing != null) return enclosing.Get(name);

        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
    }

    public object GetAt(Token name, int distance) {
        return Ancestor(distance).values[name.lexeme];
    }

    private Environment Ancestor(int distance) {
        // if distance is 0 then the variable is the in the current environment
        Environment environment = this;

        for (int i = 0; i < distance; i++) {
            environment = environment.enclosing;
        }

        return environment;
    }

    public void Assign(Token name, object value) {
        if (values.ContainsKey(name.lexeme)) {
            values[name.lexeme] = value;
            return;
        }
        
        // Assign to any matching variable above current scope
        if (enclosing != null) {
            enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
    }

    public void AssignAt(Token name, object value, int distance) {
        Ancestor(distance).values[name.lexeme] = value;
    }
}
