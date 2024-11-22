class MessageCLI
{
    public static void SendMessage(User receiver)
    {
        Console.WriteLine("Skriv ditt meddelande:");

        var messageContent = Helpers.ReadUserInput();

        if (messageContent == null)
        {
            return;
        }

        MessageHandler.AddMessage(messageContent, receiver);
    }

    public static void ShowMessages(User user, bool remove)
    {
        Console.WriteLine(user.Name);
        Console.WriteLine($"@{user.Username}");
        Console.WriteLine("----------------------");

        var messages = MessageHandler.GetMessagesBetweenUsers(user);
        var ownMessages = MessageHandler.GetSentMessages(user);

        foreach (var m in messages)
        {

            if (remove && m.Sender == UserCLI.loggedInUser.Username)
            {
                var i = ownMessages.IndexOf(m);
                Console.Write($"{i + 1} ");
            }

            Console.WriteLine($"Från: {m.Sender}");
            Console.WriteLine(m.Text);
            Console.WriteLine($"{m.Date:MM-dd HH:mm}");
            Console.WriteLine("----------------------");

        }
        MessageHandler.SetMessageToRead(user);
    }

    public static void RemoveMessage(User user)
    {
        var messages = MessageHandler.GetSentMessages(user);

        if (messages.Count == 0)
        {
            return;
        }
        ShowMessages(user, true);
        Console.WriteLine("\nTryck Esc för att gå tillbaka");
        Console.WriteLine($"Välj vilken du vill radera (1-{messages.Count})");

        int? index;
        while (true)
        {
            string choice = Helpers.ReadUserInput();

            if (choice == null)
            {
                return;
            }

            index = Helpers.CheckUserInput(messages.Count, choice);

            if (index > 0 && index <= messages.Count)
            {
                break;
            }
        }

        var chosenMessage = messages[(int)index - 1];

        Console.WriteLine("Du vill ta bort meddelandet:");
        Console.WriteLine(chosenMessage.Sender);
        Console.WriteLine(chosenMessage.Text);
        Console.WriteLine(chosenMessage.Date);

        Console.WriteLine("\nTryck Enter för att radera");
        Console.WriteLine("Tryck Esc för att avbryta");
        while (true)
        {
            var input = Console.ReadKey(true).Key;

            switch (input)
            {
                case ConsoleKey.Enter:
                    MessageHandler.RemoveMessage(chosenMessage, user);
                    return;

                case ConsoleKey.Escape:
                    return;

            }
        }
    }
}