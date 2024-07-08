namespace Lox;

public class Clock : LoxCallable {
    public int Arity() {
        return 0;
    }

    public object Call(Interpreter interpreter, List<object> arguments) {
        return (double)DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public override string ToString() {
        return "<native fn>";
    }
}

public class Exit : LoxCallable {
    public int Arity() {
        return 1;
    }

    public object Call(Interpreter interpreter, List<object> arguments) {
        System.Environment.Exit(Convert.ToInt32(arguments[0]));
        return null!;
    }

    public override string ToString() {
        return "<native fn>";
    }
}
