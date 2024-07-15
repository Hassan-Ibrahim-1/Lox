namespace Lox;

public class LoxClass : ILoxCallable {
    public readonly string name;
    private readonly Dictionary<string, LoxFunction> methods;

    public LoxClass(string name, Dictionary<string, LoxFunction> methods) {
        this.name = name;
        this.methods = methods;
    }

    public LoxFunction FindMethod(Token name) {
        if (methods.TryGetValue(name.lexeme, out LoxFunction? method)) {
            return method;
        }

        return null!;
    }

    public int Arity() {
        return 0;
    }

    public object Call(Interpreter interpreter, List<object> args) {
        LoxInstance instance = new LoxInstance(this);
        return instance;
    }

    public override string ToString() {
        return this.name;
    }
}
