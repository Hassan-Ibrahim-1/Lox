#pragma once

#include <vector>

enum OpCode {
    OP_RETURN,
};

struct Chunk {
    std::vector<OpCode> code;

    void write(OpCode byte);
};

