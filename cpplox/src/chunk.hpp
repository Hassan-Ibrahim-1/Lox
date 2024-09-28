#pragma once

#include <vector>
#include "common.hpp"
#include "value.hpp"

enum OpCode {
    OP_CONSTANT,
    OP_RETURN,
};

struct Chunk {
    std::vector<u8> code;
    std::vector<Value> values;

    void write(u8 byte);
    size_t add_constant(Value value);
};

