namespace Lox;

public class LoxInstance {
    private LoxClass _loxClass;

    public LoxInstance(LoxClass loxClass) {
        this._loxClass = loxClass;
    }
    
    public override string ToString() {
        return $"{_loxClass.name} instance";
    }
}
