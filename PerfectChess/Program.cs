using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectChess
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 F = new Form1();
            //Presenter P = new Presenter(F); 
            Present P = new Present(F, new HumanPlayer(), new EnginePlayer(), new Position());
            Application.Run(F);
        }
    }
}
