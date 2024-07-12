namespace Lox.Collections;

class HashMap<TK, TV> : Dictionary<TK, TV> where TK : notnull {

    public TV Get(TK key) {
        if (this.TryGetValue(key, out TV? value)) {
            return value;
        }
        
        // null?
        return default(TV)!;
    }

    public TV Put(TK key, TV value) {
        TV? old_value = default(TV);

        if (this.ContainsKey(key)) {
            old_value = this[key];
            this[key] = value;
            
        }
        else {
            this.Add(key, value);
        }

        return old_value!;
    }
}
