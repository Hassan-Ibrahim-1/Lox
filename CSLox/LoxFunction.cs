namespace Lox;

public class LoxFunction : ILoxCallable {
    private readonly Function _declaration;
    // Scope where the function is declared in
    private readonly Environment _closure;
    private readonly bool _isInitializer;

    public LoxFunction(Function declaration, Environment closure, bool isInitializer) {
        this._declaration = declaration;
        this._closure = closure;
        this._isInitializer = isInitializer;
    }

    /// Bind an instance of a class to a newly created environment in the method when called
    /// Returns a new instance of the method referenced with the new binding
    public LoxFunction Bind(LoxInstance instance) {
        Environment environment = new Environment(_closure);
        environment.Define(instance);
        return new LoxFunction(_declaration, environment, _isInitializer);
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
            // If there is an empty return in an initializer then return 'this'
            if (_isInitializer) return _closure.GetAt(0, 0);

            return e.value;
        }

        // Return 'this'
        if (_isInitializer) return _closure.GetAt(0, 0);
        return new Nil();
    }

    public int Arity() {
        return _declaration.functionExpr.parameters.Count;
    }

    public override string ToString() {
        return $"<fn {_declaration.name.lexeme}>";
    }
}
