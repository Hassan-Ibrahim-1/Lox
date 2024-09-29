#include "chunk.hpp"
#include "debug.hpp"

int main() {
    Chunk* chunk = new Chunk;
    size_t constant = chunk->add_constant(1.2);
    chunk->write(OP_CONSTANT, 123);
    chunk->write(constant, 123);
    chunk->write(OP_RETURN, 123);
    chunk->write(OP_RETURN, 142);
    disassemble_chunk(*chunk, "test chunk");
    printf("line size: %zu\n", chunk->lines.size());
    delete chunk;

    return 0;
}
