namespace Lox;

public class LoxFunction : LoxCallable {
    private readonly Function _declaration;
    // Scope where the function is declared in
    private readonly Environment _closure;

    public LoxFunction(Function declaration, Environment closure) {
        this._declaration = declaration;
        this._closure = closure;
    }

    public object Call(Interpreter interpreter, List<object> arguments) {
        Environment environment = new Environment(_closure);
        
        // Map arguments to parameters
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
