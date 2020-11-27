using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net.core.hangfire.demo
{
    public interface IPrintJob
    {
        void Print();
    }
    public class PrintJob : IPrintJob
    {
        public void Print()
        {
            Console.WriteLine($"Hanfire recurring job!");
        }
    }
}
