public static class Buttonhandler
{
    public static string FollowButton(User user)
    {
        User chosenUser = UserHandler.users.FirstOrDefault(u => u == user);
        if (!chosenUser.Followers.Any(u => u.Username == UserCLI.loggedInUser.Username))
        {
            return "Följ";
        }
        else
        {
            return "Avfölj";
        }
        
    }
        public static string LikeButton(Tweet tweet)
    {
        if (!tweet.Likes.Any(u => u == UserCLI.loggedInUser.Username))
        {
            return "🤍";
        }
        else
        {
            return "❤️";
        }
    }

    public static string RetweetButton(Tweet tweet)
    {
        if (!tweet.Retweet.Any(u => u == UserCLI.loggedInUser.Username))
        {
            return "🔁";
        }
        else
        {
            return "↩️";
        }
    }
}