namespace Lox;

public class LoxClass : ILoxCallable {
    public readonly string name;

    public LoxClass(string name) {
        this.name = name;
    }

    public override string ToString() {
        return this.name;
    }

    public int Arity() {
        return 0;
    }

    public object Call(Interpreter interpreter, List<object> args) {
        LoxInstance instance = new LoxInstance(this);
        return instance;
    }
}
