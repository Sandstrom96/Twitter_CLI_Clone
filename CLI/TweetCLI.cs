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
    public static void ShowTweet(Tweet tweet)
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
            CommentCLI.ShowComment(originalTweet,false);
        }
        else
        {
            RenderTweet(tweet, likeHeart, retweetButton);
            CommentCLI.ShowComment(tweet,false);
        }
    }

    public static void RenderTweet(Tweet tweet, string likeHeart, string retweetButton)
    {
        Console.WriteLine(tweet.Author);
        Console.WriteLine(tweet.Content);
        Console.WriteLine(tweet.Date.ToString("MM-dd HH:mm"));
        Console.WriteLine($"{likeHeart} ({tweet.Likes.Count}) 💬 ({tweet.Comments.Count}) {retweetButton} ({tweet.Retweet.Count})");
        Console.WriteLine("---------------------");
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

    public static void ChooseTweet(List<Tweet> tweets)
    {        
        ShowTweets(tweets, true);
        
        Console.WriteLine($"Välj tweet 1-{tweets.Count}");
        int index = int.Parse(Console.ReadLine()) - 1;
        var tweet = tweets[index];
        var tweetIndex = tweets[index].Id;
        if(tweets[index].IsRetweet)
        {
            tweet = TweetHandler.GetOriginalTweet(tweets[index]);
        }
        
        Console.Clear();
        while(true)
        {
            ShowTweet(tweets[index]);
            
            if(tweet.Author == UserCLI.loggedInUser.Username)
            {
                Console.WriteLine($"1. Gilla 2. Kommentera 3. Ta bort kommentar 4. Retweet 5. Radera Tweet 6. Tillbaka");
            }
            else
            {
                Console.WriteLine($"1. Gilla 2. Kommentera 3. Ta bort kommentar 4. Retweet 5. Tillbaka");
            }
            
            var choice = Console.ReadKey(true).Key;
            switch (choice)
            {
                case ConsoleKey.D1:
                    TweetHandler.LikeUnlikeTweet(tweetIndex);
                    break;
                
                case ConsoleKey.D2:
                    CommentCLI.CommentTweet(tweets[index]);
                    break;
                
                case ConsoleKey.D3:
                    CommentCLI.RemoveComment(tweets[index]); 
                    break;
                    
                case ConsoleKey.D4:
                    TweetHandler.Retweet(tweetIndex);
                    break;

                case ConsoleKey.D5:
                    if(tweet.Author == UserCLI.loggedInUser.Username)
                    {
                        RemoveTweet(tweet);
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