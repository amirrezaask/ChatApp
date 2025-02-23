using Microsoft.AspNetCore.SignalR;

public class MonitoringHub(ChatDbContext _dbContext) : Hub
{
    public async Task GetOnlineUsers()
    {
        var users = await _dbContext.Users.ToListAsync();
        await Clients.All.SendAsync("Users", users);
    }
}
