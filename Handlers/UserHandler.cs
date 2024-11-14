using System.Text;
static class UserHandler
{
    public static User loggedInUser;
    public static List<User> users = new List<User>();
    
    static public bool IsUsernameAvailable(string username)
    {
        if (username.Contains(" "))
        {
            username.Replace(" ", "");
        }
        
        if(users.Any(x => x.Username.ToLower() == username.ToLower()))
        {
            return false;
        }
        return true;
    }

    static public void AddNewUser(string username, string password, string name)
    {
        users.Add(new User(username, password, name));
    }

    static public bool validLogIn(string username, string password)
    {
        bool userFound = false;
        
        for (int i = 0; i < users.Count; i++)
        {
            if (username.ToLower().Equals(users[i].Username.ToLower())) 
            {
                userFound = true;
                if (password.Equals(users[i].Password))
                {
                    loggedInUser = users[i];
                    return true;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Du har angett fel lössenord!");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
            }
        }

        if(!userFound)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kan inte hitta ett konto med det användarnamn");
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        return false;
    }
    
    public enum ViewMode
    {
        Normal,
        LikedTweets,
        RemoveTweet,
        Followers,
        Following,
        Messages,
        Conversations,
    }
    //Visar profilen enligt indatan tex. den inloggade eller sökta profilen
    public static void ShowUserProfile(string username)
    {           
        User foundUser = users.FirstOrDefault(u => u.Username == username);
        User user = foundUser;

        var currentMode = ViewMode.Normal;
        
        while (true)
        {
            string followUnfollow = DynamicButtonhandler.FollowButton(username);
            
            var userTweets = TweetHandler.tweets.Where(t => foundUser.OwnTweets.Contains(t.Id)).ToList();
            var test = UnreadMessage();
            
            Console.Clear();
            Console.WriteLine(foundUser.Name);
            Console.WriteLine($"@{foundUser.Username}");
            Console.WriteLine($"Följare {foundUser.Followers.Count}\tFöljer {foundUser.Following.Count}");
            Console.WriteLine("---------------------");
            
            switch(currentMode)
            {
                case ViewMode.Normal:
                    TweetHandler.ShowTweets(userTweets, false);
                    
                    if (foundUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"1. Följer  2. Följare  3. Meddelanden ({test.Count}) 4. Gillade 5. Radera tweet 6. Hem");
                    }
                    else
                    {
                        Console.WriteLine($"1. {followUnfollow} 2. Skicka meddelande 3. Följer  4. Följare 5. Gillade 6. Hem");
                    }
                    break;
                
                case ViewMode.LikedTweets:
                    Console.WriteLine("Gillade");
                    Console.WriteLine("---------------------");
                    
                    TweetHandler.ShowLikedTweets(foundUser.Username);
                    
                    if (foundUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"1. Följer  2. Följare  3. Meddelanden ({test.Count}) 4. Profil 5. Radera tweet 6. Hem");
                    }
                    else
                    {
                        Console.WriteLine($"1. {followUnfollow} 2. Skicka meddelande 3. Följer  4. Följare 5. Profil 6. Hem");
                    }
                    break;

                case ViewMode.RemoveTweet:
                    Console.WriteLine("Radera tweet");
                    TweetHandler.RemoveTweet();
                    currentMode = ViewMode.Normal;
                    continue;

                case ViewMode.Followers:
                    Console.WriteLine("Följare");
                    Console.WriteLine("---------------------");
                    
                    ShowFollow(username, currentMode);
                    
                    if (foundUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"1. Följer  2. Profil  3. Meddelanden ({test.Count}) 4. Gillade 5. Radera tweet 6. Hem");
                    }
                    else
                    {
                        Console.WriteLine($"1. {followUnfollow} 2. Skicka meddelande 3. Följer  4. Profil 5. Gillade 6. Hem");
                    }
                    break;
                
                case ViewMode.Following:
                    Console.WriteLine("Följer");
                    Console.WriteLine("---------------------");
                    
                    ShowFollow(username, currentMode);
                    
                    if (foundUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"1. Profil  2. Följare  3. Meddelanden ({test.Count}) 4. Gillade 5. Radera tweet 6. Hem");
                    }
                    else
                    {
                        Console.WriteLine($"1. {followUnfollow} 2. Skicka meddelande 3. Profil  4. Följare 5. Gillade 6. Hem");
                    }
                    break;
                
