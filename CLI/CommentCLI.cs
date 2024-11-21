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
    public static void RemoveComment(Tweet tweet)
    {
        var ownComment = CommentHandler.GetOwnComments(tweet);
        string choice;
        if(tweet.Author == UserCLI.loggedInUser.Username)
        {
            ShowComment(tweet,true);
            Console.WriteLine("Tryck esc för att gå tillbaka");
            Console.WriteLine($"Välj vilken du vill radera (1-{tweet.Comments.Count})");
        }
        else 
        {
            CommentHandler.GetOwnComments(tweet);
            foreach(var c in tweet.Comments)
        {
            var i = ownComment.IndexOf(c);
            
            if (c.Author == UserCLI.loggedInUser.Username)
            {
                Console.Write($"{i + 1}. ");
            }

            Console.WriteLine("--------------------"); 
            Console.WriteLine($"{c.Content}"); 
            Console.WriteLine($"{c.Author} {c.Timestamp:MM-dd HH:mm}");

        }
            Console.WriteLine("Tryck esc för att gå tillbaka");
            Console.WriteLine($"Välj vilken du vill radera (1-{ownComment.Count})");
        }
        
        while (true)
        {   
                         
            choice = Helpers.ReadUserInput();

            if (choice == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(choice))
            {
                Helpers.ShowErrorMessage("Inmatingen får inte vara tom");
                continue;  
            }
            
            if (choice.All(char.IsDigit))
            {
                int choiceValue = int.Parse(choice);

                // Kontrollera om siffran är inom listans längd
                if (choiceValue > 0 && (choiceValue < tweet.Comments.Count || choiceValue < ownComment.Count))
                {
                    break;
                }
                else
                {
                    Helpers.ShowErrorMessage("Fel inmatning, försök igen!");
                }
            }
            else
            {
                
                Helpers.ShowErrorMessage("Endast siffror är tillåtna, försök igen!");
            }            
        }
        
        
        int index = int.Parse(choice);
        var chosenComment = CommentHandler.GetCommentFromIndex(index,tweet, ownComment);
        
        Console.WriteLine("Du vill ta bort meddelandet:");
        Console.WriteLine("--------------------");
        Console.WriteLine(chosenComment.Content);
        Console.WriteLine(chosenComment.Author);
        Console.WriteLine(chosenComment.Timestamp);
        Console.WriteLine(""); 
        
        Console.WriteLine("1. Radera");
        Console.WriteLine("Tryck Esc för att avbryta"); 
        while (true)
        {
            var input = Console.ReadKey(true).Key;

            switch(input)
            {
                case ConsoleKey.D1:
                tweet.Comments.Remove(chosenComment); 
                return;

                case ConsoleKey.Escape: 
                return; 

            }
        }
    }
}