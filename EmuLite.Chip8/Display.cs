using SDL2;
using System;
using System.Collections.Generic;
using System.Text;
using static SDL2.SDL;

namespace EmuLite.Chip8
{
    public class Display
    {
        public static Tuple<IntPtr, IntPtr> InitDisplay()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
            {
                throw new Exception("Error: SDL Load failed");
            }

            IntPtr window = SDL.SDL_CreateWindow("Chip-8", 128, 128, 64 * 8, 32 * 8, 0);

            if (window == IntPtr.Zero)
            {
                throw new Exception("SDL could not create a window.");
            }

            IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            if (renderer == IntPtr.Zero)
            {
                throw new Exception("SDL could not create a valid renderer.");
            }

            return Tuple.Create(window, renderer);
        }
    }
}
