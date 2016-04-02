using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace postgreconnection
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> MyLocalInventory = new List<string>();

            MyLocalInventory = Inventory.GetLocalInventory();
            MyLocalInventory.Sort();

            foreach(string x in MyLocalInventory)
            {
                Console.WriteLine(x);
            }
            Console.ReadLine();
        }
    }
}
