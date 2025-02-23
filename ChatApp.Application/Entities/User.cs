namespace ChatApp.Application.Entities;
public class User
{

    public enum Statuses
    {
        Online,
        Offline
    }
    public int Id { get; set; }
    public string Handle { get; set; }    
    public Statuses Status { get; set; }
}
