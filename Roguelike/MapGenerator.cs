using System;
using System.Drawing;

namespace Roguelike
{
    public class MapGenerator
    {
        private const int MIN_ROOMS = 6;
        private const int MAX_ROOMS = 8;
        private const int MIN_ROOM_WIDTH = 8;
        private const int MAX_ROOM_WIDTH = 10;
        private const int MIN_ROOM_HEIGHT = 6;
        private const int MAX_ROOM_HEIGHT = 8;

        private const int MAX_ATTEMPTS = 10;

        private Random _random;
        private Map _map;
        private int _mapWidth;
        private int _mapHeight;

        public MapGenerator(Random random, int width, int height)
        {
            _random = random;
            _mapWidth = width;
            _mapHeight = height;
            _map = new Map(_mapWidth, _mapHeight);
        }

        public Map GenerateMap()
        {
            GenerateRooms();
            GenerateEnemies();
            return _map;
        }

        private void GenerateRooms()
        {
            int numRooms = _random.Next(MIN_ROOMS, MAX_ROOMS);
            for (int i = 0; i < numRooms; ++i)
            {
                CreateRoom();
            }
        }

        private void CreateRoom()
        {
            int attempts = 0;
            while (attempts < MAX_ATTEMPTS)
            {
                Rectangle room = new Rectangle(
                    _random.Next(_mapWidth - 1),
                    _random.Next(_mapHeight - 1),
                    _random.Next(MIN_ROOM_WIDTH, MAX_ROOM_WIDTH),
                    _random.Next(MIN_ROOM_HEIGHT, MAX_ROOM_HEIGHT));

                if (room.Right >= _mapWidth
                    || room.Bottom >= _mapHeight)
                {
                    ++attempts;
                    continue;
                }

                bool intersects = false;
                foreach (Rectangle existingRoom in _map.Rooms)
                {
                    if (existingRoom.IntersectsWith(room))
                    {
                        intersects = true;
                        break;
                    }
                }

                if (intersects)
                {
                    ++attempts;
                    continue;
                }

                CarveRoom(room);
                _map.Rooms.Add(room);
                break;
            }
        }

        private void CarveRoom(Rectangle room)
        {
            // Carve the room into the map data.
            for (int x = room.Left; x < room.Right; ++x)
            {
                for (int y = room.Top; y < room.Bottom; ++y)
                {
                    if (y == room.Top || y == room.Bottom - 1)
                    {
                        // The top and bottom wall, set a horizontal wall tile.
                        _map.Tiles[x, y] = Tile.HorizontalWall;
                    }
                    else if (x == room.Left || x == room.Right - 1)
                    {
                        // The left and right wall, set a vertical wall tile.
                        _map.Tiles[x, y] = Tile.VerticalWall;
                    }
                    else
                    {
                        // Set a floor tile.
                        _map.Tiles[x, y] = Tile.Floor;
                    }
                }
            }

            // Put a door on the room.
            int side = _random.Next(4);
            int doorX = 0;
            int doorY = 0;
            switch (side)
            {
                case 0:
                    // Left side wall
                    doorX = room.Left;
                    doorY = room.Top + (room.Height / 2);
                    break;
                case 1:
                    // Right side wall
                    doorX = room.Right - 1;
                    doorY = room.Top + (room.Height / 2);
                    break;
                case 2:
                    // Top wall
                    doorX = room.Right - (room.Width / 2);
                    doorY = room.Top;
                    break;
                case 3:
                    // Bottom wall
                    doorX = room.Right - (room.Width / 2);
                    doorY = room.Bottom - 1;
                    break;
            }

            _map.Tiles[doorX, doorY] = Tile.Door;
        }

        private void GenerateEnemies()
        {
            int enemyNumber = _random.Next(10, 20);
            for (int i = 0; i < enemyNumber; ++i)
            {
                Enemy enemy = new Enemy()
                {
                    X = _random.Next(_mapWidth - 1),
                    Y = _random.Next(_mapHeight - 1)
                };

                if (_map.Tiles[enemy.X, enemy.Y] == Tile.Floor)
                {
                    _map.Enemies.Add(enemy);
                }
            }
        }
    }
}
