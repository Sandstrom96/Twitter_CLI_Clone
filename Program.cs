using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {   
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
            var choice = Console.ReadKey(true).Key;
            
            switch (choice)
            {
                case ConsoleKey.D1:
                    UserHandler.Register();
                    File.WriteAllText("Users.json", JsonSerializer.Serialize(UserHandler.users, options));
                    break;
                
                case ConsoleKey.D2:
                    validUser = UserHandler.Login();
                    break;
            }
        }

        //Kollar om den användaren loggat in och sedan visar huvudmenyn
        while (validUser)
        {
            Console.Clear();
            TweetHandler.SortTweets();
            Console.WriteLine("----Shitter----");
            
            TweetHandler.ShowTweets(TweetHandler.tweets, false); //Visar alla tweets i flödet
            
            Console.WriteLine("1. Tweet 2. Profil 3. Sök 4. Välj tweet 5. Avsluta");
            
            var choice = Console.ReadKey(true).Key;
            
            switch (choice)
            {
                case ConsoleKey.D1:
                    TweetHandler.MakeTweet();
                    break; 

                case ConsoleKey.D2:
                    UserHandler.ShowUserProfile(UserHandler.loggedInUser.Username); 
                    break; 

                case ConsoleKey.D3:
                    string username = UserHandler.SearchProfile();
                    if (username == null)
                    {
                        break;
                    }
                    UserHandler.ShowUserProfile(username);
                    break;

                case ConsoleKey.D4: //TODO: lägga till meny för gilla, kommentera, gå tillbaka
                    TweetHandler.ChooseTweet(choice);
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
    public static string FollowButton(string username)
    {
        User chosenUser = UserHandler.users.FirstOrDefault(u => u.Username == username);
        if (!chosenUser.Followers.Any(u => u.Username == UserHandler.loggedInUser.Username))
        {
            return "Följ";
        }
        else
        {
            return "Avfölj";
        }
        
    }
        public static string LikeButton(List<Tweet> tweet, int i)
    {
        if (!tweet[i].Likes.Any(u => u == UserHandler.loggedInUser.Username))
        {
            return "🤍";
        }
        else
        {
            return "❤️";
        }
    }
}