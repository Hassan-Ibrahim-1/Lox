#include "chunk.hpp"

LineInfo::LineInfo(size_t code_index, int line)
    : code_index(code_index), line(line) {}

void Chunk::write(u8 byte, int line) {
    // This probably isn't necessary
    if (code.capacity() < code.size() + 1) {
        size_t capacity = code.capacity() < 8 ? 8 : code.capacity() * 2;
        code.reserve(capacity);
    }
    code.push_back(byte);
    if (lines.size() == 0 || lines.back().line != line) {
        lines.push_back(LineInfo(code.size()-1, line));
    }
}

size_t Chunk::add_constant(Value value) {
    values.push_back(value);
    return values.size() - 1;
}

int Chunk::get_line(size_t code_index) const {
    for (size_t i = 0; i < lines.size(); i++) {
        if (code_index == lines[i].code_index) {
            return lines[i].line;
        }
        else if (code_index < lines[i].code_index) {
            return lines[i-1].line;
        }
    }
    // Never reached?
    return -1;
}

