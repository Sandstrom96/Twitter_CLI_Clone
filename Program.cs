using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {   
        Console.ForegroundColor = ConsoleColor.White;
        UserHandler.users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText("Users.json"));
        TweetHandler.tweets = JsonSerializer.Deserialize<List<Tweet>>(File.ReadAllText("Tweets.json"));
        var options = new JsonSerializerOptions{WriteIndented = true}; 
        //var b = new User("Oskar", "123" , "Oskar");
        //UserHandler.users.Add(b);
        bool validUser = false;

        //Meny där användaren får antingen registrera sig eller logga in 
        while (!validUser)
        {
            Console.WriteLine("Välkommen till Shitter");
            Console.WriteLine("1. Registrera");
            Console.WriteLine("2. Logga in");
            Console.WriteLine("Tryck esc för att stänga programmet");
            var choice = Console.ReadKey(true).Key;
            
            switch (choice)
            {
                case ConsoleKey.D1:
                    Console.Clear();
                    UserCLI.Register();
                    File.WriteAllText("Users.json", JsonSerializer.Serialize(UserHandler.users, options));
                    break;
                
                case ConsoleKey.D2:
                    Console.Clear();
                    validUser = UserCLI.LogIn();
                    if (!validUser)
                    {
                        continue;
                    }
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }

        //Kollar om den användaren loggat in och sedan visar huvudmenyn
        while (validUser)
        {
            Console.Clear();
            TweetHandler.SortTweets();
            Console.WriteLine("----Shitter----");
            
            TweetCLI.ShowTweets(TweetHandler.tweets, false); //Visar alla tweets i flödet
            
            Console.WriteLine("1. Tweet 2. Profil 3. Sök 4. Välj tweet 5. Avsluta");
            
            var choice = Console.ReadKey(true).Key;
            
            switch (choice)
            {
                case ConsoleKey.D1:
                    TweetCLI.MakeTweet();
                    break; 

                case ConsoleKey.D2:
                    UserCLI.ShowUserProfile(UserCLI.loggedInUser); 
                    break; 

                case ConsoleKey.D3:
                    var userList = UserCLI.Search();
                    if (userList == null)
                    {
                        break;
                    }
                    
                    UserCLI.ShowSearchedProfiles(userList);
                    var user = UserCLI.ChooseSearch(userList);
                    if (user == null)
                    {
                        break;
                    }
                    
                    UserCLI.ShowUserProfile(user);
                    break;

                case ConsoleKey.D4: //TODO: lägga till meny för gilla, kommentera, gå tillbaka
                    TweetCLI.ChooseTweet();
                    break;

                case ConsoleKey.D5: 
                    File.WriteAllText("Users.json", JsonSerializer.Serialize(UserHandler.users, options));
                    File.WriteAllText("Tweets.json",JsonSerializer.Serialize(TweetHandler.tweets, options));
                    return;
            }
            
        }

        
    }
}

public static class DynamicButtonhandler
{
    public static string FollowButton(User user)
    {
        User chosenUser = UserHandler.users.FirstOrDefault(u => u == user);
        if (!chosenUser.Followers.Any(u => u.Username == UserCLI.loggedInUser.Username))
        {
            return "Följ";
        }
        else
        {
            return "Avfölj";
        }
        
    }
        public static string LikeButton(Tweet tweet)
    {
        if (!tweet.Likes.Any(u => u == UserCLI.loggedInUser.Username))
        {
            return "🤍";
        }
        else
        {
            return "❤️";
        }
    }
}