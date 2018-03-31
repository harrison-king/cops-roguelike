using System;
using System.Collections.Generic;
using Roguelike.Common.Console;

namespace Roguelike
{
    class MainClass
    {
        private const int MAP_WIDTH = 80;
        private const int MAP_HEIGHT = 24;

        private static Random _random = new Random();

        private static Cop _player = new Cop();
        private static Map _map;

        public static void Main(string[] args)
        {
            ConsoleScreen.Initialize(MAP_WIDTH, MAP_HEIGHT, ConsoleColor.Black, ConsoleColor.White);
            ConsoleScreen.Clear();

            MapGenerator mapGenerator = new MapGenerator(_random, MAP_WIDTH, MAP_HEIGHT);
            _map = mapGenerator.GenerateMap();
            RunGame();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            // Check if we won.
            if (_map.Enemies.Count == 0)
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
                if (_map.Enemies.Count == 0)
                {
                    isRunning = false;
                }
            }
        }

        private static void DisplayGame()
        {
            // Display the map.
            for (int y = 0; y < _map.Height; ++y)
            {
                for (int x = 0; x < _map.Width; ++x)
                {
                    Tile tile = _map.Tiles[x, y];
                    switch (tile)
                    {
                        case Tile.HorizontalWall:
                            ConsoleScreen.Write(x, y, '-');
                            break;
                        case Tile.VerticalWall:
                            ConsoleScreen.Write(x, y, '|');
                            break;
                        case Tile.Door:
                            ConsoleScreen.Write(x, y, '+', ConsoleColor.Green, ConsoleColor.Black);
                            break;
                        case Tile.Floor:
                        default:
                            ConsoleScreen.Write(x, y, ' ');
                            break;
                    }
                }
            }
                
            // Display the player.
            ConsoleScreen.Write(_player.X, _player.Y, '@', ConsoleColor.DarkCyan, ConsoleColor.Black);

            // Display the enemies.
            foreach (Enemy enemy in _map.Enemies)
            {
                ConsoleScreen.Write(enemy.X, enemy.Y, 'e', ConsoleColor.Red, ConsoleColor.Black);
            }

            // Draw to the console screen.
            ConsoleScreen.Draw();
        }

        private static void ProcessInput(ConsoleKey input)
        {
            // Move the player.
            switch (input)
            {
                case ConsoleKey.LeftArrow:
                    if (_player.X > 0
                        && _map.Tiles[_player.X - 1, _player.Y] != Tile.HorizontalWall
                        && _map.Tiles[_player.X - 1, _player.Y] != Tile.VerticalWall)
                    {
                        --_player.X;
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (_player.X + 1 < MAP_WIDTH
                        && _map.Tiles[_player.X + 1, _player.Y] != Tile.HorizontalWall
                        && _map.Tiles[_player.X + 1, _player.Y] != Tile.VerticalWall)
                    {
                        ++_player.X;
                    }

                    break;
                case ConsoleKey.UpArrow:
                    if (_player.Y > 0
                        && _map.Tiles[_player.X, _player.Y - 1] != Tile.HorizontalWall
                        && _map.Tiles[_player.X, _player.Y - 1] != Tile.VerticalWall)
                    {
                        --_player.Y;
                    }

                    break;
                case ConsoleKey.DownArrow:
                    if (_player.Y + 1 < MAP_HEIGHT
                        && _map.Tiles[_player.X, _player.Y + 1] != Tile.HorizontalWall
                        && _map.Tiles[_player.X, _player.Y + 1] != Tile.VerticalWall)
                    {
                        ++_player.Y;
                    }

                    break;
            }

            // Check if we killed any enemies.
            int killedEnemyIndex = -1;
            for (int i = 0; i < _map.Enemies.Count; ++i)
            {
                Enemy enemy = _map.Enemies[i];
                if (_player.X == enemy.X
                    && _player.Y == enemy.Y)
                {
                    killedEnemyIndex = i;
                    break;
                }
            }

            if (killedEnemyIndex != -1)
            {
                _map.Enemies.RemoveAt(killedEnemyIndex);
            }
        }
    }
}
