public class Tweet 
{
    public Guid Id {get; set;}
    public string Content {get; set;}
    public string Author {get; set;}
    public DateTime Date {get; set;}
    public List<string> Likes {get; set;} = new List<string>();
    public List<Guid> Retweet {get; set;} = new List<Guid>();
    public List<Comment> Comments {get; set;} = new List<Comment>();
    public bool IsRetweet {get; set;} = false;
    public Guid OriginalTweetId {get; set;}

    public Tweet (string content, string author)
    {
        Id = Guid.NewGuid();
        Content = content;
        Author = author;
        Date = DateTime.Now;
    }
}