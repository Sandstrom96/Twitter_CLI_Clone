using System.Xml;

class UserCLI
{
    public static void Register()
    {
        Console.WriteLine("--- Registrering ---");
        Console.WriteLine("Tryck esc för att avbryta");

        string username;
        while (true)
        {
            Console.Write("Ange användarnamn: ");
            username = Helpers.ReadUserInput();
            
            if (username == null)
            {
                Console.Clear();
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Användarnamn får inte vara tomt.");
                Console.ForegroundColor = ConsoleColor.White;
                continue;
            }

            username = UserHandler.CheckUsernameForWhitespaces(username);

            if(!UserHandler.IsUsernameAvailable(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Användarnamn upptaget.");
                Console.ForegroundColor = ConsoleColor.White;
                continue;
            }
            break;
        }

        string password;  
        while (true)    
        {
            Console.Write("Ange lösenord: ");
            password = Helpers.ReadUserInput();

            if (password == null)
            {
                Console.Clear();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lösenordet får inte vara tomt.");
                Console.ForegroundColor = ConsoleColor.White;
                continue;
            }

            Console.Write("Bekräfta lösenordet: ");
            string confirmPassword = Helpers.ReadUserInput();

            if (confirmPassword == null)
            {
                Console.Clear();
                return;
            }

            if (password != confirmPassword)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lösenorden matchar inte. Försök igen!");
                Console.ForegroundColor = ConsoleColor.White;
                continue;
            }
            break;
        }

        string name;
        while (true)
        {
            Console.Write("Ange förnamn och efternamn: ");
            name = Helpers.ReadUserInput();

            if (name == null)
            {
                Console.Clear();
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Namn får inte vara tomt.");
                Console.ForegroundColor = ConsoleColor.White;
                continue;
            }
            break;
        }
        UserHandler.AddNewUser(username, password, name);
    }
    public static bool LogIn()
    {
        bool validUser = false;
        
        while (!validUser)
        {
            Console.WriteLine("----- Logga in -----");
            Console.WriteLine("Tryck esc för att avbryta");
            
            string username;
            string password;
            while (true)
            {
                Console.Write("Ange användarnamn: ");
                username = Helpers.ReadUserInput();

                if (username == null)
                {
                    Console.Clear();
                    return validUser;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Användarnamn får inte vara tomt.");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                Console.Write("Ange Lössenord: ");
                password = Helpers.ReadUserInput();

                if (password == null)
                {
                    Console.Clear();
                    return validUser;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Lösenordet får inte vara tomt.");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                if (!UserHandler.validUsername(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Kan inte hitta ett konto med det användarnamn");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                if (!UserHandler.validPassword(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Du har angett fel lösenord!");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
            
                validUser = true;
                break;
            }
        }
        
        return validUser;
    }
}