                case ViewMode.Conversations:
                    var conversations = Conversations();
                    
                    for (int i = 0; i < conversations.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {conversations[i]} ({UnreadConversations(conversations[i]).Count})");
                    }
                    
                    Console.WriteLine("\nVälj vilken konversation du vill öppna");
                    Console.WriteLine("Tryck esc för att gå tillbaka");
                    
                    var index = int.Parse(Helpers.ReadUserInput()) - 1;
                    
                    if (index == -1)
                    {
                        currentMode = ViewMode.Normal;
                        continue;
                    }
                    
                    user = users.FirstOrDefault(u => u.Username == conversations[index]);
                    currentMode = ViewMode.Messages;
                    continue;
                
                case ViewMode.Messages:
                    Console.Clear();
                    Console.WriteLine("-----Meddelanden-----");
                    
                    ShowMessages(user, false);
                    
                    Console.WriteLine("\n1. Skriv meddelande 2. Ta bort meddelande");
                    Console.WriteLine("Tryck esc för att gå tillbaka");
                    
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.D1:
                            SendMessage(user);
                            continue;
                        
                        case ConsoleKey.D2:
                            RemoveMessage(user); 
                            continue;
                        
                        case ConsoleKey.Escape:
                            currentMode = ViewMode.Normal;
                            continue;
                    }
                    break;
            }

            if(foundUser.Username == loggedInUser.Username)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        currentMode = currentMode == ViewMode.Following ? ViewMode.Normal : ViewMode.Following;
                        break;

                    case ConsoleKey.D2:
                        currentMode = currentMode == ViewMode.Followers ? ViewMode.Normal : ViewMode.Followers;
                        break;
                    
                    case ConsoleKey.D3:
                        currentMode = ViewMode.Conversations;
                        break;

                    case ConsoleKey.D4:
                        currentMode = currentMode == ViewMode.LikedTweets ? ViewMode.Normal : ViewMode.LikedTweets;
                        break;
                    
                    case ConsoleKey.D5:
                        currentMode = ViewMode.RemoveTweet;
                        break;

                    case ConsoleKey.D6:
                    return;
                }
            }
            // Visar den sökta profilen
            else
            {
                var choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.D1:
                        FollowUnfollow(username);
                        break;

                    case ConsoleKey.D2:
                        currentMode = ViewMode.Messages;
                        break;

                    case ConsoleKey.D3:
                        currentMode = currentMode == ViewMode.Following ? ViewMode.Normal : ViewMode.Following;
                        break;

                    case ConsoleKey.D4:
                        currentMode = currentMode == ViewMode.Followers ? ViewMode.Normal : ViewMode.Followers;
                        break;

                    case ConsoleKey.D5:
                        currentMode = currentMode == ViewMode.LikedTweets ? ViewMode.Normal : ViewMode.LikedTweets;
                        break;

                    case ConsoleKey.D6:
                        return;
                }
            }
        }
    }
    public static string SearchProfile()
    {
        Console.WriteLine("Tryck esc för att gå tillbaka"); 
        Console.Write("Sök: ");
        
        var search = Helpers.ReadUserInput();

        var userName = users.FirstOrDefault(u => u.Username == search);
        if (userName.Username != search)
        {
            Console.WriteLine("Kan inte hitta användaren.");
        }
        return userName.Username;
    }

    public static void FollowUnfollow(string username)
    {
        User chosenUser = users.FirstOrDefault(u => u.Username == username);
        
        if (!chosenUser.Followers.Any(u => u.Username == loggedInUser.Username))
        {
            chosenUser.Followers.Add(loggedInUser);
            loggedInUser.Following.Add(chosenUser);
        }
        else
        {
            chosenUser.Followers.Remove(loggedInUser);
            loggedInUser.Following.Remove(chosenUser);
        }
        
    }

    // Tar in användare och nuvarande visar läge i profilen
    // och visar antingen användarens följare eller vilka de följer
    public static void ShowFollow(string username, ViewMode currentMode)
    {
        User userFound = users.FirstOrDefault(u => u.Username == username);

        switch (currentMode)
        {
            case ViewMode.Followers:
                for (int i = 0; i < userFound.Followers.Count; i++)
                {
                    Console.WriteLine($"{userFound.Followers[i].Name} | @{userFound.Followers[i].Username}");
                }
                break;
        
            case ViewMode.Following:
                for (int i = 0; i < userFound.Following.Count; i++)
                {
                    Console.WriteLine($"{userFound.Following[i].Name} | @{userFound.Following[i].Username}");
                }
                break;
        }
    }

    public static void SendMessage(User receiver)
    {
        Console.WriteLine("Skriv ditt meddelande:");
        
        var messageContent = Helpers.ReadUserInput();

        var message = new Message(messageContent, receiver.Username, loggedInUser.Username);
            
        receiver.Messages.Add(message);
        loggedInUser.Messages.Add(message);
    }

    public static void ShowMessages(User user, bool remove)
    {
        Console.WriteLine($"@{user.Username}");
        Console.WriteLine("---------------------");
        
        var messages = loggedInUser.Messages.Where(m => (m.Sender == user.Username && m.Receiver == loggedInUser.Username) || (m.Receiver == user.Username && m.Sender == loggedInUser.Username)).OrderBy(m => m.Date).ToList();
        var ownMessages = loggedInUser.Messages.Where(m => m.Receiver == user.Username && m.Sender == loggedInUser.Username).OrderBy(m => m.Date).ToList();
        
        foreach(var m in messages)
        {   

            if(remove && m.Sender == loggedInUser.Username)
            {
                var i = ownMessages.IndexOf(m);
                Console.Write($"{i + 1} ");
            }
            
            Console.WriteLine($"Från: {m.Sender}");
            Console.WriteLine(m.Text);
            Console.WriteLine($"{m.Date:MM-dd HH:mm}");
            Console.WriteLine("---------------------");
            
        }
        SetMessageToRead(user);
    }
