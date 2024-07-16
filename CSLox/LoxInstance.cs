using Lox.Collections;

namespace Lox;

public class LoxInstance {
    private LoxClass _loxClass = default!;
    private bool _staticInstance = false;

    // NOTE: Could just be a regular dictionary. Maybe?
    private readonly HashMap<string, object> fields = new HashMap<string, object>();

    public LoxInstance(LoxClass loxClass) {
        this._loxClass = loxClass;
    }

    // For static methods
    public LoxInstance() {}

    public object Get(Token name) {
        if (fields.TryGetValue(name.lexeme, out object? value)) {
            return value;
        }

        LoxFunction method = _loxClass.FindMethod(name.lexeme);
        // Create a new environment when a method is encountered at runtime
        if (method != null) {
            if (_staticInstance && !method.isStatic) {
                throw new RuntimeError(name, "Can't call a nonstatic method on a static instance.");
            }
            if (!_staticInstance && method.isStatic) {
                throw new RuntimeError(name, "Can't call a static method on a nonstatic instance.");
            }
            
            return method.Bind(this);
        }

        LoxGetter getter = _loxClass.FindGetter(name.lexeme);
        if (getter != null) {
            return getter.Bind(this);
        }

        throw new RuntimeError(name, $"Undefined property {name.lexeme}.");
    }

    public void Set(Token name, object value) {
        fields.Put(name.lexeme, value);
    }

    protected void SetLoxClass(LoxClass loxClass) {
        this._loxClass = loxClass;
        _staticInstance = true;
    }
    
    public override string ToString() {
        return $"{_loxClass.name} instance";
    }
}
