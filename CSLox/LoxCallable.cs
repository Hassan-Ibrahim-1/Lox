namespace Lox;

public interface LoxCallable {
    object Call(Interpreter interpreter, List<object> arguments);
    int Arity();
}

