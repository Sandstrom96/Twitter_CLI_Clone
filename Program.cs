using System.ComponentModel.Design;
using System.Dynamic;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;

class Program
{
    public static void Main(string[] args)
    {    
        bool validUser = false;
          
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
                    break;
                
                case 2:
                    validUser = UserHandler.Login();
                    break;
            }
        }

        while (validUser)
        {
            Console.WriteLine("----Shitter----");
            Console.WriteLine("1. Tweet 2. Profil 3. Sök");
            TweetHandler.MakeTweet();
            foreach(Tweet t in TweetHandler.tweets)
            {
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date);
            }
            validUser = false;
        }

        
    }
}

class User
{
    public string Username {get; set;}
    public string Password {get; set;}
    public string Name {get; set;}
    public List<User> followers {get; set;} = new List<User>();
    public List<User> following {get; set;} = new List<User>();
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
    public static string loggedInUser;
    static List<User> users = new List<User>();
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
                    
        while (!validUser)
        {
            Console.Write("Ange användarnamn: ");
            userName = Console.ReadLine();
            Console.Write("Ange lössenord: ");
            password = Console.ReadLine();
            for (int i = 0; i < users.Count; i++)
            {
                if (!userName.Equals(users[i].Username))
                {
                    Console.WriteLine("Kan inte hitta ett konto med det användarnamn.");
                }
                else if (!password.Equals(users[i].Password))
                {
                    Console.WriteLine("Du har anget fel lössenord!");
                }
                else if (userName.Equals(users[i].Username) && password.Equals(users[i].Password))
                {
                    loggedInUser = userName;
                    return true;
                }
            }
        
        }
        return false;
    }
}
class Tweet 
{
    public string Content {get; set;}
    public string Author {get; set;}
    public DateTime Date {get; set;}

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
        string tweet = Console.ReadLine();
        tweets.Add(new Tweet (tweet, UserHandler.loggedInUser, DateTime.Now));
    }
}