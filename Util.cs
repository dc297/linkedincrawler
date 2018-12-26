using System;

namespace LinkedInCrawler{
    public class Util{
        public static string GetInputFromConsole(bool Masked){
            string input = "";

            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    input += key.KeyChar;
                    if(Masked) Console.Write("");
                    else Console.Write(key.KeyChar);
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input.Substring(0, (input.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if(key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("");
                        break;
                    }
                }
            } while (true);
            return input;
        }
    }
}