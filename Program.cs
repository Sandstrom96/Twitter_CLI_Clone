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
            int choice = int.Parse(Console.ReadLine());
            
            switch (choice)
            {
                case 1:
                    UserHandler.Register();
                    File.WriteAllText("Users.json", JsonSerializer.Serialize(UserHandler.users, options));
                    break;
                
                case 2:
                    validUser = UserHandler.Login();
                    break;
            }
        }

        //Kollar om den användaren loggat in och sedan visar huvudmenyn
        while (validUser)
        {
            TweetHandler.SortTweets();
            Console.WriteLine("----Shitter----");
            
            TweetHandler.ShowTweets(TweetHandler.tweets, 0); //Visar alla tweets i flödet
            
            Console.WriteLine("1. Tweet 2. Profil 3. Sök 4. Välj tweet 5. Avsluta");
            
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                TweetHandler.MakeTweet();
                break; 

                case 2:
                UserHandler.ShowUserProfile(UserHandler.loggedInUser.Username); 
                break; 

                case 3:
                Console.Write("Sök: ");
                string search = Console.ReadLine();
                User searchedUser = UserHandler.users.FirstOrDefault(u => u.Username == search);
                UserHandler.ShowUserProfile(UserHandler.SearchProfile(search));
                break;

                case 4: 
                TweetHandler.ShowTweets(TweetHandler.tweets, choice); 
                choice = int.Parse(Console.ReadLine());
                TweetHandler.LikeUnlikeTweet(TweetHandler.tweets, choice); 
                break;

                case 5: 
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
    public static void ShowUserProfile(string username)
    {   
        Console.Clear();
        User chosenUser = users.FirstOrDefault(u => u.Username == username);
        Console.WriteLine(chosenUser.Name);
        Console.WriteLine($"@{chosenUser.Username}");
        Console.WriteLine($"Följare {chosenUser.Followers.Count}\tFöljer {chosenUser.Following.Count}");
        Console.WriteLine("----------------------");
        TweetHandler.ShowTweets(chosenUser.OwnTweets, 0);

        if(chosenUser.Username == UserHandler.loggedInUser.Username)
        {
            Console.WriteLine("1. Följer  2. Följare  3. Meddelanden 4. Gillade 5. Radera tweet 6. Gå tillbaka");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1: 
                break; 

                case 2: 
                break; 
                
                case 3: 
                break;

                case 4:
                TweetHandler.ShowLikedTweets(username);
                break;
                
                case 5:
                TweetHandler.ShowTweets(chosenUser.OwnTweets, 4);
                Console.WriteLine("Välj vilken du vill radera");
                choice = int.Parse(Console.ReadLine()) -1;
                TweetHandler.RemoveTweet(choice); 
                break;

                case 6:
                return;

                default:
                break;
            }
        }
        else
        {
            DynamicButtonhandler.FollowButton(username);
            Console.WriteLine($" 2. Skicka meddelande 3. Följer  4. Följare 5. Gillade 6. Gå tillbaka"); // metod för dynamisk följ/avfölj
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    FollowUnfollow(username);
                break; 

                case 2: 
                break; 
                
                case 3: 
                break;
                
                case 4:
                break;

                case 5:
                TweetHandler.ShowLikedTweets(username);
                break;

                case 6:
                return;

                default:
                break;
            }
        }
    }
    public static string SearchProfile(string user)
    {
        var userName = users.FirstOrDefault(u => u.Username == user);
        if (userName.Username != user)
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
        Console.Write("Skriv din tweet: ");
        string tweetContent = Console.ReadLine();
        
        var tweet = new Tweet(tweetContent, UserHandler.loggedInUser.Username, DateTime.Now);
        
        tweets.Add(tweet);
        
        User loggedInUser = UserHandler.users.FirstOrDefault(u => u.Username == UserHandler.loggedInUser.Username);
        loggedInUser.OwnTweets.Add(tweet);
         
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
    public static void ShowTweets(List<Tweet> tweet, int choice)
    {
        if (choice == 4)
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
        var tempTweet = tweets.FirstOrDefault(t => t.Content == temp.Content && t.Author == temp.Author);
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
        int choice = int.Parse(Console.ReadLine()); 
        if (choice == 1)
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