public class User
{
    public string Username {get; set;}
    public string Password {get; set;}
    public string Name {get; set;}
    public List<User> Followers {get; set;} = new List<User>();
    public List<User> Following {get; set;} = new List<User>();
    public List<Guid> OwnTweets {get; set;} = new List<Guid>();
    public List<Message> Messages {get; set;} = new List<Message>();
    public User(string username, string password, string name)
    {
        Username = username;
        Password = password;
        Name = name;
    }
}