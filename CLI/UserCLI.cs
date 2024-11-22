using System.Xml;
class UserCLI
{
    static User defaultUser = new User("", "", "");
    public static User loggedInUser = defaultUser;
    public static void Register()
    {
        Console.WriteLine("--- Registrering ---");
        Console.WriteLine("Tryck Esc för att avbryta");

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
                Helpers.ShowErrorMessage("Användarnamn får inte vara tomt.");
                continue;
            }

            username = UserHandler.CheckUsernameForWhitespaces(username);
            Helpers.ShowErrorMessage("Ditt användarnamn kan inte innehålla mellanslag.");
            Console.WriteLine($"Detta är nu ditt användarnamn: {username}");

            if (!UserHandler.IsUsernameAvailable(username))
            {
                Helpers.ShowErrorMessage("Användarnamn upptaget.");
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
                Helpers.ShowErrorMessage("Lösenordet får inte vara tomt.");
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
                Helpers.ShowErrorMessage("Lösenorden matchar inte. Försök igen!");
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
                Helpers.ShowErrorMessage("Namn får inte vara tomt.");
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
            Console.WriteLine("Tryck Esc för att avbryta");

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
                    Helpers.ShowErrorMessage("Användarnamn får inte vara tomt.");
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
                    Helpers.ShowErrorMessage("Lösenordet får inte vara tomt.");
                    continue;
                }

                if (!UserHandler.validUsername(username))
                {
                    Helpers.ShowErrorMessage("Kan inte hitta ett konto med det användarnamn");
                    continue;
                }

                if (!UserHandler.validPassword(password))
                {
                    Helpers.ShowErrorMessage("Du har angett fel lösenord!");
                    continue;
                }

