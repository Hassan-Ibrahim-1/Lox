#pragma once

#include "chunk.hpp"

void disassemble_chunk(const Chunk& chunk, const std::string& name);
size_t disassemble_instruction(const Chunk& chunk, size_t offset);

