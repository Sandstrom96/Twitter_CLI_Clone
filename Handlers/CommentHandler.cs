class CommentHandler
{
    public static void AddComment(string commentContent, Tweet tweet)
    {
        var comment = new Comment(commentContent, UserCLI.loggedInUser.Username);
            
        tweet.Comments.Add(comment);
    }
}