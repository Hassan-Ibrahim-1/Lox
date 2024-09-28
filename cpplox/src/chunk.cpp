#include "chunk.hpp"

void Chunk::write(OpCode byte) {
    if (code.capacity() < code.size() + 1) {
        size_t capacity = code.capacity() < 8 ? 8 : code.capacity() * 2;
        code.reserve(capacity);
    }
    code.push_back(byte);
}

