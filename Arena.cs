using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    public enum Food
    {
        None = 0,
        Apple,
        Orange
    }

    public class Arena
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public SnakeModel Snake {get; set;}
        public SnakeModel Snake2;

        public Boolean isTwoP = false;//判断是否为双人对战

        public Food[,] Cells;
        private Random random = new Random();
        public bool stop=false;

        public Arena(int width, int height)
        {
            Width = width;
            Height = height;

            Cells = new Food[width, height];

            Snake = new SnakeModel(this);
        }

        public void Update()
        {
            if (isTwoP)
                Snake.Move(1,Snake2.growth);
            else
                Snake.Move(0,0);
            try
            {
                if(Snake2 != null)
                {
                    //Console.Write("Moving\n");
                    Snake2.Move(2,Snake.growth);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("NOT MOVING");
            }
            if (random.Next(100) <= 4)
            {
                CreateFood();
            }
        }

        public void CreateFood()
        {
            Cells[random.Next(0, Width), random.Next(0, Height)] = (Food)random.Next(1, 3);
        }
        
    }
}