public static void RemoveMessage(User user)
    {
        string choice;
        var messages = loggedInUser.Messages.Where(m => m.Receiver == user.Username && m.Sender == loggedInUser.Username).OrderBy(m => m.Date).ToList();
        
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
                if (choiceValue >= 0 && choiceValue <= loggedInUser.Messages.Count)
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
                loggedInUser.Messages.Remove(chosenMessage);
                user.Messages.Remove(chosenMessage);
 
                Console.WriteLine($"Meddelandet togs bort");
            }
        }
    }
    
    // Itererar igenom den inloggade användarens meddelanden
    // och kollar vilka användaren har konversationer med 
    // och skriver ut användarna
    public static List<string> Conversations()
    {
        List<string> users = new List<string>();

        if(loggedInUser.Messages.Count <= 0)
        {
            Console.WriteLine("Här var det tomt");
        }
        else
        {
            foreach (var user in loggedInUser.Messages)
            {
                string test = user.Sender == loggedInUser.Username ? user.Receiver : user.Sender;
                if(!users.Contains(test))
                    users.Add(test);
            }
        }
        return users;
    }

    public static List<Message> UnreadMessage()
    {
        return loggedInUser.Messages.Where(m => m.Isread == false && m.Receiver == loggedInUser.Username).ToList();
    }
    
    public static void SetMessageToRead(User user)
    {
        var messages = loggedInUser.Messages.Where(m => m.Isread == false && m.Receiver == loggedInUser.Username && m.Sender == user.Username).ToList();
        messages.AddRange(user.Messages.Where(m => m.Isread == false && m.Receiver == loggedInUser.Username && m.Sender == user.Username).ToList());

        foreach(var m in messages)
        {
            m.Isread = true;
        }
    }

    public static List<Message> UnreadConversations (string user)
    {
        return loggedInUser.Messages.Where(m => m.Isread == false && m.Receiver == loggedInUser.Username && m.Sender == user).ToList();
    }
    
}