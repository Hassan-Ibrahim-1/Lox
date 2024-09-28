#include "chunk.hpp"
#include "debug.hpp"

int main() {
    Chunk* chunk = new Chunk;
    size_t constant = chunk->add_constant(1.2);
    chunk->write(OP_CONSTANT);
    chunk->write(constant);
    disassemble_chunk(*chunk, "test chunk");
    delete chunk;

    return 0;
}
