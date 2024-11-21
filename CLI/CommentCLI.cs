class CommentCLI
{
    public static void CommentTweet(Tweet tweet)
    {
        Console.Clear();
        TweetCLI.ShowTweets(tweet);
        Console.WriteLine("Tryck esc för att avbryta");
        Console.WriteLine("Skriv din kommentar:");
        
        
        string commentContent = Helpers.ReadUserInput();

        if (commentContent == null)
        {
            return;
        }

        CommentHandler.AddComment(commentContent, tweet);
    }
    public static void ShowComment (Tweet tweet, bool showIndex)
    {
        foreach(var c in tweet.Comments)
        {
            var i = tweet.Comments.IndexOf(c);

            if (showIndex)
            {
                Console.Write($"{i + 1}. ");
            }

            Console.WriteLine("--------------------"); 
            Console.WriteLine($"{c.Content}"); 
            Console.WriteLine($"{c.Author} {c.Timestamp:MM-dd HH:mm}");
        }
    }
}