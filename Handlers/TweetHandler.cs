using System.Text;
static class TweetHandler
{
    public static List<Tweet> tweets = new List<Tweet> ();
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
    
    public static void LikeUnlikeTweet(Guid id)
    {
        var tweet = tweets.FirstOrDefault(t => t.Id == id);
        
        if (tweet.IsRetweet)
        {
            var originalTweet = tweets.FirstOrDefault(t => t.Id == tweet.OriginalTweetId);

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
        var originalTweet = tweets.FirstOrDefault(x => x.Id == id);
        
        // Kollar om det är en retweet, om true hämtar originalet
        if (originalTweet.IsRetweet)
        {
            originalTweet = tweets.FirstOrDefault(x => x.Id == originalTweet.OriginalTweetId);
        }
        

        var existingRetweet = tweets.FirstOrDefault(x => x.OriginalTweetId == originalTweet.Id && x.Author == UserCLI.loggedInUser.Username);


        if (existingRetweet != null)
        {
            tweets.Remove(existingRetweet);
            UserCLI.loggedInUser.OwnTweets.Remove(existingRetweet.Id);
            originalTweet.Retweet.Remove(UserCLI.loggedInUser.Username);
        }
        else
        {
            var retweet = new Tweet(null, UserCLI.loggedInUser.Username) {OriginalTweetId = originalTweet.Id, IsRetweet = true};
            tweets.Add(retweet);
            UserCLI.loggedInUser.OwnTweets.Add(retweet.Id);
            originalTweet.Retweet.Add(UserCLI.loggedInUser.Username);
        }
    }

    public static List<Tweet> GetUserTweets(User user)
    {
        return tweets.Where(t => user.OwnTweets.Contains(t.Id)).ToList();
    }
}