public class Tweet
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public DateTime Date { get; set; }
    public List<string> Likes { get; set; } = new List<string>();
    public List<string> Retweet { get; set; } = new List<string>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public bool IsRetweet { get; set; } = false;
    public Guid OriginalTweetId { get; set; }

    public Tweet(string content)
    {
        Id = Guid.NewGuid();
        Content = content;
        Author = UserCLI.loggedInUser.Username;
        Date = DateTime.Now;
    }
}