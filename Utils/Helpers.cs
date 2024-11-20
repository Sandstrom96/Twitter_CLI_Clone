using System.Text;
static class Helpers
{
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
    
    public static string ReadUserInput()
    {
        StringBuilder sbInput = new();
        
        while (true)
        {   
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return null;
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
        return sbInput.ToString();
    }

    public static void ShowErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }
}