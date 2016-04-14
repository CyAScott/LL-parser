using System;

namespace LLParser.ConsoleApp
{
    public static partial class Program
    {
        static Program()
        {
            Console.BufferHeight = 1024;
            Console.BufferWidth = 1024;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Main()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[LL Parser Console App]");
                Console.WriteLine();

                GetInput();


            }
            catch (Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
            }
        }
    }
}
