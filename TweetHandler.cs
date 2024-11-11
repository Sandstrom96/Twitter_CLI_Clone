using System.IO.Compression;
using System.Text;
using System.Transactions;
public class Comment
{
    public string Content { get; set; }
    public string Author { get; set; }
    public DateTime Timestamp { get; set; }

    public Comment(string content, string author)
    {
        Content = content;
        Author = author;
        Timestamp = DateTime.Now;
    }
}
public class Tweet 
{
    public Guid Id {get; set;}
    public string Content {get; set;}
    public string Author {get; set;}
    public DateTime Date {get; set;}
    public List<string> Likes {get; set;} = new List<string>();
    public List<Guid> Retweet {get; set;} = new List<Guid>();
    public List<Comment> Comments {get; set;} = new List<Comment>();
    public bool IsRetweet {get; set;} = false;
    public Guid OriginalTweetId {get; set;}

    public Tweet (string content, string author)
    {
        Id = Guid.NewGuid();
        Content = content;
        Author = author;
        Date = DateTime.Now;
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
                var tweet = new Tweet(tweetContent, UserHandler.loggedInUser.Username);
            
                tweets.Add(tweet);
            
                UserHandler.loggedInUser.OwnTweets.Add(tweet.Id);
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
    // Visar alla tweets från listan man tar in och kan visa index av alla tweets
    // om showIndex är sant
    public static void ShowTweets(List<Tweet> tweet, bool showIndex)
    {
        foreach(Tweet t in tweet)
        {
            var i = tweet.IndexOf(t);
            
            string likeHeart = DynamicButtonhandler.LikeButton(t);
            
            if (showIndex)
            {
                Console.Write($"{i + 1}. ");
            }

            if (t.IsRetweet)
            {   
                var originalTweet = tweets.FirstOrDefault(x => x.Id == t.OriginalTweetId);
                var originalTweetIndex = tweets.IndexOf(originalTweet);
                likeHeart = DynamicButtonhandler.LikeButton(originalTweet);
                
                Console.WriteLine($"Retweet från: {t.Author}");
                Console.WriteLine(originalTweet.Author);
                Console.WriteLine($"{originalTweet.Content}");
                Console.WriteLine(originalTweet.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({originalTweet.Likes.Count}) 💬 ({originalTweet.Comments.Count})");
                ShowComment(originalTweet);
                
            }
            else
            {
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({t.Likes.Count}) 💬 ({t.Comments.Count})");
                ShowComment(t); 
            }
        }
    }
    
    // Visar vald tweet (index) från listan man tar in
    public static void ShowTweets(Tweet tweet)
    {
        string likeHeart = DynamicButtonhandler.LikeButton(tweet);
        
        if (tweet.IsRetweet)
        {   
            var originalTweet = tweets.FirstOrDefault(x => x.Id == tweet.OriginalTweetId);
            var originalTweetIndex = tweets.IndexOf(originalTweet);
            
            likeHeart = DynamicButtonhandler.LikeButton(originalTweet);
            
            Console.WriteLine($"Retweet från: {tweet.Author}");
            Console.WriteLine(originalTweet.Author);
            Console.WriteLine($"{originalTweet.Content}");
            Console.WriteLine(originalTweet.Date.ToString("MM-dd HH:mm"));
            Console.WriteLine($"{likeHeart} ({originalTweet.Likes.Count}) 💬 ({originalTweet.Comments.Count})");
            ShowComment(originalTweet);
        }
        else
        {
            Console.WriteLine(tweet.Author);
            Console.WriteLine(tweet.Content);
            Console.WriteLine(tweet.Date.ToString("MM-dd HH:mm"));
            Console.WriteLine($"{likeHeart} ({tweet.Likes.Count}) 💬 ({tweet.Comments.Count})");
            ShowComment(tweet);
        }
    }
    public static void RemoveTweet()
    {
        StringBuilder sbChoice = new StringBuilder();

        var tweet = tweets.Where(t => UserHandler.loggedInUser.OwnTweets.Contains(t.Id) && !t.IsRetweet).ToList();
        
        while (true)
        {   
            if(tweet.Count == 0)
            {
                Console.WriteLine("Du har inga tweets att radera. Tryck en tangent för att fortsätta.");
                Console.ReadKey(true);
                return; 
            }
            ShowTweets(tweet, true);
            Console.WriteLine("Tryck esc för att gå tillbaka");
            Console.WriteLine($"Välj vilken du vill radera (1-{tweet.Count})");
            
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
                if (choiceValue >= 0 && choiceValue <= UserHandler.loggedInUser.OwnTweets.Count)
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fel inmatning, försök igen!");
                    Console.ForegroundColor = ConsoleColor.White;
                    sbChoice.Clear();
                }
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Endast siffror är tillåtna, försök igen!");
                Console.ForegroundColor = ConsoleColor.White;
                sbChoice.Clear();
            }
        }
        
        string index = sbChoice.ToString();

        var chosenTweet = tweet[int.Parse(index) - 1];
        var originalTweet = tweets.FirstOrDefault(t => t.Id == chosenTweet.OriginalTweetId);
        var retweets = tweets.Where(t => t.OriginalTweetId == chosenTweet.Id).ToList();
        
        Console.WriteLine("Du vill ta bort tweeten:");
        Console.WriteLine(chosenTweet.Author);
        Console.WriteLine(chosenTweet.Content);
        Console.WriteLine(chosenTweet.Date);
        Console.WriteLine("1. Radera 2. Avbryt");
        var choice = Console.ReadKey(true).Key;

        if (choice == ConsoleKey.D1)
        {
            if (index == "")
            {
                return;
            }
            else
            {
                UserHandler.loggedInUser.OwnTweets.Remove(chosenTweet.Id);
                foreach (var r in retweets)
                {
                    UserHandler.loggedInUser.OwnTweets.Remove(r.Id);
                }
                
                tweets.Remove(originalTweet);
                foreach(Tweet t in retweets)
                {
                    tweets.Remove(t);
                }
                
                foreach(var u in UserHandler.users)
                {
                    foreach (var r in retweets)
                    {
                        if (u.OwnTweets.Contains(r.Id))
                        {
                            u.OwnTweets.Remove(r.Id);
                        }
                    }
                }
                Console.WriteLine($"Tweeten togs bort");
            }
        }
    }

