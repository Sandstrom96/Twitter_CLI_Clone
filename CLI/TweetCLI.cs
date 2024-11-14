class TweetCLI
{
    public static void MakeTweet()
    {
        Console.Clear();
        Console.WriteLine("Tryck esc för att gå tillbaka");
        Console.Write("Skriv din tweet: ");
        var tweetContent = Helpers.ReadUserInput();

        Console.WriteLine("1. Tweeta 2. Ångra");
        var choice = Console.ReadKey(intercept: true).Key;
        
        switch (choice)
        {
            case ConsoleKey.D1:
                var tweet = new Tweet(tweetContent, UserCLI.loggedInUser.Username);
            
                TweetHandler.tweets.Add(tweet);
            
                UserCLI.loggedInUser.OwnTweets.Add(tweet.Id);
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
            
            string likeHeart = DynamicButtonhandler.LikeButton(t);
            
            if (showIndex)
            {
                Console.Write($"{i + 1}. ");
            }

            if (t.IsRetweet)
            {   
                var originalTweet = TweetHandler.tweets.FirstOrDefault(x => x.Id == t.OriginalTweetId);

                var originalTweetIndex = TweetHandler.tweets.IndexOf(originalTweet);

                
                likeHeart = DynamicButtonhandler.LikeButton(originalTweet);
                
                Console.WriteLine($"Retweet från: {t.Author}");
                Console.WriteLine(originalTweet.Author);
                Console.WriteLine($"{originalTweet.Content}");
                Console.WriteLine(originalTweet.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({originalTweet.Likes.Count}) 💬 ({originalTweet.Comments.Count})");
                CommentCLI.ShowComment(originalTweet);
                
            }
            else
            {
                Console.WriteLine(t.Author);
                Console.WriteLine(t.Content);
                Console.WriteLine(t.Date.ToString("MM-dd HH:mm"));
                Console.WriteLine($"{likeHeart} ({t.Likes.Count}) 💬 ({t.Comments.Count})");
                CommentCLI.ShowComment(t); 
            }
        }
    }

    // Visar vald tweet (index) från listan man tar in
    public static void ShowTweets(Tweet tweet)
    {
        string likeHeart = DynamicButtonhandler.LikeButton(tweet);
        
        if (tweet.IsRetweet)
        {   
            var originalTweet = TweetHandler.tweets.FirstOrDefault(x => x.Id == tweet.OriginalTweetId);

            var originalTweetIndex = TweetHandler.tweets.IndexOf(originalTweet);

            
            likeHeart = DynamicButtonhandler.LikeButton(originalTweet);
            
            Console.WriteLine($"Retweet från: {tweet.Author}");
            Console.WriteLine(originalTweet.Author);
            Console.WriteLine($"{originalTweet.Content}");
            Console.WriteLine(originalTweet.Date.ToString("MM-dd HH:mm"));
            Console.WriteLine($"{likeHeart} ({originalTweet.Likes.Count}) 💬 ({originalTweet.Comments.Count})");
            CommentCLI.ShowComment(originalTweet);
        }
        else
        {
            Console.WriteLine(tweet.Author);
            Console.WriteLine(tweet.Content);
            Console.WriteLine(tweet.Date.ToString("MM-dd HH:mm"));
            Console.WriteLine($"{likeHeart} ({tweet.Likes.Count}) 💬 ({tweet.Comments.Count})");
            CommentCLI.ShowComment(tweet);
        }
    }
    public static void RemoveTweet()
    {
        string choice; 

        var tweet = TweetHandler.tweets.Where(t => UserCLI.loggedInUser.OwnTweets.Contains(t.Id) && !t.IsRetweet).ToList();
        
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
            
            choice = Helpers.ReadUserInput(); 
            

            if (choice.All(char.IsDigit))
            {
                int choiceValue = int.Parse(choice.ToString());

                // Kontrollera om siffran är inom listans längd
                if (choiceValue >= 0 && choiceValue <= UserCLI.loggedInUser.OwnTweets.Count)
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fel inmatning, försök igen!");
                    Console.ForegroundColor = ConsoleColor.White;
                    choice = null;
                }
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Endast siffror är tillåtna, försök igen!");
                Console.ForegroundColor = ConsoleColor.White;
                choice = null;
            }
        }
        
        string index = choice;

        var chosenTweet = tweet[int.Parse(index) - 1];
        var originalTweet = TweetHandler.tweets.FirstOrDefault(t => t.Id == chosenTweet.OriginalTweetId);
        var retweets = TweetHandler.tweets.Where(t => t.OriginalTweetId == chosenTweet.Id).ToList();
        
        
        Console.WriteLine("Du vill ta bort tweeten:");
        Console.WriteLine(chosenTweet.Author);
        Console.WriteLine(chosenTweet.Content);
        Console.WriteLine(chosenTweet.Date);
        Console.WriteLine("1. Radera 2. Avbryt");
        var input = Console.ReadKey(true).Key;

        if (input == ConsoleKey.D1)
        {
            if (index == "")
            {
                return;
            }
            else
            {
                UserCLI.loggedInUser.OwnTweets.Remove(chosenTweet.Id);
                foreach (var r in retweets)
                {
                    UserCLI.loggedInUser.OwnTweets.Remove(r.Id);
                }
                
                TweetHandler.tweets.Remove(originalTweet);

                foreach(Tweet t in retweets)
                {
                    TweetHandler.tweets.Remove(t);
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

    public static void ShowLikedTweets(string username)
    {
        foreach(Tweet t in TweetHandler.tweets)
        {
            if(t.Likes.Any(u => u == username))
            {
                var i = TweetHandler.tweets.IndexOf(t);
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
        
        ShowTweets(TweetHandler.tweets, true);
        
        Console.WriteLine($"Välj tweet 1-{TweetHandler.tweets.Count}");
        int index = int.Parse(Console.ReadLine()) - 1;
        var tweetIndex = TweetHandler.tweets[index].Id;
        
        Console.Clear();
        while(true)
        {
            ShowTweets(TweetHandler.tweets[index]);
            
            Console.WriteLine($"1. Gilla 2. Kommentera 3. Retweet 4. Hem");
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
                    TweetHandler.Retweet(tweetIndex);
                    break;
                    
                case ConsoleKey.D4:
                    return;
            }
        }
    }
}