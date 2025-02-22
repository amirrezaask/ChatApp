using ChatApp.Application.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Hubs;

public static class Protocol
{
    public enum MessageTypes
    {
        Join,
        UserInfo,
        NewMessage,
        IsTyping,
        Online,
        Offline
    }



    public class UserInfo
    {
        public int UserId { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}



public class ChatHub(ChatDbContext _dbContext) : Hub
{
    private static Dictionary<string, int> connectionsToUserIds = new();
    public async Task Join(string handle)
    {

        var userId = await _dbContext.Users.Where(u => u.Handle == handle).Select(u => u.Id).FirstOrDefaultAsync();
        if (userId == 0)
        {
            var user = new User { Handle = handle };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            userId = user.Id;
        }

        connectionsToUserIds[Context.ConnectionId] = userId;

        var messages = await _dbContext.Messages.ToListAsync();
        await Clients.Caller.SendAsync(Protocol.MessageTypes.UserInfo.ToString(), new Protocol.UserInfo { UserId = userId, Messages = messages });
    }
    public async Task NewMessage(Message message)
    {
        var userId = new int();
        
        if (!connectionsToUserIds.TryGetValue(Context.ConnectionId, out userId))
            throw new Exception($"User not found for connection {Context.ConnectionId}");
        
        var handle = await _dbContext.Users.Where(u => u.Id == userId).Select(u => u.Handle).FirstOrDefaultAsync();
        if (handle == "" || handle is null) throw new Exception($"No handle found for user with id {userId}");

        message.SenderHandle = handle!;

        await _dbContext.Messages.AddAsync(message);

        await _dbContext.SaveChangesAsync();

        await Clients.All.SendAsync(Protocol.MessageTypes.NewMessage.ToString(), message);
    }


    public async Task Online(string handle)
    {
        await Clients.All.SendAsync(Protocol.MessageTypes.Online.ToString(), handle);
    }

    public async Task Offline(string handle)
    {
        await Clients.All.SendAsync(Protocol.MessageTypes.Offline.ToString(), handle);
    }
}
