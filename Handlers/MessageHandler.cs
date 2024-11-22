class MessageHandler
{
    // Itererar igenom den inloggade användarens meddelanden
    // och kollar vilka användaren har konversationer med 
    // och skriver ut användarna
    public static List<string> GetConversations()
    {
        List<string> users = new List<string>();

        foreach (var user in UserCLI.loggedInUser.Messages)
        {
            string test = user.Sender == UserCLI.loggedInUser.Username ? user.Receiver : user.Sender;
            if (!users.Contains(test))
                users.Add(test);
        }
        return users;
    }

    public static List<Message> GetUnreadMessages()
    {
        return UserCLI.loggedInUser.Messages.Where(m => m.Isread == false && m.Receiver == UserCLI.loggedInUser.Username).ToList();
    }

    public static void SetMessageToRead(User user)
    {
        // Hämtar den inloggade användarens mottagna meddelanden från den hämtade användaren (user) som är olästa
        // och lägger i en lista
        var messages = UserCLI.loggedInUser.Messages.Where(m => m.Isread == false && m.Receiver == UserCLI.loggedInUser.Username && m.Sender == user.Username).ToList();

        // Hämtar den hämtade användarens (user) skickade meddelanden till den inloggade användaren som är olästa
        // och lägger till i samma lista
        messages.AddRange(user.Messages.Where(m => m.Isread == false && m.Receiver == UserCLI.loggedInUser.Username && m.Sender == user.Username).ToList());

        // Sätter meddelandena till lästa
        foreach (var m in messages)
        {
            m.Isread = true;
        }
    }

    // Hämtar alla olästa meddelanden så man kan visa hur många olästa man har
    public static List<Message> UnreadConversations(string user)
    {
        return UserCLI.loggedInUser.Messages.Where(m => m.Isread == false && m.Receiver == UserCLI.loggedInUser.Username && m.Sender == user).ToList();
    }

    // Hämtar konversationen som användaren väljer med index
    public static User GetConversation(int index, List<string> conversations)
    {
        return UserHandler.users.FirstOrDefault(u => u.Username == conversations[index]);
    }

    public static void AddMessage(string messageContent, User receiver)
    {
        var message = new Message(messageContent, receiver.Username, UserCLI.loggedInUser.Username);

        receiver.Messages.Add(message);
        UserCLI.loggedInUser.Messages.Add(message);
    }

    // Hämtar alla meddelanden mellan 2 användare
    public static List<Message> GetMessagesBetweenUsers(User user)
    {
        return UserCLI.loggedInUser.Messages.Where(m => (m.Sender == user.Username && m.Receiver == UserCLI.loggedInUser.Username) ||
            (m.Receiver == user.Username && m.Sender == UserCLI.loggedInUser.Username)).OrderBy(m => m.Date).ToList();
    }

    public static List<Message> GetSentMessages(User user)
    {
        return UserCLI.loggedInUser.Messages.Where(m => m.Receiver == user.Username && m.Sender == UserCLI.loggedInUser.Username).OrderBy(m => m.Date).ToList();
    }

    public static void RemoveMessage(Message message, User user)
    {
        UserCLI.loggedInUser.Messages.Remove(message);
        user.Messages.Remove(message);
    }
}