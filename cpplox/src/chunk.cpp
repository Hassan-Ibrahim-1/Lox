#include "chunk.hpp"

void Chunk::write(u8 byte) {
    // This probably isn't necessary
    if (code.capacity() < code.size() + 1) {
        size_t capacity = code.capacity() < 8 ? 8 : code.capacity() * 2;
        code.reserve(capacity);
    }
    code.push_back(byte);
}

size_t Chunk::add_constant(Value value) {
    values.push_back(value);
    return values.size() - 1;
}

