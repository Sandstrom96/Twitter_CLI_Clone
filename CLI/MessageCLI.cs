class MessageCLI
{
    public static void SendMessage(User receiver)
    {
        Console.WriteLine("Skriv ditt meddelande:");
        
        var messageContent = Helpers.ReadUserInput();

        var message = new Message(messageContent, receiver.Username, UserCLI.loggedInUser.Username);
            
        receiver.Messages.Add(message);
        UserCLI.loggedInUser.Messages.Add(message);
    }

    public static void ShowMessages(User user, bool remove)
    {
        Console.WriteLine($"@{user.Username}");
        Console.WriteLine("---------------------");
        
        var messages = UserCLI.loggedInUser.Messages.Where(m => (m.Sender == user.Username && m.Receiver == UserCLI.loggedInUser.Username) || (m.Receiver == user.Username && m.Sender == UserCLI.loggedInUser.Username)).OrderBy(m => m.Date).ToList();
        var ownMessages = UserCLI.loggedInUser.Messages.Where(m => m.Receiver == user.Username && m.Sender == UserCLI.loggedInUser.Username).OrderBy(m => m.Date).ToList();
        
        foreach(var m in messages)
        {   

            if(remove && m.Sender == UserCLI.loggedInUser.Username)
            {
                var i = ownMessages.IndexOf(m);
                Console.Write($"{i + 1} ");
            }
            
            Console.WriteLine($"Från: {m.Sender}");
            Console.WriteLine(m.Text);
            Console.WriteLine($"{m.Date:MM-dd HH:mm}");
            Console.WriteLine("---------------------");
            
        }
        MessageHandler.SetMessageToRead(user);
    }

    public static void RemoveMessage(User user)
    {
        string choice;
        var messages = UserCLI.loggedInUser.Messages.Where(m => m.Receiver == user.Username && m.Sender == UserCLI.loggedInUser.Username).OrderBy(m => m.Date).ToList();
        
        while (true)
        {   
            ShowMessages(user,true); 
            Console.WriteLine("Tryck esc för att gå tillbaka");
            Console.WriteLine($"Välj vilken du vill radera (1-{messages.Count})");
            
            choice = Helpers.ReadUserInput();

            if (choice.All(char.IsDigit))
            {
                int choiceValue = int.Parse(choice);

                // Kontrollera om siffran är inom listans längd
                if (choiceValue >= 0 && choiceValue <= UserCLI.loggedInUser.Messages.Count)
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fel inmatning, försök igen!");
                    Console.ForegroundColor = ConsoleColor.White;
                    choice = null;
                }
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Endast siffror är tillåtna, försök igen!");
                Console.ForegroundColor = ConsoleColor.White;
                choice = null;
            }
        }
        
        string index = choice;

        var chosenMessage = messages[int.Parse(index) - 1];
        
        Console.WriteLine("Du vill ta bort meddelandet:");
        Console.WriteLine(chosenMessage.Sender);
        Console.WriteLine(chosenMessage.Text);
        Console.WriteLine(chosenMessage.Date);
        
        Console.WriteLine("1. Radera 2. Avbryt");
        var input = Console.ReadKey(true).Key;

        if (input == ConsoleKey.D1)
        {
            if (index == "")
            {
                return;
            }
            else
            {
                UserCLI.loggedInUser.Messages.Remove(chosenMessage);
                user.Messages.Remove(chosenMessage);
 
                Console.WriteLine($"Meddelandet togs bort");
            }
        }
    }
}