using System;
using System.Collections.Generic;
using System.Text;

namespace EmuLite.Chip8
{
    public class Chip8
    {
        byte[] memory; // memoria
        byte[] display; //tela
        byte[] stack; //pilha
        byte[] V; //registradores
        ushort I; //indice registrador
        ushort opcode; //opcode
        ushort sp; //ponteiro pilha
        ushort pc; //contador ex: 0x200
        ushort delay_timer;
        ushort sound_timer;
        Random rnd;

        byte[] build_font = {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F     
        };

        public void InitializeChip()
        {
            stack = new byte[16];
            memory = new byte[4096];
            display = new byte[64 * 32];

            pc = 0x200;
            delay_timer = 0;
            sound_timer = 0;
            rnd = new Random();
        }

        public void EmulateCycle()
        {
            opcode = (ushort)(memory[pc] << 8 | memory[pc + 1]);

            byte X = (byte)(opcode & 0x0F00 >> 8);
            byte Y = (byte)(opcode & 0x00F0 >> 4);
            byte N = (byte)(opcode & 0x000F); // 4 bit
            byte NN = (byte)(opcode & 0x00FF); // 8 bit
            ushort NNN = (ushort)(opcode & 0x0FFF); // 12 bits

            switch (opcode & 0xF000)
            {
                case 0x0000:
                    switch (opcode)
                    {
                        case 0x00E0:
                            Array.Clear(display, 0, display.Length);
                            break;
                        case 0x00EE:
                            pc = stack[sp--];
                            break;
                    }
                    break;

                case 0x1000:
                    pc = NNN;
                    break;

                case 0x2000:
                    stack[++sp] = (byte)pc;
                    pc = NNN;
                    break;

                case 0x3000:
                    if (V[X] == NN)
                        pc += 2;
                    break;

                case 0x4000:
                    if (V[X] != NN)
                        pc += 2;
                    break;

                case 0x5000:
                    if (V[X] == V[Y])
                        pc += 2;
                    break;

                case 0x6000:
                    V[X] = NN;
                    break;

                case 0x7000:
                    V[X] += N;
                    break;

                case 0x8000:
                    switch (opcode & 0x000F)
                    {
                        case 0:
                            V[X] = V[Y];
                            break;
                        case 1:
                            V[X] = (byte)(V[X] | V[Y]);
                            break;
                        case 2:
                            V[X] = (byte)(V[X] & V[Y]);
                            break;
                        case 3:
                            V[X] = (byte)(V[X] ^ V[Y]);
                            break;
                        case 4:
                            if (V[Y] > (0xFF - V[X]))
                                V[0xF] = 1;
                            else
                                V[0xF] = 0;

                            V[X] += V[Y];
                            break;
                        case 5:
                            if (V[Y] > V[X])
                                V[0xF] = 0;
                            else
                                V[0xF] = 1;

                            V[X] -= V[Y];
                            break;
                        case 6:
                            V[0xF] = (byte)(V[X] & 0x1);
                            V[X] >>= 0x1;
                            break;
                        case 7:
                            int diff = V[Y] - V[X];
                            V[X] = (byte)(diff & 0xFF);
                            V[0xF] = (byte)(diff > 0 ? 1 : 0);
                            break;
                        case 0xE:
                            V[0xF] = (byte)((V[X] & 0x80) >> 7);
                            V[X] <<= 0x1;
                            break;
                    }
                    break;

                case 0x9000:
                    if (V[X] != V[Y])
                        pc += 2;
                    break;

                case 0xA000:
                    I = NNN;
                    break;

                case 0xB000:
                    pc = (ushort)(V[0] + NNN);
                    break;

                case 0xC000:
                    V[X] = (byte)(rnd.Next(0, 255) & NN);
                    break;
            }
        }
    }
}
