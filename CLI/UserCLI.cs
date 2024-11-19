using System.Xml;

class UserCLI
{
    public static User loggedInUser;
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ditt användarnamn kan inte innehålla mellanslag.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Detta är nu ditt användarnamn: {username}");

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
    public static void ShowUserProfile(User username)
    {           
        User user = foundUser;
        User foundUser = UserHandler.GetUser(username);

        var currentMode = ViewMode.Normal;
        
        while (true)
        {
            string followUnfollow = DynamicButtonhandler.FollowButton(username);
            
            var userTweets = TweetHandler.GetUserTweets(username);
            var unreadMessages = MessageHandler.UnreadMessage();
            
            Console.Clear();
            Console.WriteLine(foundUser.Name);
            Console.WriteLine($"@{foundUser.Username}");
            Console.WriteLine($"Följare {foundUser.Followers.Count}\tFöljer {foundUser.Following.Count}");
            Console.WriteLine("---------------------");
            
            switch(currentMode)
            {
                case ViewMode.Normal:
                    TweetCLI.ShowTweets(userTweets, false);
                    
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
                    
                    TweetCLI.ShowLikedTweets(foundUser.Username);
                    
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
                    TweetCLI.RemoveTweet();
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
                    var conversations = MessageHandler.Conversations();
                    
                    for (int i = 0; i < conversations.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {conversations[i]} ({MessageHandler.UnreadConversations(conversations[i]).Count})");
                    }
                    
                    Console.WriteLine("\nVälj vilken konversation du vill öppna");
                    Console.WriteLine("Tryck esc för att gå tillbaka");
                    
                    var index = int.Parse(Helpers.ReadUserInput()) - 1;
                    
                    if (index == -1)
                    {
                        currentMode = ViewMode.Normal;
                        continue;
                    }
                    
                    user = UserHandler.users.FirstOrDefault(u => u.Username == conversations[index]);
                    currentMode = ViewMode.Messages;
                    continue;
                
                case ViewMode.Messages:
                    Console.Clear();
                    Console.WriteLine("-----Meddelanden-----");
                    
                    MessageCLI.ShowMessages(user, false);
                    
                    Console.WriteLine("\n1. Skriv meddelande 2. Ta bort meddelande");
                    Console.WriteLine("Tryck esc för att gå tillbaka");
                    
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.D1:
                            MessageCLI.SendMessage(user);
                            continue;
                        
                        case ConsoleKey.D2:
                            MessageCLI.RemoveMessage(user); 
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
                        UserHandler.FollowUnfollow(username);
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

    // Tar in användare och nuvarande visar läge i profilen
    // och visar antingen användarens följare eller vilka de följer
    public static void ShowFollow(User username, ViewMode currentMode)
    {
        User userFound = UserHandler.users.FirstOrDefault(u => u == username);

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

    public static User SearchProfile()
    {
        Console.WriteLine("Tryck esc för att gå tillbaka"); 
        Console.Write("Sök: ");
        
        var search = Helpers.ReadUserInput();

        User user = UserHandler.users.FirstOrDefault(u => u.Username == search);
        if (user.Username != search)
        {
            Console.WriteLine("Kan inte hitta användaren.");
        }
        return user;
    }
}