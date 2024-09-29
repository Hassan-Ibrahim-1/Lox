#pragma once

#include "chunk.hpp"
#include "common.hpp"
#include "value.hpp"

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

    InterpretResult run();
    u8 read_byte();
    Value read_constant();
    Value read_long_constant();
};

