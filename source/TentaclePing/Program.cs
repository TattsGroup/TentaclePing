using System;
using CLAP;

namespace TentaclePing
{
    static class Program
    {
        static void Main(string[] args)
        {
            var p = new Parser<PingApp>();

            p.Register.EmptyHelpHandler(help => Console.WriteLine(help));
            p.Register.HelpHandler("?,h,help", help => Console.WriteLine(help));
            p.Register.ErrorHandler(e => Console.WriteLine(p.GetHelpString()));

            p.Run(args, new PingApp()); ;
        }
    }
}
