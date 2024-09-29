#pragma once

#include <vector>
#include "common.hpp"
#include "value.hpp"

enum OpCode {
    OP_CONSTANT,
    OP_CONSTANT_LONG,
    OP_RETURN,
};

struct LineInfo {
    size_t code_index;
    int line;
    LineInfo(size_t code_index, int line);
};

struct Chunk {
    std::vector<u8> code;
    // Stores the index of the first op code on a new line
    // Any op codes after that that are on the same line won't be added
    // The op code on the next line gets added
    std::vector<LineInfo> lines;
    std::vector<Value> values;

    void write(u8 byte, int line);
    void write_constant(Value value, int line);
    size_t add_constant(Value value);
    int get_line(size_t code_index) const;
};

