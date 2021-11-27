using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandory_Install_Sort
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            clsMain newMain = new clsMain();
            newMain.Start();
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Finished, press any key to exit...");
            Console.ReadLine();
        }
    }
}
