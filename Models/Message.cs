public class Message 
{
    public string Text { get; set; }
    public string Sender { get; set; }
    public string Receiver { get; set; }
    public DateTime Date { get; set; }
    public bool Isread { get; set; } = false; 

    public Message(string text, string receiver, string sender)
    {
        Text = text; 
        Sender = sender;
        Receiver = receiver;
        Date = DateTime.Now;
        
    }
}