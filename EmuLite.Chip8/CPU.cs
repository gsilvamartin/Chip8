using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EmuLite.Chip8
{
    public class CPU
    {
        #region CPU Variables
        Random rnd;
        public byte[] memory;
        public uint[] display;
        public byte[] V;
        public ushort[] stack;
        public byte SP;
        public ushort PC;
        public ushort I;
        public ushort key;
        public ushort opcode;
        public byte delay_timer;
        public byte sound_timer;
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
        #endregion

        #region CPU Methods
        public void InitializeChip()
        {
            memory = new byte[4096];
            display = new uint[64 * 32];
            stack = new ushort[16];
            V = new byte[16];

            SP = 0;
            I = 0;
            PC = 0x200;
            delay_timer = 0;
            sound_timer = 0;
            rnd = new Random();
        }

        public void EmulateCycle()
        {
            opcode = (ushort)(memory[PC] << 8 | memory[PC + 1]);

            var opdata = new OpCodeData
            {
                OpCode = opcode,
                X = (byte)(opcode & 0x0F00 >> 8),
                Y = (byte)(opcode & 0x00F0 >> 4),
                N = (byte)(opcode & 0x000F),
                NN = (byte)(opcode & 0x00FF),
                NNN = (ushort)(opcode & 0x0FFF)
            };

            PC += 2;

            switch (opcode & 0xF000)
            {
                case 0x0000:
                    switch (opcode)
                    {
                        case 0x00E0:
                            FN_00E0(opdata);
                            break;

                        case 0x00EE:
                            FN_00EE(opdata);
                            break;
                    }
                    break;

                case 0x1000:
                    FN_1NNN(opdata);
                    break;

                case 0x2000:
                    FN_2NNN(opdata);
                    break;

                case 0x3000:
                    FN_3XNN(opdata);
                    break;

                case 0x4000:
                    FN_4XNN(opdata);
                    break;

                case 0x5000:
                    FN_5XY0(opdata);
                    break;

                case 0x6000:
                    FN_6XNN(opdata);
                    break;

                case 0x7000:
                    FN_7XNN(opdata);
                    break;

                case 0x8000:
                    switch (opcode)
                    {
                        case 0x8000:
                            FN_8XY0(opdata);
                            break;

                        case 0x8001:
                            FN_8XY1(opdata);
                            break;

                        case 0x8002:
                            FN_8XY2(opdata);
                            break;

                        case 0x8003:
                            FN_8XY3(opdata);
                            break;

                        case 0x8004:
                            FN_8XY4(opdata);
                            break;

                        case 0x8005:
                            FN_8XY5(opdata);
                            break;

                        case 0x8006:
                            FN_8XY6(opdata);
                            break;

                        case 0x8007:
                            FN_8XY7(opdata);
                            break;

                        case 0x800E:
                            FN_8XYE(opdata);
                            break;
                    }
                    break;

                case 0x9000:
                    FN_9XY0(opdata);
                    break;

                case 0xA000:
                    FN_ANNN(opdata);
                    break;

                case 0xB000:
                    FN_BNNN(opdata);
                    break;

                case 0xC000:
                    FN_CXNN(opdata);
                    break;

                case 0xD000:
                    FN_DXYN(opdata);
                    break;

                case 0xE000:
                    switch (opcode)
                    {
                        case 0xE00E:
                            FN_EX9E(opdata);
                            break;

                        case 0xE001:
                            FN_EXA1(opdata);
                            break;
                    }
                    break;

                case 0xF000:
                    switch (opcode)
                    {
                        case 0xF007:
                            FN_FX07(opdata);
                            break;

                        case 0xF00A:
                            FN_FX0A(opdata);
                            break;

                        case 0xF015:
                            FN_FX15(opdata);
                            break;

                        case 0xF018:
                            FN_FX18(opdata);
                            break;

                        case 0xF01E:
                            FN_FX1E(opdata);
                            break;

                        case 0xF029:
                            FN_FX29(opdata);
                            break;

                        case 0xF033:
                            FN_FX33(opdata);
                            break;

                        case 0xF055:
                            FN_FX55(opdata);
                            break;

                        case 0xF065:
                            FN_FX65(opdata);
                            break;
                    }
                    break;
            }

            if (delay_timer > 0)
                --delay_timer;

            if (sound_timer > 0)
                --sound_timer;
        }

        public void LoadROM(string path)
        {
            LoadFont();

            var rom = File.ReadAllBytes(path);

            for (int i = 0; i < rom.Length; i++)
            {
                memory[i + 0x200] = rom[i];
            }
        }

        public void LoadFont()
        {
            for (int i = 0; i < 80; i++)
            {
                memory[0x50 + i] = build_font[i];
            }
        }

        public void KeyPressed(byte key)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region OpCode Functions
        private void FN_00E0(OpCodeData data)
        {
            Array.Clear(display, 0, display.Length);
        }

        private void FN_00EE(OpCodeData data)
        {
            SP--;
            PC = stack[SP];
        }

        private void FN_1NNN(OpCodeData data)
        {
            PC = data.NNN;
        }

        private void FN_2NNN(OpCodeData data)
        {
            stack[SP] = PC;
            SP++;
            PC = data.NNN;
        }

        private void FN_3XNN(OpCodeData data)
        {
            if (V[data.X] == data.NN)
                PC += 2;
        }

        private void FN_4XNN(OpCodeData data)
        {
            if (V[data.X] != data.NN)
                PC += 2;
        }

        private void FN_5XY0(OpCodeData data)
        {
            if (V[data.X] == V[data.Y])
                PC += 2;
        }

        private void FN_6XNN(OpCodeData data)
        {
            V[data.X] = data.NN;
        }

        private void FN_7XNN(OpCodeData data)
        {
            V[data.X] += data.NN;
        }

        private void FN_8XY0(OpCodeData data)
        {
            V[data.X] = V[data.Y];
        }

        private void FN_8XY1(OpCodeData data)
        {
            V[data.X] |= V[data.Y];
        }

        private void FN_8XY2(OpCodeData data)
        {
            V[data.X] &= V[data.Y];
        }

        private void FN_8XY3(OpCodeData data)
        {
            V[data.X] ^= V[data.Y];
        }

        private void FN_8XY4(OpCodeData data)
        {
            var sum = (byte)(V[data.X] + V[data.Y]);

            V[0xF] = (byte)(sum > 255 ? 1 : 0);
            V[data.X] = (byte)(sum & 0xFF);
        }

        private void FN_8XY5(OpCodeData data)
        {
            var sub = V[data.X] - V[data.Y];

            V[0xF] = (byte)(V[data.X] > V[data.Y] ? 1 : 0);
            V[data.X] = (byte)sub;
        }

        private void FN_8XY6(OpCodeData data)
        {
            V[0xF] = (byte)(V[data.X] & 0x1); //LSB
            V[data.X] >>= 1;
        }

        private void FN_8XY7(OpCodeData data)
        {
            var sub = V[data.Y] - V[data.X];

            V[0xF] = (byte)(V[data.Y] > V[data.X] ? 1 : 0);
            V[data.X] = (byte)sub;
        }

        private void FN_8XYE(OpCodeData data)
        {
            V[0xF] = (byte)((V[data.X] & 0x80) >> 7); //MSB
            V[data.X] <<= 1;
        }

        private void FN_9XY0(OpCodeData data)
        {
            if (V[data.X] != V[data.Y])
                PC += 2;
        }

        private void FN_ANNN(OpCodeData data)
        {
            I = data.NNN;
        }

        private void FN_BNNN(OpCodeData data)
        {
            PC = (ushort)(data.NNN + V[0]);
        }

        private void FN_CXNN(OpCodeData data)
        {
            V[data.X] = (byte)(rnd.Next(0, 255) & data.NN);
        }

        private void FN_DXYN(OpCodeData data)
        {
            int x = V[(opcode & 0x0F00) >> 8];
            int y = V[(opcode & 0x00F0) >> 4];
            int n = opcode & 0x000F;

            V[15] = 0;

            for (int i = 0; i < n; i++)
            {
                byte mem = memory[I + i];

                for (int j = 0; j < 8; j++)
                {
                    byte pixel = (byte)((mem >> (7 - j)) & 0x01);
                    int index = x + j + (y + i) * 64;

                    if (index > 2047) continue;

                    if (pixel == 1 && display[index] != 0) V[15] = 1;

                    display[index] = (byte)((display[index] != 0 && pixel == 0) || (display[index] == 0 && pixel == 1) ? 0xffffffff : 0);//(byte)(display[index] ^ pixel);
                }
            }
        }

        private void FN_EX9E(OpCodeData data)
        {
            if (key == V[data.X])
                PC += 2;
        }

        private void FN_EXA1(OpCodeData data)
        {
            if (key != V[data.X])
                PC += 2;
        }

        private void FN_FX07(OpCodeData data)
        {
            V[data.X] = delay_timer;
        }

        private void FN_FX0A(OpCodeData data)
        {
            V[data.X] = (byte)key;
        }

        private void FN_FX15(OpCodeData data)
        {
            delay_timer = V[data.X];
        }

        private void FN_FX18(OpCodeData data)
        {
            sound_timer = V[data.X];
        }

        private void FN_FX1E(OpCodeData data)
        {
            I += V[data.X];
        }

        private void FN_FX29(OpCodeData data)
        {
            var digit = V[data.X];

            I = (ushort)(0x50 + (5 * digit));
        }

        private void FN_FX33(OpCodeData data)
        {
            var val = V[data.X];

            memory[I + 2] = (byte)(val % 10);
            val /= 10;

            memory[I + 1] = (byte)(val % 10);
            val /= 10;

            memory[I] = (byte)(val % 10);
        }

        private void FN_FX55(OpCodeData data)
        {
            for (int i = 0; i < V[data.X]; ++i)
                memory[I + i] = V[i];
        }

        private void FN_FX65(OpCodeData data)
        {
            for (int i = 0; i < V[data.X]; ++i)
                V[i] = memory[I + i];
        }
        #endregion
    }
}
