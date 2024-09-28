#include "chunk.hpp"
#include "debug.hpp"

int main() {
    Chunk* chunk = new Chunk;
    chunk->write(OP_RETURN);
    disassemble_chunk(*chunk, "test chunk");
    delete chunk;

    return 0;
}
