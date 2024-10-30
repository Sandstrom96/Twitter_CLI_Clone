using System.ComponentModel.Design;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.VisualBasic;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

class Program
{
    public static void Main(string[] args)
    {   
        UserHandler.users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText("Users.json"));
        TweetHandler.tweets = JsonSerializer.Deserialize<List<Tweet>>(File.ReadAllText("Tweets.json"));
        var options = new JsonSerializerOptions{WriteIndented = true}; 
        
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
            
            TweetHandler.ShowTweets(TweetHandler.tweets, 0); //Visar alla tweets i flödet
            
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
                    TweetHandler.ShowTweets(TweetHandler.tweets, choice); 
                    int index = int.Parse(Console.ReadLine());
                    TweetHandler.LikeUnlikeTweet(TweetHandler.tweets, index);
                    break;

                case ConsoleKey.D5: 
                    File.WriteAllText("Users.json", JsonSerializer.Serialize(UserHandler.users, options));
                    File.WriteAllText("Tweets.json",JsonSerializer.Serialize(TweetHandler.tweets, options));
                    return;
            }
            
        }

        
    }
}

public class User
{
    public string Username {get; set;}
    public string Password {get; set;}
    public string Name {get; set;}
    public List<User> Followers {get; set;} = new List<User>();
    public List<User> Following {get; set;} = new List<User>();
    public List<Tweet> OwnTweets {get; set;} = new List<Tweet>();
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
        Console.Write("Ange användarnamn: ");
        userName = Console.ReadLine();
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
    
    //Visar profilen enligt indatan tex. den inloggade eller sökta profilen
    //TODO: skapa metod för sidhuvud. 
    public static void ShowUserProfile(string username)
    {   
        Console.Clear();
        User chosenUser = users.FirstOrDefault(u => u.Username == username);
        Console.WriteLine(chosenUser.Name);
        Console.WriteLine($"@{chosenUser.Username}");
        Console.WriteLine($"Följare {chosenUser.Followers.Count}\tFöljer {chosenUser.Following.Count}");
        Console.WriteLine("----------------------");
        TweetHandler.ShowTweets(chosenUser.OwnTweets, 0);

        while (true)
        {
            if(chosenUser.Username == UserHandler.loggedInUser.Username)
            {
                Console.WriteLine("1. Följer  2. Följare  3. Meddelanden 4. Gillade 5. Radera tweet 6. Gå tillbaka");
                var choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.D1: 
                    break; 

                    case ConsoleKey.D2: 
                    break; 
                    
                    case ConsoleKey.D3: 
                    break;

                    case ConsoleKey.D4:
                        Console.WriteLine("Gillade tweets");
                        TweetHandler.ShowLikedTweets(chosenUser.Username);
                        break;
                    
                    case ConsoleKey.D5:
                    TweetHandler.ShowTweets(chosenUser.OwnTweets, ConsoleKey.D4);
                    Console.WriteLine("Välj vilken du vill radera");
                    choice = Console.ReadKey(true).Key;
                    TweetHandler.RemoveTweet((int)choice);
                    break;

                    case ConsoleKey.D6:
                    return;

                    default:
                    break;
                }
            }
            else
            {
                DynamicButtonhandler.FollowButton(username);
                Console.WriteLine($" 2. Skicka meddelande 3. Följer  4. Följare 5. Gillade 6. Gå tillbaka"); // metod för dynamisk följ/avfölj
                var choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.D1:
                        FollowUnfollow(username);
                    break; 

                    case ConsoleKey.D2: 
                    break; 
                    
                    case ConsoleKey.D3: 
                    break;
                    
                    case ConsoleKey.D4:
                    break;

                    case ConsoleKey.D5:
                    TweetHandler.ShowLikedTweets(username);
                    break;

                    case ConsoleKey.D6:
                    return;

                    default:
                    break;
                }
        }
        }
    }
    public static string SearchProfile()
    {
        StringBuilder sbSearch = new();
        
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
}
public class Tweet 
{
    public string Content {get; set;}
    public string Author {get; set;}
    public DateTime Date {get; set;}
    public List<string> Likes {get; set;} = new List<string>();

    public Tweet (string content, string author, DateTime date)
    {
        Content = content; 
        Author = author; 
        Date = date; 
    }
    
}

static class TweetHandler
{
    static public List<Tweet> tweets= new List<Tweet> ();

