namespace ChatApp.Application.Entities;

public class Message
{
    public int Id { get; set; }
    public int SenderId {get; set;}

    public int ConversationId {get; set;}
    public Conversation Conversation {get; set;}
    public string Text { get; set; }
}
