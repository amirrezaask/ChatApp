using ChatApp.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Endpoints.Users;


static class UsersEndpoints
{

    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder router) 
    {
        router.MapGroup("/users")
            .MapGet("/online", ListOnlineUsers);

        return router;
    }



    public static async Task<List<User>> ListOnlineUsers(ChatDbContext _context) 
    {
        return await _context.Users.Where(u => u._Status == User.Status.Online).ToListAsync();
    }

}