    public static void MakeTweet()
    {
        Console.Clear();
        Console.WriteLine("Tryck esc för att gå tillbaka");
        Console.Write("Skriv din tweet: ");
        StringBuilder sbTweetContent = new();
        
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
                    if(sbTweetContent.Length > 0)
                    {
                        sbTweetContent.Remove(sbTweetContent.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    sbTweetContent.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
        }
        
        string tweetContent = sbTweetContent.ToString();

        Console.WriteLine("1. Tweeta 2. Ångra");
        var choice = Console.ReadKey(intercept: true).Key;
        
        switch (choice)
        {
            case ConsoleKey.D1:
                var tweet = new Tweet(tweetContent, UserHandler.loggedInUser.Username, DateTime.Now);
            
                tweets.Add(tweet);
            
                User loggedInUser = UserHandler.users.FirstOrDefault(u => u.Username == UserHandler.loggedInUser.Username);
                loggedInUser.OwnTweets.Add(tweet);
                break; 

            case ConsoleKey.D2:
            return; 
        }
    }
    public static void SortTweets()
    {
        for (int i = 0; i < tweets.Count; i++)
        {
            for (int j = 0; j < tweets.Count -1; j++)
            {
                if (tweets[j].Date < tweets[j + 1].Date) 
                {
                    var temp = tweets[j];
                    tweets[j] = tweets[j + 1];
                    tweets[j + 1] = temp;
                }
            }
        }
    }
    public static void ShowTweets(List<Tweet> tweet, ConsoleKey choice)
    {
        if (choice == ConsoleKey.D4)
        {
            foreach(Tweet t in tweet)
            {
                var i = tweet.IndexOf(t);
                Console.Write($"{i + 1}. ");
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                DynamicButtonhandler.LikeButton(tweet, i);
                Console.WriteLine($" ({t.Likes.Count})"); //metod för gilla? röd om du gillar/vit om inte
            }
        }
        else 
        {
            foreach(Tweet t in tweet)
            {
                var i = tweet.IndexOf(t);
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                DynamicButtonhandler.LikeButton(tweet, i);
                Console.WriteLine($" ({t.Likes.Count})"); //metod för gilla? röd om du gillar/vit om inte
            }
        }
    }

    public static void RemoveTweet(int index)
    {
        User loggedInUser = UserHandler.users.FirstOrDefault(u => u.Username == UserHandler.loggedInUser.Username);
        
        var temp = loggedInUser.OwnTweets[index];
        var tempTweet = tweets.FirstOrDefault(t => t.Content == temp.Content && t.Author == temp.Author && t.Date == temp.Date);
        var indexOfTweet = tweets.IndexOf(tempTweet);
        
        if (index > loggedInUser.OwnTweets.Count)
        {
            Console.WriteLine($"Ogiltigt val, ange tweet 1 - {loggedInUser.OwnTweets.Count}");
        }
        else 
        {
            loggedInUser.OwnTweets.RemoveAt(index);
            tweets.RemoveAt(indexOfTweet);
            Console.WriteLine($"Tweeten togs bort");
        }
        
        Console.WriteLine("1. Ångra 2. Fortsätt");
        var choice = Console.ReadKey(true).Key; 
        
        if ((int)choice == 1)
        {
            loggedInUser.OwnTweets.Add(temp);
            tweets.Add(temp);
        }
         
    }

    public static void LikeUnlikeTweet(List<Tweet> tweet, int i)
    {
        if (!tweet[i - 1].Likes.Any(u => u == UserHandler.loggedInUser.Username))
        {
            tweet[i - 1].Likes.Add(UserHandler.loggedInUser.Username);
        }
        else
        {
            tweet[i - 1].Likes.Remove(UserHandler.loggedInUser.Username);
        }
    }

    public static void ShowLikedTweets(string username)
    {
        foreach(Tweet t in tweets)
        {
            if(t.Likes.Any(u => u == username))
            {
                var i = tweets.IndexOf(t);
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                DynamicButtonhandler.LikeButton(tweets, i);
                Console.WriteLine($" ({t.Likes.Count})");
            }
        }
    }
}

public static class DynamicButtonhandler
{
    public static void FollowButton(string username)
    {
        User chosenUser = UserHandler.users.FirstOrDefault(u => u.Username == username);
        if (!chosenUser.Followers.Any(u => u.Username == UserHandler.loggedInUser.Username))
        {
            Console.Write("1. Följ");
        }
        else
        {
            Console.Write("1. Avfölj");
        }
        
    }
        public static void LikeButton(List<Tweet> tweet, int i)
    {
        if (!tweet[i].Likes.Any(u => u == UserHandler.loggedInUser.Username))
        {
            Console.Write("🤍");
        }
        else
        {
            Console.Write("❤️");
        }
    }
}