    public static void LikeUnlikeTweet(Guid id)
    {
        var tweet = tweets.FirstOrDefault(t => t.Id == id);
        
        if (tweet.IsRetweet)
        {
            var originalTweet = tweets.FirstOrDefault(t => t.Id == tweet.OriginalTweetId);

            if (!originalTweet.Likes.Any(u => u == UserHandler.loggedInUser.Username))
            {
                originalTweet.Likes.Add(UserHandler.loggedInUser.Username);
            }
            else
            {
                originalTweet.Likes.Remove(UserHandler.loggedInUser.Username);
            }
        }
        else
        {
            if (!tweet.Likes.Any(u => u == UserHandler.loggedInUser.Username))
            {
                tweet.Likes.Add(UserHandler.loggedInUser.Username);
            }
            else
            {
                tweet.Likes.Remove(UserHandler.loggedInUser.Username);
            }
        }
    }

    public static void ShowLikedTweets(string username)
    {
        foreach(Tweet t in tweets)
        {
            if(t.Likes.Any(u => u == username))
            {
                var i = tweets.IndexOf(t);
                string likeHeart = DynamicButtonhandler.LikeButton(t);
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({t.Likes.Count})");
            }
        }
    }
    public static void ChooseTweet()
    {
        Console.Clear();
        
        ShowTweets(tweets, true);
        
        Console.WriteLine($"Välj tweet 1-{tweets.Count}");
        int index = int.Parse(Console.ReadLine()) - 1;
        var tweetIndex = tweets[index].Id;
        
        Console.Clear();
        while(true)
        {
            ShowTweets(tweets[index]);
            
            Console.WriteLine($"1. Gilla 2. Kommentera 3. Retweet 4. Hem");
            var choice1 = Console.ReadKey(true).Key;
            switch (choice1)
            {
                case ConsoleKey.D1:
                    LikeUnlikeTweet(tweetIndex);
                    break;
                
                case ConsoleKey.D2:
                    CommentTweet(tweets[index]);
                    break;
                
                case ConsoleKey.D3:
                    Retweet(tweetIndex);
                    break;
                    
                case ConsoleKey.D4:
                    return;
            }
        }
    }

    public static void Retweet(Guid id)
    {
        var originalTweet = tweets.FirstOrDefault(x => x.Id == id);
        
        // Kollar om det är en retweet, om true hämtar originalet
        if (originalTweet.IsRetweet)
        {
            originalTweet = tweets.FirstOrDefault(x => x.Id == originalTweet.OriginalTweetId);
        }
        
        var existingRetweet = tweets.FirstOrDefault(x => x.OriginalTweetId == originalTweet.Id && x.Author == UserHandler.loggedInUser.Username);

        if (existingRetweet != null)
        {
            tweets.Remove(existingRetweet);
            UserHandler.loggedInUser.OwnTweets.Remove(existingRetweet.Id);
        }
        else
        {
            var retweet = new Tweet(null, UserHandler.loggedInUser.Username) {OriginalTweetId = originalTweet.Id, IsRetweet = true};
            tweets.Add(retweet);
            UserHandler.loggedInUser.OwnTweets.Add(retweet.Id);
        }
    }

    public static void CommentTweet(Tweet tweet)
    {
        Console.Clear();
        ShowTweets(tweet);
        Console.WriteLine("Tryck esc för att avbryta");
        Console.WriteLine("Skriv din kommentar:");
        StringBuilder sbComment = new();
        
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
                if(sbComment.Length > 0)
                {
                    sbComment.Remove(sbComment.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else
            {
                sbComment.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
        
        string commentContent = sbComment.ToString();

        var comment = new Comment(commentContent, UserHandler.loggedInUser.Username);
            
        tweet.Comments.Add(comment);
    }
    public static void ShowComment (Tweet tweet)
    {
        foreach(var c in tweet.Comments)
        {
            Console.WriteLine("--------------------"); 
            Console.WriteLine($"{c.Content}"); 
            Console.WriteLine($"{c.Author} {c.Timestamp:MM-DD HH-mm}"); 
        }
    }
}