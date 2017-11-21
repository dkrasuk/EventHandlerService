
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHandler.BusinessLayer.Staging;

namespace ConsoleApplicationTest
{
    class Program
    {
        static void Main(string[] args)
        {

            CollateralSync.StartSyncCollateral();
        }
    }
}
