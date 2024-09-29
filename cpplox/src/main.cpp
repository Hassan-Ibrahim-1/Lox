#include "chunk.hpp"
#include "debug.hpp"
#include "vm.hpp"

int main() {
    VM vm;
    vm.init();
    Chunk* chunk = new Chunk;
    /*auto chunk = std::make_unique<Chunk>();*/
    size_t constant = chunk->add_constant(1.2);
    size_t constant2 = chunk->add_constant(7);
    chunk->write(OP_CONSTANT, 123);
    chunk->write(constant, 123);
    chunk->write(OP_NEGATE, 123);
    chunk->write(OP_RETURN, 123);
    if (vm.interpret(chunk) == INTERPRET_OK) {
        printf("INTERPRET_OK\n");
    }
    /*disassemble_chunk(*chunk, "test chunk");*/
    delete chunk;
    vm.free();

    return 0;
}
