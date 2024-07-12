namespace Lox.Collections;

class HashMap<TK, TV> : OrderedDictionary<TK, TV> where TK : notnull {
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

    public int GetIndex(TK key) {
        if (!this.ContainsKey(key)) throw new KeyNotFoundException();

        for (int i = 0; i < this.Count; i++) {
            if (this.ElementAt(i).Key.Equals(key)) {
                return i;
            }
        }
        // Never reached
        return -1;
    }
}
