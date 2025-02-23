using Blazorise;
using ChatApp.Application.Endpoints.Users;
using ChatApp.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using ChatApp.Application.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services.AddSignalR();


builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddDbContext<ChatDbContext>(options => 
{
    if (builder.Configuration.GetSection("Database")["Type"] == "Sqlite")
        options.UseSqlite("Data Source=Chat.db");
    else if (builder.Configuration.GetSection("Database")["Type"] == "InMemory")
        options.UseInMemoryDatabase("Chat");
});

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline (middlewares).
app.UseExceptionHandler();
app.MapOpenApi(); // SwaggerUI
app.MapGroup("/api").MapUserEndpoints(); //APIs
app.MapHub<ChatHub>("/chathub"); //SignalR
app.MapStaticAssets();
app.UseAntiforgery();
app.MapRazorComponents<ChatApp.Application.Components.App>().AddInteractiveServerRenderMode();
app.MapInfraDefaultEndpoints();
app.Run();





public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations {get; set;}

}
