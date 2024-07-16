namespace Lox;

public class LoxGetter : ILoxCallable {
    private readonly Getter _declaration;
    private readonly Environment _closure;

    public LoxGetter(Getter declaration, Environment closure) {
        this._declaration = declaration;
        this._closure = closure;
    }

    public LoxGetter Bind(LoxInstance instance) {
        Environment environment = new Environment(_closure);
        environment.Define(instance);
        return new LoxGetter(_declaration, environment);
    }

    // args are never taken because arity is always 0
    public object Call(Interpreter interpreter, List<object> args = null!) {
        Environment environment = new Environment(_closure);
        
        try {
            interpreter.ExecuteBlock(_declaration.statements, environment);
            throw new RuntimeError(_declaration.name, "A getter must return a value");
        }
        catch (ReturnException e) {
            return e.value;
        }
    }
    
    public int Arity() => 0;

    public override string ToString() {
        return $"<getter {_declaration.name.lexeme}>";
    }
}
