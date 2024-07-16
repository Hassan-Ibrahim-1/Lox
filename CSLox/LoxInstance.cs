using Lox.Collections;

namespace Lox;

public class LoxInstance {
    private LoxClass _loxClass;
    // NOTE: Could just be a regular dictionary. Maybe?
    private readonly HashMap<string, object> fields = new HashMap<string, object>();

    public LoxInstance(LoxClass loxClass) {
        this._loxClass = loxClass;
    }

    public object Get(Token name) {
        if (fields.TryGetValue(name.lexeme, out object? value)) {
            return value;
        }

        LoxFunction method = _loxClass.FindMethod(name);
        // Create a new environment when a method is encountered at runtime
        if (method != null) {
            return method.Bind(this);
        }

        throw new RuntimeError(name, $"Undefined property {name.lexeme}.");
    }

    public void Set(Token name, object value) {
        fields.Put(name.lexeme, value);
    }
    
    public override string ToString() {
        return $"{_loxClass.name} instance";
    }
}
