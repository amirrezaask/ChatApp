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
        public List<Conversation> Conversation { get; set; }
    }

    public class MessageDTO
    {
        public int SenderId { get; set; }
        public string SenderHandle {get; set;}
        public int ReceiverId { get; set; }
        public string ReceiverHandle { get; set; }

        public Conversation Conversation { get; set; }
        public string Text { get; set; }
    }
}



public class ChatHub(ChatDbContext _dbContext) : Hub
{
    private static Dictionary<string, int> connectionsToUserIds = new();
    public async Task Join(string handle)
    {

        var user = await _dbContext.Users.Where(u => u.Handle == handle).FirstOrDefaultAsync();
        if (user is null)
        {
            user = new User { Handle = handle, Status= User.Statuses.Online };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        user.Status = User.Statuses.Online;
        await _dbContext.SaveChangesAsync();

        connectionsToUserIds[Context.ConnectionId] = user.Id;



        var conversations = await _dbContext.Conversations.Where(c => c.User2Id == user.Id || c.User1Id==user.Id).ToListAsync();

        await Clients.Caller.SendAsync(Protocol.MessageTypes.UserInfo.ToString(), new Protocol.UserInfo { UserId = user.Id, Conversation = conversations });
    }
    public async Task NewMessage(Protocol.MessageDTO message)
    {
        var userId = new int();

        if (!connectionsToUserIds.TryGetValue(Context.ConnectionId, out userId))
            throw new Exception($"User not found for connection {Context.ConnectionId}");

        if (userId != message.SenderId)
            throw new Exception($"Unauthorized, userId: {userId}, message.SenderId:{message.SenderId}");

        var sender = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        if (sender is null) throw new Exception($"No user found with id {userId}");

        var receiver = await _dbContext.Users.Where(u => u.Handle == message.ReceiverHandle).FirstOrDefaultAsync();
        if (receiver is null) throw new Exception($"No user found with id {message.ReceiverHandle}");

        if (receiver.Id == message.SenderId)
            throw new Exception("Cannot send message to self");

        message.SenderId = sender.Id!;
        message.ReceiverId = receiver.Id!;

        var user1 = new int();
        var user2 = new int();

        if (sender.Id < receiver.Id)
        {
            user1 = sender.Id;
            user2 = receiver.Id;
        }
        else
        {
            user1 = receiver.Id;
            user2 = sender.Id;
        }

        var conversation = await _dbContext.Conversations.Where(c => c.User1Id == user1 && c.User2Id == user2).FirstOrDefaultAsync();
        if (conversation is null)
        {
            conversation = new Conversation
            {
                User1Id = user1,
                User2Id = user2,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _dbContext.Conversations.AddAsync(conversation);
            await _dbContext.SaveChangesAsync();
        }

        await _dbContext.Messages.AddAsync(new Message
        {
            SenderId = sender.Id,
            ConversationId = conversation.Id,
            Text = message.Text
        });

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
