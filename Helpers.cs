using System.Text;
static class Helpers
{
    // Some nice helper methods for input:
    public static int ReadInt()
    {
        int input;
        while (!int.TryParse(Console.ReadLine(), out input))
        {
            Console.WriteLine("Ogiltig inmatning, försök igen:");
        }
        return input;
    }

    public static DateTime ReadDate()
    {
        DateTime input;
        while (!DateTime.TryParse(Console.ReadLine(), out input))
        {
            Console.WriteLine("Ogiltigt datumformat, försök igen (åååå-mm-dd):");
        }
        return input;
    }

    public static string ReadString()
    {
        string input;
        while (string.IsNullOrEmpty(input = Console.ReadLine()))
        {
            Console.WriteLine("Ogiltig inmatning, försök igen:");
        }
        return input;
    }
    public static string StringBuilder()
    {
        StringBuilder sbInput = new();
        
        while (true)
        {   
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return "0";
            }
            else if (key.Key == ConsoleKey.Enter)
            { 
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if(sbInput.Length > 0)
                {
                    sbInput.Remove(sbInput.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else
            {
                sbInput.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
        
        string input = sbInput.ToString();
        return input;
    }
}