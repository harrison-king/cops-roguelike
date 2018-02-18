using System;

namespace Roguelike
{
    class MainClass
    {
        private const int MAP_WIDTH = 80;
        private const int MAP_HEIGHT = 24;

        private static Cop _player = new Cop();

        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();

            RunGame();


            Console.Clear();
        }

        private static void RunGame()
        {
            // Run the main game loop.
            bool isRunning  = true;
            while (isRunning)
            {
                // Display the game.
                DisplayGame();

                // Process the player input.
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        isRunning = false;
                        break;
                    default:
                        ProcessInput(input.Key);
                        break;
                }
            }
        }

        private static void DisplayGame()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.SetCursorPosition(_player.X, _player.Y);
            Console.Write('@');
        }

        private static void ProcessInput(ConsoleKey input)
        {
            switch (input)
            {
                case ConsoleKey.LeftArrow:
                    if (_player.X > 0)
                        --_player.X;
                    break;
                case ConsoleKey.RightArrow:
                    if (_player.X + 1 < MAP_WIDTH)
                        ++_player.X;
                    break;
                case ConsoleKey.UpArrow:
                    if (_player.Y > 0)
                        --_player.Y;
                    break;
                case ConsoleKey.DownArrow:
                    if (_player.Y + 1 < MAP_HEIGHT)
                        ++_player.Y;
                    break;
            }
        }
    }
}
