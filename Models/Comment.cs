public class Comment
{
    public string Content { get; set; }
    public string Author { get; set; }
    public DateTime Timestamp { get; set; }

    public Comment(string content, string author)
    {
        Content = content;
        Author = author;
        Timestamp = DateTime.Now;
    }
}