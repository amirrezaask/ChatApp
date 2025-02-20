namespace ChatApp.Application.Entities;
public class User
{

    public enum Status
    {
        Online,
        Offline
    }
    public int Id { get; set; }
    public string Handle { get; set; }    
    public Status _Status { get; set; }
}
