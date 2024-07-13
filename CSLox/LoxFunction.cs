namespace Lox;

public class LoxFunction : ILoxCallable {
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
            environment.Define(arguments[i]);
        }

        try {
            interpreter.ExecuteBlock(_declaration.functionExpr.body, environment);
        }
        catch (ReturnException e) {
            return e.value;
        }
        return new Nil();
    }

    public int Arity() {
        return _declaration.functionExpr.parameters.Count;
    }

    public override string ToString() {
        return $"<fn {_declaration.name.lexeme}>";
    }
}
