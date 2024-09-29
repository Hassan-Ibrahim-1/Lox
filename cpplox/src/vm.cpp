#include "vm.hpp"
#include "chunk.hpp"
#include "debug.hpp"
#include "common.hpp"
#include "value.hpp"
#include <cstddef>

void VM::init() {
    reset_stack();
}

void VM::free() {

}

InterpretResult VM::interpret(Chunk* chunk) {
    _chunk = chunk;
    _ip = chunk->code.data();

    return run();
}

void VM::push(Value value) {
    *_stack_top = value;
    _stack_top++;
}

Value VM::pop() {
    _stack_top--;
    return *_stack_top;
}

void VM::reset_stack() {
    _stack_top = _stack.data();
}

void VM::print_stack() {
    printf("          ");
    for (Value* slot = _stack.data(); slot < _stack_top; slot++) {
        printf("[");
        print_value(*slot);
        printf("]");
    }
}

InterpretResult VM::run() {
    while (true) {
    #ifdef DEBUG_TRACE_EXECUTION
        print_stack();
        printf("\n");
        disassemble_instruction(*_chunk, (size_t)(_ip - _chunk->code.data()));
    #endif
        u8 instruction = read_byte();
        switch(instruction) {
            case OP_RETURN: {
                printf("stack top: ");
                print_value(pop());
                printf("\n");
                return INTERPRET_OK;
            }
            case OP_CONSTANT: {
                Value value = read_constant();
                push(value);
                print_value(value);
                printf("\n");
                break;
            }
            case OP_CONSTANT_LONG: {
                Value value = read_long_constant();
                push(value);
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

