using System.Text;
public class Message 
{
    public string Text { get; set; }
    public string Sender { get; set; }
    public DateTime Date { get; set; }

    public Message(string text, string sender)
    {
        Text = text; 
        Sender = sender;
        Date = DateTime.Now;
        
    }
}
public class User
{
    public string Username {get; set;}
    public string Password {get; set;}
    public string Name {get; set;}
    public List<User> Followers {get; set;} = new List<User>();
    public List<User> Following {get; set;} = new List<User>();
    public List<Guid> OwnTweets {get; set;} = new List<Guid>();
    public List<Message> Messages {get; set;} = new List<Message>();
    public User(string username, string password, string name)
    {
        Username = username;
        Password = password;
        Name = name;
    }
}

static class UserHandler
{
    static string userName;
    static string password;
    public static User loggedInUser;
    public static List<User> users = new List<User>();
    
    static public void Register()
    {
        Console.WriteLine("Registrering");
        Console.WriteLine("---------------------");
        bool validUser = false;
        while (!validUser)
        {
            Console.Write("Ange användarnamn: ");
            userName = Console.ReadLine();
            
            if(users.Any(x => x.Username == userName))
            {
                Console.WriteLine("Användarnamn upptaget");
            }
            else
            {
                validUser = true;
            }
        }    
        
        Console.Write("Ange lössenord: ");
        
        password = Console.ReadLine();
        Console.Write("Ange förnamn och efternamn: ");
        string name = Console.ReadLine();
        users.Add(new User(userName, password, name));
    }

    static public bool Login()
    {
        bool validUser = false;
        bool userFound = false;

        while (!validUser)
        {            
            if(!userFound)
            {
                Console.Write("Ange användarnamn: ");
                userName = Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"Ange Användarnamn: {userName}");
            }
            Console.Write("Ange lössenord: ");
            password = Console.ReadLine();

            for (int i = 0; i < users.Count; i++)
            {
                if (userName.Equals(users[i].Username)) 
                {
                    userFound = true;
                    if (password.Equals(users[i].Password))
                    {
                        loggedInUser = users[i];
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Du har angett fel lössenord!");
                        break;
                    }
                }
            }

            if(!userFound)
            {
                Console.WriteLine("Kan inte hitta ett konto med det användarnamn");
            }
        
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
    }
    //Visar profilen enligt indatan tex. den inloggade eller sökta profilen
    public static void ShowUserProfile(string username)
    {           
        User chosenUser = users.FirstOrDefault(u => u.Username == username);

        var currentMode = ViewMode.Normal;
        
        while (true)
        {
            string followUnfollow = DynamicButtonhandler.FollowButton(username);
            
            var userTweets = TweetHandler.tweets.Where(t => chosenUser.OwnTweets.Contains(t.Id)).ToList();
            
            Console.Clear();
            Console.WriteLine(chosenUser.Name);
            Console.WriteLine($"@{chosenUser.Username}");
            Console.WriteLine($"Följare {chosenUser.Followers.Count}\tFöljer {chosenUser.Following.Count}");
            Console.WriteLine("----------------------");
            
            switch(currentMode)
            {
                case ViewMode.Normal:
                    TweetHandler.ShowTweets(userTweets, false);
                    
                    if (chosenUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine("1. Följer  2. Följare  3. Meddelanden 4. Gillade 5. Radera tweet 6. Hem");
                    }
                    else
                    {
                        Console.WriteLine($"1. {followUnfollow} 2. Skicka meddelande 3. Följer  4. Följare 5. Gillade 6. Hem");
                    }
                    break;
                
                case ViewMode.LikedTweets:
                    Console.WriteLine("Gillade");
                    Console.WriteLine("----------------------");
                    TweetHandler.ShowLikedTweets(chosenUser.Username);
                    if (chosenUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine("1. Följer  2. Följare  3. Meddelanden 4. Profil 5. Radera tweet 6. Hem");
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
                    Console.WriteLine("----------------------");
                    ShowFollow(username, currentMode);
                    if (chosenUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine("1. Följer  2. Profil  3. Meddelanden 4. Gillade 5. Radera tweet 6. Hem");
                    }
                    else
                    {
                        Console.WriteLine($"1. {followUnfollow} 2. Skicka meddelande 3. Följer  4. Profil 5. Gillade 6. Hem");
                    }
                    break;
                
                case ViewMode.Following:
                    Console.WriteLine("Följer");
                    Console.WriteLine("----------------------");
                    ShowFollow(username, currentMode);
                    if (chosenUser.Username == loggedInUser.Username)
                    {
                        Console.WriteLine("1. Profil  2. Följare  3. Meddelanden 4. Gillade 5. Radera tweet 6. Hem");
                    }
                    else
                    {
                        Console.WriteLine($"1. {followUnfollow} 2. Skicka meddelande 3. Profil  4. Följare 5. Gillade 6. Hem");
                    }
                    break;
            }

            if(chosenUser.Username == loggedInUser.Username)
            {
                var choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.D1:
                        currentMode = currentMode == ViewMode.Following ? ViewMode.Normal : ViewMode.Following;
                        break;

                    case ConsoleKey.D2:
                        currentMode = currentMode == ViewMode.Followers ? ViewMode.Normal : ViewMode.Followers;
                        break;
                    
                    case ConsoleKey.D3:
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
                        SendMessage(chosenUser); 
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
        StringBuilder sbSearch = new();
        Console.WriteLine("Tryck esc för att gå tillbaka"); 
        Console.Write("Sök: ");
        
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
                    if(sbSearch.Length > 0)
                    {
                        sbSearch.Remove(sbSearch.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    sbSearch.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
        }
        string search = sbSearch.ToString();

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
        Console.Clear();
        ShowMessages(receiver);
        Console.WriteLine("Tryck esc för att avbryta");
        Console.WriteLine("Skriv ditt meddelande:");
        StringBuilder sbMessage = new();
        
        while (true)
        {   
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return;
            }
            else if (key.Key == ConsoleKey.Enter)
            { 
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if(sbMessage.Length > 0)
                {
                    sbMessage.Remove(sbMessage.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else
            {
                sbMessage.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
        
        string messageContent = sbMessage.ToString();

        var message = new Message(messageContent, UserHandler.loggedInUser.Username);
            
        receiver.Messages.Add(message);
    }
    public static void ShowMessages(User receiver)
    {
        foreach(var m in receiver.Messages)
        {
            Console.WriteLine($"Från:{m.Sender}");
            Console.WriteLine(m.Text);
            Console.WriteLine($"{m.Date:MM-dd HH:mm}"); 
        }
    }
}