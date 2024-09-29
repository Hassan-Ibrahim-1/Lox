#pragma once

#include "chunk.hpp"
#include "common.hpp"
#include "value.hpp"

#define STACK_MAX 256

enum InterpretResult {
    INTERPRET_OK,
    INTERPRET_COMPILE_ERROR,
    INTERPRET_RUNTIME_ERROR,
};

class VM {
public:
    void init();
    void free();

    InterpretResult interpret(Chunk* chunk);

private:
    Chunk* _chunk;
    // What instruction is going to be executed next
    u8* _ip;

    std::array<Value, STACK_MAX> _stack;
    // points to the value past the element containing the actual top value
    // if it points to the 0th element it means the stack is empty
    Value* _stack_top;

    void push(Value value);
    Value pop();
    void reset_stack();
    void print_stack();

    InterpretResult run();
    u8 read_byte();
    Value read_constant();
    Value read_long_constant();
};

