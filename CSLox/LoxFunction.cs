namespace Lox;

public class LoxFunction : LoxCallable {
    private readonly Function _declaration;

    public LoxFunction(Function declaration) {
        this._declaration = declaration;
    }

    public object Call(Interpreter interpreter, List<object> arguments) {
        Environment environment = new Environment(interpreter.globals);
        
        // Maps arguments to parameters
        for (int i = 0; i < arguments.Count; i++) {
            environment.Define(_declaration.parameters[i], arguments[i]);
        }

        try {
            interpreter.ExecuteBlock(_declaration.body, environment);
        }
        catch (ReturnException e) {
            return e.value;
        }
        return new Nil();
    }

    public int Arity() {
        return _declaration.parameters.Count;
    }

    public override string ToString() {
        return $"<fn {_declaration.name}>";
    }
}
