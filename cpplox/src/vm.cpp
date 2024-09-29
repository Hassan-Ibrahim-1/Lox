#include "vm.hpp"
#include "chunk.hpp"
#include "value.hpp"

void VM::init() {

}

void VM::free() {

}

InterpretResult VM::interpret(Chunk* chunk) {
    _chunk = chunk;
    _ip = chunk->code.data();

    return run();
}

InterpretResult VM::run() {
    while (true) {
        u8 instruction = read_byte();
        switch(instruction) {
            case OP_RETURN: {
                return INTERPRET_OK;
            }
            case OP_CONSTANT: {
                Value value = read_constant();
                print_value(value);
                printf("\n");
                break;
            }
            case OP_CONSTANT_LONG: {
                Value value = read_long_constant();
                print_value(value);
                printf("\n");
                break;
            }
        }
    }
}

u8 VM::read_byte() {
    return *_ip++;
}
Value VM::read_constant() {
    return _chunk->values[read_byte()];
}

Value VM::read_long_constant() {
    u32 const_index =
        (_chunk->code[read_byte()]) |
        (_chunk->code[read_byte()] << 8) |
        (_chunk->code[read_byte()] << 16);
    return _chunk->values[const_index];
}

