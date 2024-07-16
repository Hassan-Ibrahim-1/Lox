namespace Lox;

public class LoxClass : LoxInstance, ILoxCallable {
    public readonly string name;
    private readonly Dictionary<string, LoxFunction> _methods;
    private readonly Dictionary<string, LoxGetter> _getters;

    public LoxClass(string name, Dictionary<string, LoxFunction> methods, Dictionary<string, LoxGetter> getters) {
        this.name = name;
        this._methods = methods;
        this._getters = getters;
        SetLoxClass(this);
    }

    public LoxFunction FindMethod(string name) {
        if (_methods.TryGetValue(name, out LoxFunction? method)) {
            return method;
        }
        return null!;
    }

    public LoxGetter FindGetter(string name) {
        if (_getters.TryGetValue(name, out LoxGetter? getter)) {
            return getter;
        }
        return null!;
    }

    public int Arity() {
        LoxFunction initializer = FindMethod("init");
        if (initializer == null) return 0;
        return initializer.Arity(); // the class has the same arity as the init function
    }

    /// Create a new instance
    public object Call(Interpreter interpreter, List<object> args) {
        LoxInstance instance = new LoxInstance(this);
        LoxFunction initializer = FindMethod("init");
        if (initializer != null) {
            // Create an init function that has the new instance binded to it and call that instead
            initializer.Bind(instance).Call(interpreter, args);
        }
        return instance;
    }

    public override string ToString() {
        return this.name;
    }
}
