using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace SnakeGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread thread1 = new Thread(new ThreadStart(FF));
            thread1.Start();
            Thread thread2 = new Thread(new ThreadStart(FF2));
            thread2.Start();
        }
        static void FF()
        {
            Application.Run(new SnakeMainForm());
        }
        static void FF2()
        {
            Application.Run(new Form1());
        }
    }
}
