namespace ChatApp.Application.Hubs.MonitoringHub;

using Blazorise;
using ChatApp.Application.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Protocol;


public class Protocol
{
    public enum MessageTypes
    {
        Join,
        Overview
    }
    public record Overview
    {
        public List<User> Users { get; set; }
        public List<Conversation> Conversations { get; set; }
        public List<Message> Messages { get; set; }
    }
}


public class MonitoringHub(ChatDbContext _dbContext) : Hub
{
    public async Task<Overview> GetOverviewAsync()
    {
        var users = await _dbContext.Users.ToListAsync();
        var conversations = await _dbContext.Conversations.ToListAsync();
        var messages = await _dbContext.Messages.ToListAsync();

        return new Overview
        {
            Users = users,
            Conversations = conversations,
            Messages = messages
        };
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        await Clients.Caller.SendAsync(Protocol.MessageTypes.Overview.ToString(), await GetOverviewAsync());
    }


    public async Task Join()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));


        while (await timer.WaitForNextTickAsync())
        {
            var overview = await GetOverviewAsync();

            Console.WriteLine($"#Users: {overview.Users.Count} #Conversations: {overview.Conversations.Count} #Messages: {overview.Messages.Count}");
            await Clients.Caller.SendAsync(Protocol.MessageTypes.Overview.ToString(), overview);
        }
    }
}
