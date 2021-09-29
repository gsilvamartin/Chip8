using EmuLite.Chip8;
using SDL2;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using static SDL2.SDL;

namespace EmuLite
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cpu = new CPU();
            var (display, renderer) = Display.InitDisplay();

            cpu.InitializeChip();
            cpu.LoadROM("C:\\Users\\Guilherme Martin\\Downloads/test_opcode.ch8");

            while (true)
            {
                cpu.EmulateCycle();

                SDL_SetRenderDrawColor(renderer, 0x00, 0x00, 0x00, 0xFF);
                SDL_RenderClear(renderer);
                SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);

                int rowNum;

                for (int y = 0; y < 32; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        rowNum = y * 64;
                        if (cpu.display[x + rowNum] != 0)
                            SDL_RenderDrawPoint(renderer, x, y);
                    }
                }

                SDL_RenderPresent(renderer);
            }
        }
    }
}
