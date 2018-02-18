using System;
using System.Collections.Generic;

namespace Roguelike
{
    class MainClass
    {
        private const int MAP_WIDTH = 80;
        private const int MAP_HEIGHT = 24;

        private static Random _random = new Random();

        private static Cop _player = new Cop();
        private static List<Enemy> _enemies = new List<Enemy>();

        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();

            GenerateEnemies();
            RunGame();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            // Check if we won.
            if (_enemies.Count == 0)
            {
                Console.WriteLine("You won!!!");
            }
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

                // Check if we won (no enemies left).
                if (_enemies.Count == 0)
                {
                    isRunning = false;
                }
            }
        }

        private static void DisplayGame()
        {
            Console.Clear();

            // Display the player.
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.SetCursorPosition(_player.X, _player.Y);
            Console.Write('@');

            // Display the enemies.
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (Enemy enemy in _enemies)
            {
                Console.SetCursorPosition(enemy.X, enemy.Y);
                Console.Write('e');
            }
        }

        private static void ProcessInput(ConsoleKey input)
        {
            // Move the player.
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

            // Check if we killed any enemies.
            int killedEnemyIndex = -1;
            for (int i = 0; i < _enemies.Count; ++i)
            {
                Enemy enemy = _enemies[i];
                if (_player.X == enemy.X
                    && _player.Y == enemy.Y)
                {
                    killedEnemyIndex = i;
                    break;
                }
            }

            if (killedEnemyIndex != -1)
            {
                _enemies.RemoveAt(killedEnemyIndex);
            }
        }

        private static void GenerateEnemies()
        {
            int enemyNumber = _random.Next(10, 20);
            for (int i = 0; i < enemyNumber; ++i)
            {
                Enemy enemy = new Enemy()
                {
                    X = _random.Next(MAP_WIDTH - 1),
                    Y = _random.Next(MAP_HEIGHT - 1)
                };

                _enemies.Add(enemy);
            }
        }
    }
}
