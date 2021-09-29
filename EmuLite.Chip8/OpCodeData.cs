using System;
using System.Collections.Generic;
using System.Text;

namespace EmuLite.Chip8
{
    struct OpCodeData
    {
        public ushort NNN, OpCode;
        public byte NN, N, X, Y;
    }
}
