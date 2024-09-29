#include <cstdio>
#include "debug.hpp"
#include "chunk.hpp"
#include "value.hpp"

static size_t simple_instruction(const std::string& op_name, size_t offset);
static size_t constant_instruction(const std::string& op_name, const Chunk& chunk, size_t offset);
static size_t long_constant_instruction(const std::string& op_name, const Chunk& chunk, size_t offset);

void disassemble_chunk(const Chunk& chunk, const std::string& name) {
    printf("== %s ==\n", name.c_str());
    for (size_t offset = 0; offset < chunk.code.size();) {
        offset = disassemble_instruction(chunk, offset);
    }
}

size_t disassemble_instruction(const Chunk& chunk, size_t offset) {
    printf("%04zu ", offset);
    if (offset > 0) {
        int l1 = chunk.get_line(offset);
        int l2 = chunk.get_line(offset-2);
    }
    if (offset > 0 && chunk.get_line(offset) == chunk.get_line(offset-1)) {
        printf("   | ");
    }
    else {
        printf("%4d ", chunk.get_line(offset));
    }
    u8 instruction = chunk.code[offset];
    switch (instruction) {
        case OP_RETURN:
            return simple_instruction("OP_RETURN", offset);
        case OP_CONSTANT:
            return constant_instruction("OP_CONSTANT", chunk, offset);
        case OP_CONSTANT_LONG:
            return long_constant_instruction("OP_CONSTANT_LONG", chunk, offset);
        case OP_NEGATE:
            return simple_instruction("OP_NEGATE", offset);
        default:
            printf("Unknown opcode: %u\n", instruction);
            return offset+1;
    }
}

static size_t simple_instruction(const std::string& op_name, size_t offset) {
    printf("%s\n", op_name.c_str());
    return offset + 1;
}

static size_t constant_instruction(const std::string& op_name, const Chunk& chunk, size_t offset) {
    u8 const_index = chunk.code[offset + 1];
    printf("%-16s %4d '", op_name.c_str(), const_index);
    print_value(chunk.values[const_index]);
    printf("'\n");
    return offset + 2;
}

static size_t long_constant_instruction(const std::string& op_name, const Chunk& chunk, size_t offset) {
    u32 const_index =
        (chunk.code[offset + 1]) |
        (chunk.code[offset + 2] << 8) |
        (chunk.code[offset + 3] << 16);
    printf("%-16s %4d '", op_name.c_str(), const_index);
    print_value(chunk.values[const_index]);
    printf("'\n");
    return offset + 4;
}

