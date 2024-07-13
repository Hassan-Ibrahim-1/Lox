namespace Lox;

public class Environment {
    private readonly List<object> _values = new List<object>(); 
    public readonly Environment enclosing;

    public Environment(Environment enclosing = null!) {
        this.enclosing = enclosing;
    }

    public void Define(object value) {
        _values.Add(value);
    }

    public object GetAt(int index, int? distance) {
        return Ancestor(distance)._values[index];
    }

    public void AssignAt(int index, object value, int? distance) {
        Ancestor(distance)._values[index] = value;
    }

    private Environment Ancestor(int? distance) {
        // if distance is 0 then the variable is the in the current environment
        Environment environment = this;
        for (int i = 0; i < distance; i++) {
            environment = environment.enclosing;
        }

        return environment;
    }
}
