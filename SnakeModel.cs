using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using LightBuzz.Vitruvius;
namespace SnakeGame
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    /// <summary>
    /// Move
    /// </summary>
    public class SnakeModel
    {
        public LinkedList<Point> Body = new LinkedList<Point>();
        
        private String direction;
        private Arena arena;

        public int growth = 2;

        Point speed = new Point();

        public SnakeModel(Arena arena)
        {
            Body.AddLast(new Point(1, 1));

            this.arena = arena;
            ChangeDirection("Right");
        }
        public SnakeModel(Arena arena,int x)
        {
            Body.AddLast(new Point(20, 20));
            this.arena = arena;
            ChangeDirection("Left");
        }
        /// <summary>
        /// move
        /// </summary>
        public void Move(int x,int growth2)
        {
            Point newHead = GetNewHead();
            if (arena.Cells[newHead.X, newHead.Y] == Food.Apple)
            {
                growth += 2;
            }
            else if (arena.Cells[newHead.X, newHead.Y] == Food.Orange)
            {
                growth++;
            }
            arena.Cells[newHead.X, newHead.Y] = Food.None;

            if (growth > 0)
            {
                growth--;
            }
            else
            {
                Body.RemoveFirst();
            }
            if (x == 0)
            {
                if (JudgeLifeSingle(newHead))
                    Body.AddLast(newHead);
            }
            else
            {
                if (JudgeLifeDouble(newHead,x,growth2))
                    Body.AddLast(newHead);
            }

        }
        /// <summary>
        /// Judge whether the snake had bitten himself leading to his death
        /// </summary>
        /// <param name="newHead"></param>
        public Boolean JudgeLifeSingle(Point newHead)
        {
            foreach(Point p in Body)
            {
                if (p.X == newHead.X && p.Y == newHead.Y)
                {
                    arena.stop = true;
                    GameOver();
                    return false;
                    break;
                }
            }
            return true;
        }
        /// <summary>
        /// Judge whether the snake had bitten himself leading to his death
        /// </summary>
        /// <param name="newHead"></param>
        public Boolean JudgeLifeDouble(Point newHead,int x, int growth2)
        {
            LinkedList<Point> B;
            if (x == 1)
                B = arena.Snake2.Body;//对手的身体 如果这个身体是1 另外一个人就是2
            else
                B = arena.Snake.Body;
            foreach (Point p in B)
            {
                if (p.X == newHead.X && p.Y == newHead.Y)
                {
                    arena.stop = true;
                    GameOver("Snake"+x,growth2);//自己的头撞到别人的身子 输 自己碰自己没事
                    return false;
                    break;
                }
            }
            return true;
        }
        /// <summary>
        /// Reaction to GameOver单人模式
        /// </summary>
        public void GameOver()
        {
            DialogResult result = MessageBox.Show("GameOver!!!!\n Your Score is "+growth+"\nWould you want to try again?", "Restart", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                //Reset every param
                Body.Clear();
                Body.AddLast(new Point(1, 1));
                growth = 2;
                ChangeDirection("Right");
                for (int x = 0; x < arena.Width; x++)
                {
                    for (int y = 0; y < arena.Height; y++)
                    {
                        arena.Cells[x, y] = Food.None;
                    }
                }
                arena.stop = false;
            }
            else
                Application.Exit();
        }
        /// <summary>
        ///  2p的GameOver
        /// </summary>
        /// <param name="name"></param>
        public void GameOver(string name,int growth2)
        {
            DialogResult result = MessageBox.Show(name + " Lost!!!!\n Your Score is "+ growth +"!!!!! \nWould you want to try again?", "Restart", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                //Reset every param
                Body.Clear(); //只有可能自己撞别人死 所以只需重置自己的身体和位置即可
                Body.AddLast(new Point(1, 1));
                growth = 2;
                ChangeDirection("Right");
                for (int x = 0; x < arena.Width; x++)
                {
                    for (int y = 0; y < arena.Height; y++)
                    {
                        arena.Cells[x, y] = Food.None;
                    }
                }
                arena.stop = false;
            }
            else
            {
                if(growth > growth2)//name 代表自己
                {
                    MessageBox.Show("Congrads!! Snake" + name + " Won!!!");
                }
                else
                {
                    MessageBox.Show("I'm Sorry!! Snake" + name + " Lost!!!");
                }
                Application.Exit();
            }
               
        }

        public void ChangeDirection(String type)
        {
            switch (type)
            {
                case "Down":
                    if (direction != "Up" && direction != "Down")
                    {
                        speed.X = 0;
                        speed.Y = 1;
                        direction = type;
                    }
                    break;

                case "Up":
                    if (direction != "Down" && direction != "Up")
                    {
                        speed.X = 0;
                        speed.Y = -1;
                        direction = type;
                    }
                    break;

                case "Left":
                    if (direction != "Right" && direction != "Left")
                    {
                        speed.X = -1;
                        speed.Y = 0;
                        direction = type;
                    }
                    break;

                case "Right":
                    if (direction != "Left" && direction != "Right")
                    {
                        speed.X = 1;
                        speed.Y = 0;
                        direction = type;
                    }
                    break;
            }
        }

        private Point GetNewHead()
        {
            Point head = Body.Last.Value;
            //处理穿墙的情况
            int newX = (head.X + speed.X) % arena.Width;
            if (newX < 0)
            {
                newX += arena.Width;
            }

            int newY = (head.Y + speed.Y) % arena.Height;
            if (newY < 0)
            {
                newY += arena.Height;
            }

            Point newHead = new Point(newX, newY);
            return newHead;
        }
    }
}
