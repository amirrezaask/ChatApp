var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapStaticAssets();

app.UseAntiforgery();

app.MapRazorComponents<ChatApp.Application.Components.App>()
    .AddInteractiveServerRenderMode();


app.MapDefaultEndpoints();

app.Run();
