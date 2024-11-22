
static class TweetHandler
{
    public static List<Tweet> tweets = new List<Tweet>(); // Listan som innehåller hela programmets tweets

    // Sorterar tweets med hjälp av bubble sort
    // Ej effektiv men ville testa att vi fortfarande kunde
    public static void SortTweets()
    {
        for (int i = 0; i < tweets.Count; i++)
        {
            for (int j = 0; j < tweets.Count - 1; j++)
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

    public static void LikeUnlikeTweet(Guid id)
    {
        var tweet = GetTweet(id);

        if (tweet.IsRetweet)
        {
            // Om tweeten är en retweet, hämtar originalet och låter använadern gilla originalet
            var originalTweet = GetOriginalTweet(tweet);

            // Kollar om användaren redan gillat
            if (!originalTweet.Likes.Any(u => u == UserCLI.loggedInUser.Username))
            {
                originalTweet.Likes.Add(UserCLI.loggedInUser.Username);
            }
            else
            {
                originalTweet.Likes.Remove(UserCLI.loggedInUser.Username);
            }
        }
        else
        {
            if (!tweet.Likes.Any(u => u == UserCLI.loggedInUser.Username))
            {
                tweet.Likes.Add(UserCLI.loggedInUser.Username);
            }
            else
            {
                tweet.Likes.Remove(UserCLI.loggedInUser.Username);
            }
        }
    }

    public static void Retweet(Guid id)
    {
        var originalTweet = GetTweet(id);

        // Kollar om tweeten är en retweet, om true hämtar originalet
        if (originalTweet.IsRetweet)
        {
            originalTweet = tweets.FirstOrDefault(x => x.Id == originalTweet.OriginalTweetId);
        }

        // Kollar om användaren har retweetat tweeten
        var existingRetweet = tweets.FirstOrDefault(x => x.OriginalTweetId == originalTweet.Id && x.Author == UserCLI.loggedInUser.Username);

        // Om användaren har retweetat ta bort retweeten annars retweeta
        if (existingRetweet != null)
        {
            tweets.Remove(existingRetweet);
            UserCLI.loggedInUser.OwnTweets.Remove(existingRetweet.Id);
            originalTweet.Retweet.Remove(UserCLI.loggedInUser.Username);
        }
        else
        {
            var retweet = new Tweet(null) { OriginalTweetId = originalTweet.Id, IsRetweet = true };
            tweets.Add(retweet);
            UserCLI.loggedInUser.OwnTweets.Add(retweet.Id);
            originalTweet.Retweet.Add(UserCLI.loggedInUser.Username);
        }
    }

    public static List<Tweet> GetUserTweets(User user)
    {
        return tweets.Where(t => user.OwnTweets.Contains(t.Id)).ToList();
    }

    public static void AddTweet(string tweetContent)
    {
        var tweet = new Tweet(tweetContent);

        tweets.Add(tweet);

        UserCLI.loggedInUser.OwnTweets.Add(tweet.Id);
    }

    public static Tweet GetOriginalTweet(Tweet tweet)
    {
        return tweets.FirstOrDefault(x => x.Id == tweet.OriginalTweetId);
    }

    public static Tweet GetTweet(Guid id)
    {
        return tweets.FirstOrDefault(t => t.Id == id);
    }

    public static void RemoveTweet(Tweet tweet, List<Tweet> retweets)
    {
        tweets.Remove(tweet);
        UserCLI.loggedInUser.OwnTweets.Remove(tweet.Id);

        // Tar bort varje retweet hos alla användare och i listan med tweets
        foreach (var r in retweets)
        {
            UserCLI.loggedInUser.OwnTweets.Remove(r.Id);
        }

        foreach (Tweet t in retweets)
        {
            tweets.Remove(t);
        }

        foreach (var u in UserHandler.users)
        {
            foreach (var r in retweets)
            {
                if (u.OwnTweets.Contains(r.Id))
                {
                    u.OwnTweets.Remove(r.Id);
                }
            }
        }
    }
}