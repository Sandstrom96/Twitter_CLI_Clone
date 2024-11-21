class CommentHandler
{
    public static void AddComment(string commentContent, Tweet tweet)
    {
        var comment = new Comment(commentContent, UserCLI.loggedInUser.Username);
            
        tweet.Comments.Add(comment);
    }

    public static List<Comment> GetOwnComments(Tweet tweet)
    {
        return tweet.Comments.Where(c => c.Author == UserCLI.loggedInUser.Username).OrderBy(c => c.Timestamp).ToList();
    }

    public static Comment GetCommentFromIndex(int index, Tweet tweet, List<Comment> ownComment)
    {
        
        if(tweet.Author == UserCLI.loggedInUser.Username)
        {
            return tweet.Comments[index -1];
        }
        else
        {
            return ownComment[index -1];
        }
    }
}