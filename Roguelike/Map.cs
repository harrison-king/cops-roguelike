using System;
using System.Collections.Generic;
using System.Drawing;

namespace Roguelike
{
    public enum Tile
    {
        Floor,
        VerticalWall,
        HorizontalWall,
        Door
    }

    public class Map
    {
        public Map(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Tiles = new Tile[this.Width, this.Height];
            this.Rooms = new List<Rectangle>();
            this.Enemies = new List<Enemy>();
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public Tile[,] Tiles
        {
            get;
            set;
        }

        public List<Rectangle> Rooms
        {
            get;
            set;
        }

        public List<Enemy> Enemies
        {
            get;
            set;
        }
    }
}
