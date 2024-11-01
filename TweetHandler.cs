using System.Text;

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
    static public List<Tweet> tweets = new List<Tweet> ();

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
    public static void ShowTweets(List<Tweet> tweet, bool index)
    {
        foreach(Tweet t in tweet)
        {
            var i = tweet.IndexOf(t);
            string likeHeart = DynamicButtonhandler.LikeButton(tweet, i);
            if (index)
            {
                Console.Write($"{i + 1}. ");
            }
            Console.WriteLine(t.Author);
            Console.WriteLine(t.Content);
            Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
            Console.WriteLine($"{likeHeart} ({t.Likes.Count})"); //metod för gilla? röd om du gillar/vit om inte
        }
    }
    public static void ShowTweets(List<Tweet> tweet, int index)
    {
        string likeHeart = DynamicButtonhandler.LikeButton(tweet, index);
        Console.WriteLine(tweet[index].Author);
        Console.WriteLine(tweet[index].Content);
        Console.WriteLine(tweet[index].Date.ToString("MM-dd HH:mm"));
        Console.WriteLine($"{likeHeart} ({tweet[index].Likes.Count})");
    }
    public static void RemoveTweet()
    {
        User loggedInUser = UserHandler.users.FirstOrDefault(u => u.Username == UserHandler.loggedInUser.Username);
        
        StringBuilder sbChoice = new StringBuilder();
        
        while (true)
        {   
            if(loggedInUser.OwnTweets.Count == 0)
            {
                Console.WriteLine("Du har inga tweets att radera. Tryck en tangent för att fortsätta.");
                Console.ReadKey(true);
                return; 
            }
            ShowTweets(loggedInUser.OwnTweets, true);
            Console.WriteLine("Tryck esc för att gå tillbaka");
            Console.WriteLine($"Välj vilken du vill radera (1-{loggedInUser.OwnTweets.Count})");
            
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
                    if(sbChoice.Length > 0)
                    {
                        sbChoice.Remove(sbChoice.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    sbChoice.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }

            if (sbChoice.ToString().All(char.IsDigit))
            {
                int choiceValue = int.Parse(sbChoice.ToString());

                // Kontrollera om siffran är inom listans längd
                if (choiceValue >= 0 && choiceValue <= loggedInUser.OwnTweets.Count)
                {
                    // Giltig inmatning
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("\nFel inmatning, försök igen!");
                    sbChoice.Clear();  // Rensa ogiltig inmatning
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("\nFel inmatning, försök igen! Endast siffror är tillåtna.");
                sbChoice.Clear();  // Rensa ogiltig inmatning
            }
        }
        
        string index = sbChoice.ToString();

        var temp = loggedInUser.OwnTweets[int.Parse(index)-1];
        var tempTweet = tweets.FirstOrDefault(t => t.Content == temp.Content && t.Author == temp.Author && t.Date == temp.Date);
        var indexOfTweet = tweets.IndexOf(tempTweet);
        
        if (index == "")
        {
            return;
        }
        else
        {
            loggedInUser.OwnTweets.RemoveAt(int.Parse(index)-1);
            tweets.RemoveAt(indexOfTweet);
            Console.WriteLine($"Tweeten togs bort");
        }
        
        Console.WriteLine("1. Ångra 2. Fortsätt");
        var choice = Console.ReadKey(true).Key; 
        
        if (choice == ConsoleKey.D1)
        {
            loggedInUser.OwnTweets.Add(temp);
            tweets.Add(temp);
        }
         
    }

    public static void LikeUnlikeTweet(List<Tweet> tweet, int i)
    {
        if (!tweet[i].Likes.Any(u => u == UserHandler.loggedInUser.Username))
        {
            tweet[i].Likes.Add(UserHandler.loggedInUser.Username);
        }
        else
        {
            tweet[i].Likes.Remove(UserHandler.loggedInUser.Username);
        }
    }

    public static void ShowLikedTweets(string username)
    {
        foreach(Tweet t in tweets)
        {
            if(t.Likes.Any(u => u == username))
            {
                var i = tweets.IndexOf(t);
                string likeHeart = DynamicButtonhandler.LikeButton(tweets, i);
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({t.Likes.Count})");
            }
        }
    }
    public static void ChooseTweet(ConsoleKey choice)
    {
        Console.Clear();
        
        ShowTweets(tweets, true);
        
        Console.WriteLine($"Välj tweet 1-{tweets.Count}");
        int index = int.Parse(Console.ReadLine()) - 1;
        
        Console.Clear();
        while(true)
        {
            ShowTweets(tweets, index);
            
            Console.WriteLine($"1. Gilla 2. Kommentera 3. Retweet 4. Hem");
            var choice1 = Console.ReadKey(true).Key;
            switch (choice1)
            {
                case ConsoleKey.D1:
                    TweetHandler.LikeUnlikeTweet(tweets, index);
                    break;
                
                case ConsoleKey.D2:
                    break;
                
                case ConsoleKey.D3:
                    break;
                    
                case ConsoleKey.D4:
                    return;
            }
        }
    }
}