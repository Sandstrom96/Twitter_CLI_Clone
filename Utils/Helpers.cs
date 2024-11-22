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
                if (sbInput.Length > 0)
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

    public static int? CheckUserInput(int maxValue, string input)
    {
        if (int.TryParse(input, out int choiceValue))
        {
            // Kontrollera om siffran är inom listans längd
            if (choiceValue > 0 && choiceValue <= maxValue)
            {
                return choiceValue;
            }
            else
            {
                ShowErrorMessage("Felaktig inmatning, ange ett giltigt alternativ");
            }
        }
        else
        {
            ShowErrorMessage("Endast siffror tillåtna, försök igen");
        }
        return null;
    }
}