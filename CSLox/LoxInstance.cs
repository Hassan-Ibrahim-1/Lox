using Lox.Collections;

namespace Lox;

public class LoxInstance {
    private LoxClass _loxClass;
    // Could just be a regular dictionary
    private readonly Dictionary<string, object> fields = new Dictionary<string, object>();

    public LoxInstance(LoxClass loxClass) {
        this._loxClass = loxClass;
    }

    public object Get(Token name) {
        if (fields.TryGetValue(name.lexeme, out object? value)) {
            return value;
        }
        throw new RuntimeError(name, $"Undefined property {name.lexeme}.");
    }

    public void Set(Token name, object value) {
        fields.Add(name.lexeme, value);
    }
    
    public override string ToString() {
        return $"{_loxClass.name} instance";
    }
}
