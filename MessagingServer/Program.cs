using MessagingServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.UseCors(builder =>
{
    builder.WithOrigins(Environment.GetEnvironmentVariable("CLIENT_URL") ?? "") //Source
        .AllowAnyHeader()
        .WithMethods("GET", "POST")
        .AllowCredentials();

    builder.WithOrigins("https://localhost:7032") //Source
        .AllowAnyHeader()
        .WithMethods("GET", "POST")
        .AllowCredentials();
});

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MessagingHub>("/messagingHub");
});

app.Run();
