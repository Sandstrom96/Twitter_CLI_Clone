class TweetCLI
{
    public static void MakeTweet()
    {
        Console.WriteLine("\nTryck Esc för att gå tillbaka");
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

        Console.WriteLine("Tryck enter för att Tweeta");
        var choice = Console.ReadKey(intercept: true).Key;
        
        switch (choice)
        {
            case ConsoleKey.Enter:
                TweetHandler.AddTweet(tweetContent);
                break;

            case ConsoleKey.Escape:
            return; 
        }
    }

    // Visar alla tweets från listan man tar in och kan visa index av alla tweets
    // om showIndex är sant
    public static void ShowTweets(List<Tweet> tweets, bool showIndex)
    {
        foreach(Tweet tweet in tweets)
        {
            var i = tweets.IndexOf(tweet);
            
            string likeHeart = Buttonhandler.LikeButton(tweet);
            string retweetButton = Buttonhandler.RetweetButton(tweet);
            
            if (showIndex)
            {
                Console.Write($"{i + 1}. ");
            }

            if (tweet.IsRetweet)
            {   
                var originalTweet = TweetHandler.GetOriginalTweet(tweet);

                likeHeart = Buttonhandler.LikeButton(originalTweet);
                retweetButton = Buttonhandler.RetweetButton(originalTweet);
                
                Console.WriteLine($"Retweet från: {tweet.Author}");
                RenderTweet(originalTweet, likeHeart, retweetButton);
            }
            else
            {
                RenderTweet(tweet, likeHeart, retweetButton);
            }
        }
    }

    // Visar vald tweet (index) från listan man tar in
    public static void ShowTweet(Tweet tweet, bool showIndex)
    {
        string likeHeart = Buttonhandler.LikeButton(tweet);
        string retweetButton = Buttonhandler.RetweetButton(tweet);
        
        if (tweet.IsRetweet)
        {   
            var originalTweet = TweetHandler.GetOriginalTweet(tweet);
            
            likeHeart = Buttonhandler.LikeButton(originalTweet);
            retweetButton = Buttonhandler.RetweetButton(originalTweet);
            
            Console.WriteLine($"Retweet från: {tweet.Author}");
            RenderTweet(originalTweet, likeHeart, retweetButton);
            Console.WriteLine("---- Kommentarer -----");
            if (showIndex)
            {
                CommentCLI.ShowComments(originalTweet,true);
            }
            else
            {
                CommentCLI.ShowComments(originalTweet,false);
            }
            //CommentCLI.ShowComment(originalTweet,false);
        }
        else
        {
            RenderTweet(tweet, likeHeart, retweetButton);
            Console.WriteLine("---- Kommentarer -----");
            if (showIndex)
            {
                CommentCLI.ShowComments(tweet,true);
            }
            else
            {
                CommentCLI.ShowComments(tweet,false);
            }
            //CommentCLI.ShowComment(tweet,false);
        }
    }

    public static void RenderTweet(Tweet tweet, string likeHeart, string retweetButton)
    {
        Console.WriteLine(tweet.Author);
        Console.WriteLine(tweet.Content);
        Console.WriteLine(tweet.Date.ToString("MM-dd HH:mm"));
        Console.WriteLine($"{likeHeart} ({tweet.Likes.Count}) 💬 ({tweet.Comments.Count}) {retweetButton} ({tweet.Retweet.Count})");
        Console.WriteLine("----------------------");
    }

    public static void RemoveTweet(Tweet tweet)
    {
        Console.Clear();
        var retweets = TweetHandler.tweets.Where(t => t.OriginalTweetId == tweet.Id).ToList();
        
        Console.WriteLine("Du vill ta bort tweeten:");
        Console.WriteLine(tweet.Author);
        Console.WriteLine(tweet.Content);
        Console.WriteLine(tweet.Date.ToString("MM-dd HH:mm"));

        Console.WriteLine("\nTryck Enter för att radera");
        Console.WriteLine("Tryck Esc för att avbryta"); 
        while (true)
        {
            var input = Console.ReadKey(true).Key;

            switch(input)
            {
                case ConsoleKey.Enter:
                TweetHandler.RemoveTweet(tweet, retweets);  
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
                string retweetButton = Buttonhandler.RetweetButton(t);
                
                RenderTweet(t, likeHeart, retweetButton);
            }
        }
    }

    public static void ChooseTweet(List<Tweet> tweets)
    {        
        Console.WriteLine("----- Välj Tweet -----");
        ShowTweets(tweets, true);
        
        if (tweets.Count == 0)
        {
            Console.WriteLine("\nHär var det tomt");
        }
        
        Console.WriteLine("\nTryck Esc för att gå tillbaka");
        
        if (tweets.Count > 0)
        {
            Console.WriteLine($"Välj tweet 1-{tweets.Count}");
        }
        
        int? index; 
        while (true)
        {   
            string input = Helpers.ReadUserInput();

            if (input == null)
            {
                return;
            }

            index = Helpers.CheckUserInput(tweets.Count,input);

            if(index > 0 && index <= tweets.Count)
            {
                break; 
            }
        }
        
        var tweet = tweets[(int)index - 1];
        var tweetIndex = tweet.Id;
        
        var originalTweet = tweet;
        if(tweet.IsRetweet)
        {
            originalTweet = TweetHandler.GetOriginalTweet(tweet);
        }
        
        while(true)
        {
            Console.Clear();
            ShowTweet(tweets[(int)index - 1], false);
            
            if(tweet.Author == UserCLI.loggedInUser.Username)
            {
                Console.WriteLine($"\n[1. Gilla] [2. Kommentera] [3. Ta bort kommentar] [4. Retweet] [5. Radera Tweet] [6. Tillbaka]");
            }
            else
            {
                Console.WriteLine($"\n[1. Gilla] [2. Kommentera] [3. Ta bort kommentar] [4. Retweet] [5. Tillbaka]");
            }
            
            var choice = Console.ReadKey(true).Key;
            switch (choice)
            {
                case ConsoleKey.D1:
                    TweetHandler.LikeUnlikeTweet(tweetIndex);
                    break;
                
                case ConsoleKey.D2:
                    CommentCLI.CommentTweet(tweet);
                    break;
                
                case ConsoleKey.D3:
                    CommentCLI.RemoveComment(tweet); 
                    break;
                    
                case ConsoleKey.D4:
                    if(!tweet.IsRetweet || tweet.Author != UserCLI.loggedInUser.Username)
                    {
                        TweetHandler.Retweet(tweetIndex);
                        continue;
                    }
                    else
                    {
                        TweetHandler.Retweet(tweetIndex);
                        return;
                    }

                case ConsoleKey.D5:
                    if(tweet.Author == UserCLI.loggedInUser.Username)
                    {
                        RemoveTweet(originalTweet);
                        return;
                    }
                    else
                    {
                        return;
                    }
                    
                case ConsoleKey.D6:
                    if(tweet.Author == UserCLI.loggedInUser.Username)
                        return;
                    else
                        continue;
            }
        }
    }
}