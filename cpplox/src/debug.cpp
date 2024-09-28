#include <cstdio>
#include "debug.hpp"

static size_t simple_instruction(const std::string& op_name, size_t offset);

void disassemble_chunk(const Chunk& chunk, const std::string& name) {
    printf("== %s ==\n", name.c_str());
    for (size_t offset = 0; offset < chunk.code.size();) {
        offset = disassemble_instruction(chunk, offset);
    }
}

size_t disassemble_instruction(const Chunk& chunk, size_t offset) {
    printf("%04zu ", offset);
    OpCode instruction = chunk.code[offset];
    switch (instruction) {
    case OP_RETURN:
        return simple_instruction("OP_RETURN", offset);
    default:
        printf("Unknown opcode: %u\n", instruction);
        return offset+1;
    }
}

static size_t simple_instruction(const std::string& op_name, size_t offset) {
    printf("%s\n", op_name.c_str());
    return offset + 1;
}

