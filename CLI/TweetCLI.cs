class TweetCLI
{
    public static void MakeTweet()
    {
        Console.WriteLine("Tryck esc för att gå tillbaka");
        Console.Write("Skriv din tweet: ");
        string tweetContent;
        while(true)
        {
            tweetContent = Helpers.ReadUserInput();

            if (tweetContent == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(tweetContent))
            {
                Helpers.ShowErrorMessage("Tweeten får inte vara tom.");
                continue;
            }
            break;
        }

        Console.WriteLine("1. Tweeta 2. Ångra");
        var choice = Console.ReadKey(intercept: true).Key;
        
        switch (choice)
        {
            case ConsoleKey.D1:
                TweetHandler.AddTweet(tweetContent);
                break;

            case ConsoleKey.D2:
            return; 
        }
    }

    // Visar alla tweets från listan man tar in och kan visa index av alla tweets
    // om showIndex är sant
    public static void ShowTweets(List<Tweet> tweet, bool showIndex)
    {
        foreach(Tweet t in tweet)
        {
            var i = tweet.IndexOf(t);
            
            string likeHeart = Buttonhandler.LikeButton(t);
            string retweetButton = Buttonhandler.RetweetButton(t);
            
            if (showIndex)
            {
                Console.Write($"{i + 1}. ");
            }

            if (t.IsRetweet)
            {   
                var originalTweet = TweetHandler.GetOriginalTweet(t);

                likeHeart = Buttonhandler.LikeButton(originalTweet);
                retweetButton = Buttonhandler.RetweetButton(originalTweet);
                
                Console.WriteLine($"Retweet från: {t.Author}");
                Console.WriteLine(originalTweet.Author);
                Console.WriteLine($"{originalTweet.Content}");
                Console.WriteLine(originalTweet.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({originalTweet.Likes.Count}) 💬 ({originalTweet.Comments.Count}) {retweetButton} ({originalTweet.Retweet.Count})");
                Console.WriteLine("---------------------");
            }
            else
            {
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({t.Likes.Count}) 💬 ({t.Comments.Count}) {retweetButton} ({t.Retweet.Count})");
                Console.WriteLine("---------------------");
            }
        }
    }

    // Visar vald tweet (index) från listan man tar in
    public static void ShowTweets(Tweet tweet)
    {
        string likeHeart = Buttonhandler.LikeButton(tweet);
        string retweetButton = Buttonhandler.RetweetButton(tweet);
        
        if (tweet.IsRetweet)
        {   
            var originalTweet = TweetHandler.GetOriginalTweet(tweet);
            
            likeHeart = Buttonhandler.LikeButton(originalTweet);
            retweetButton = Buttonhandler.RetweetButton(originalTweet);
            
            Console.WriteLine($"Retweet från: {tweet.Author}");
            Console.WriteLine(originalTweet.Author);
            Console.WriteLine($"{originalTweet.Content}");
            Console.WriteLine(originalTweet.Date.ToString("MM-dd HH:mm"));
            Console.WriteLine($"{likeHeart} ({originalTweet.Likes.Count}) 💬 ({originalTweet.Comments.Count}) {retweetButton} ({originalTweet.Retweet.Count})");
            CommentCLI.ShowComment(originalTweet,false);
        }
        else
        {
            Console.WriteLine(tweet.Author);
            Console.WriteLine(tweet.Content);
            Console.WriteLine(tweet.Date.ToString("MM-dd HH:mm"));
            Console.WriteLine($"{likeHeart} ({tweet.Likes.Count}) 💬 ({tweet.Comments.Count}) {retweetButton} ({tweet.Retweet.Count})");
            CommentCLI.ShowComment(tweet,false);
        }
    }
    public static void RemoveTweet()
    {
        var tweet = TweetHandler.tweets.Where(t => UserCLI.loggedInUser.OwnTweets.Contains(t.Id) && !t.IsRetweet).ToList();

        if(tweet.Count == 0)
        {
            Console.WriteLine("Du har inga tweets att radera. Tryck en tangent för att fortsätta.");
            Console.ReadKey(true);
            return; 
        }
            
        ShowTweets(tweet, true);
        Console.WriteLine("Tryck esc för att gå tillbaka");
        Console.WriteLine($"Välj vilken du vill radera (1-{tweet.Count})");

        int? index; 
        while (true)
        {   
            string choice = Helpers.ReadUserInput();

            if (choice == null)
            {
                return;
            }

            index = Helpers.CheckUserInput(tweet.Count,choice);

            if(index > 0 && index <= tweet.Count)
            {
                break; 
            }
        }
        
        var chosenTweet = tweet[(int)index -1];
        var originalTweet = TweetHandler.GetOriginalTweet(chosenTweet);
        var retweets = TweetHandler.tweets.Where(t => t.OriginalTweetId == chosenTweet.Id).ToList();
        
        
        Console.WriteLine("Du vill ta bort tweeten:");
        Console.WriteLine(chosenTweet.Author);
        Console.WriteLine(chosenTweet.Content);
        Console.WriteLine(chosenTweet.Date.ToString("MM-dd HH:mm"));

        Console.WriteLine("1. Radera");
        Console.WriteLine("Tryck Esc för att avbryta"); 
        while (true)
        {
            var input = Console.ReadKey(true).Key;

            switch(input)
            {
                case ConsoleKey.D1:
                TweetHandler.RemoveTweet(chosenTweet, retweets, originalTweet);  
                return;

                case ConsoleKey.Escape: 
                return; 

            }
        }
    }

    public static void ShowLikedTweets(string username)
    {
        foreach(Tweet t in TweetHandler.tweets)
        {
            if(t.Likes.Any(u => u == username))
            {
                var i = TweetHandler.tweets.IndexOf(t);
                string likeHeart = Buttonhandler.LikeButton(t);
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
        
        ShowTweets(TweetHandler.tweets, true);
        
        Console.WriteLine($"Välj tweet 1-{TweetHandler.tweets.Count}");
        int index = int.Parse(Console.ReadLine()) - 1;
        var tweetIndex = TweetHandler.tweets[index].Id;
        
        Console.Clear();
        while(true)
        {
            ShowTweets(TweetHandler.tweets[index]);
            
            Console.WriteLine($"1. Gilla 2. Kommentera 3. Ta bort kommentar 4. Retweet 5.Hem");
            var choice1 = Console.ReadKey(true).Key;
            switch (choice1)
            {
                case ConsoleKey.D1:
                    TweetHandler.LikeUnlikeTweet(tweetIndex);
                    break;
                
                case ConsoleKey.D2:
                    CommentCLI.CommentTweet(TweetHandler.tweets[index]);
                    break;
                
                case ConsoleKey.D3:
                    CommentCLI.RemoveComment(TweetHandler.tweets[index]); 
                    break;
                    
                case ConsoleKey.D4:
                    TweetHandler.Retweet(tweetIndex);
                    return;

                case ConsoleKey.D5:
                return; 
            }
        }
    }
}