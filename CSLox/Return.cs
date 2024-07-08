namespace Lox;

public class ReturnException : Exception {
    public readonly object value;

    public ReturnException(object value) {
        this.value = value;
    }
}