                loggedInUser = UserHandler.GetLoggedInUser(username);
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
        Followers,
        Following,
        Messages,
        Conversations,
    }

    public static void RenderHeader(User user)
    {
        Console.WriteLine(user.Name);
        Console.WriteLine($"@{user.Username}");
        Console.WriteLine($"Följare ({user.Followers.Count})  Följer ({user.Following.Count})");
        Console.WriteLine("----------------------");
    }

    //Visar profilen enligt indatan tex. den inloggade eller sökta profilen
    public static void ShowUserProfile(User username)
    {
        User user = UserHandler.GetUser(username);

        var currentMode = ViewMode.Normal;

        while (true)
        {
            string followButton = Buttonhandler.FollowButton(username);

            var userTweets = TweetHandler.GetUserTweets(username);
            var unreadMessages = MessageHandler.GetUnreadMessages();

            Console.Clear();
            Console.WriteLine("------- Profil -------");
            RenderHeader(user);

            switch (currentMode)
            {
                case ViewMode.Normal:
                    TweetCLI.ShowTweets(userTweets, false);

                    if (user.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"\n[1. Följer] [2. Följare] [3. Meddelanden ({unreadMessages.Count})] [4. Gillade] [5. Välj tweet] [6. Hem]");
                    }
                    else
                    {
                        Console.WriteLine($"\n[1. {followButton}] [2. Skicka meddelande] [3. Följer] [4. Följare] [5. Gillade] [6. Välj tweet] [7. Hem]");
                    }
                    break;

                case ViewMode.LikedTweets:
                    Console.WriteLine("       Gillade");
                    Console.WriteLine("----------------------");

                    TweetCLI.ShowLikedTweets(user.Username);

                    if (user.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"\n[1. Följer] [2. Följare] [3. Meddelanden ({unreadMessages.Count})] [4. Profil] [5. Välj tweet] [6. Hem]");
                    }
                    else
                    {
                        Console.WriteLine($"\n[1. {followButton}] [2. Skicka meddelande] [3. Följer] [4. Följare] [5. Profil] [6. Välj tweet] [7. Hem]");
                    }
                    break;


                case ViewMode.Followers:
                    Console.WriteLine("        Följare");
                    Console.WriteLine("----------------------");

                    ShowFollowers(username, currentMode);

                    if (user.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"\n[1. Följer] [2. Profil] [3. Meddelanden ({unreadMessages.Count})] [4. Gillade] [5. Välj tweet] [6. Hem]");
                    }
                    else
                    {
                        Console.WriteLine($"\n[1. {followButton}] [2. Skicka meddelande] [3. Följer] [4. Profil] [5. Gillade] [6. Välj tweet] [7. Hem]");
                    }
                    break;

                case ViewMode.Following:
                    Console.WriteLine("        Följer");
                    Console.WriteLine("----------------------");

                    ShowFollowers(username, currentMode);

                    if (user.Username == loggedInUser.Username)
                    {
                        Console.WriteLine($"\n[1. Profil] [2. Följare] [3. Meddelanden ({unreadMessages.Count})] [4. Gillade] [5. Välj tweet] [6. Hem]");
                    }
                    else
                    {
                        Console.WriteLine($"\n[1. {followButton}] [2. Skicka meddelande] [3. Profil] [4. Följare] [5. Gillade] [6. Välj tweet] [7. Hem]");
                    }
                    break;

                case ViewMode.Conversations:
                    var conversations = MessageHandler.GetConversations();
                    Console.WriteLine("     Meddelanden");
                    Console.WriteLine("----------------------\n");

                    if (conversations.Count <= 0)
                    {
                        Console.WriteLine("Här var det tomt.\n");
                    }

                    for (int i = 0; i < conversations.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {conversations[i]} ({MessageHandler.UnreadConversations(conversations[i]).Count})");
                    }

                    if (conversations.Count > 0)
                    {
                        Console.WriteLine("\nVälj vilken konversation du vill öppna");
                    }
                    Console.WriteLine("Tryck Esc för att gå tillbaka");

                    var input = Helpers.ReadUserInput();

                    if (input == null)
                    {
                        currentMode = ViewMode.Normal;
                        continue;
                    }

                    if (!int.TryParse(input, out var index) || index <= 0 || index > conversations.Count)
                    {
                        continue;
                    }

                    index -= 1;

                    user = MessageHandler.GetConversation(index, conversations);
                    currentMode = ViewMode.Messages;
                    continue;

                case ViewMode.Messages:
                    Console.WriteLine("     Meddelanden");
                    Console.WriteLine("----------------------");
                    MessageCLI.ShowMessages(user, false);

                    Console.WriteLine("\n1. Skriv meddelande 2. Ta bort meddelande");
                    Console.WriteLine("Tryck Esc för att gå tillbaka");

                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.D1:
                            MessageCLI.SendMessage(user);
                            continue;

                        case ConsoleKey.D2:
                            MessageCLI.RemoveMessage(user);
                            continue;

                        case ConsoleKey.Escape:
                            user = UserHandler.GetUser(username);
                            currentMode = ViewMode.Normal;
                            continue;
                    }
                    break;
            }

            if (user.Username == loggedInUser.Username)
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
                        Console.Clear();
                        RenderHeader(user);
                        TweetCLI.ChooseTweet(userTweets);
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
                        Console.Clear();
                        RenderHeader(user);
                        TweetCLI.ChooseTweet(userTweets);
                        break;

                    case ConsoleKey.D7:
                        return;
                }
            }
        }
    }

    // Tar in användare och nuvarande visar läge i profilen
    // och visar antingen användarens följare eller vilka de följer
    public static void ShowFollowers(User username, ViewMode currentMode)
    {
        User userFound = UserHandler.GetUser(username);

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

    public static List<User>? Search()
    {
        Console.Clear();
        Console.WriteLine("------- Sök -------");
        Console.WriteLine("Tryck Esc för att gå tillbaka");
        Console.Write("Sök: ");

        var search = Helpers.ReadUserInput();

        if (search == null)
        {
            return null;
        }

        var userList = UserHandler.GetSearchUsers(search);

        if (userList == null)
        {
            return null;
        }
        return userList;
    }
    public static void ShowSearchedProfiles(List<User> userList)
    {
        Console.WriteLine("\nProfiler:");

        if (userList.Count == 0)
        {
            Console.WriteLine("Finns inga användare som liknar din sökning");
        }
        else
        {
            foreach (var u in userList)
            {
                var i = userList.IndexOf(u);
                Console.WriteLine($"{i + 1}. {u.Username}");
            }
        }
    }

    public static User? ChooseSearch(List<User> userList)
    {
        if (userList.Count > 0)
        {
            Console.WriteLine($"Välj användare (1-{userList.Count})");
        }

        var index = -1;
        while (index > userList.Count || index < userList.Count)
        {
            var input = Helpers.ReadUserInput();

            if (input == null)
            {
                return null;
            }

            if (!int.TryParse(input, out index) || index <= 0 || index > userList.Count)
            {
                Helpers.ShowErrorMessage("Ogiltigt index. Försök igen!");
                continue;
            }
        }

        index -= 1;
        return UserHandler.GetUser(userList[index]);
    }
}