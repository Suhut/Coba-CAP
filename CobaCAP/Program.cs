using DotNetCore.CAP;

var builder = WebApplication.CreateBuilder(args);

var capConnString = builder.Configuration.GetConnectionString("CapConnection");

builder.Services.AddCap(x =>
{
    x.UseSqlServer(capConnString);
    x.UseRabbitMQ("localhost");
    //x.UseRabbitMQ(h =>
    //{
    //    h.HostName = "localhost";
    //    h.Port = 15672;
    //    h.UserName = "guest";
    //    h.Password = "guest";
    //});

    x.UseDashboard();
    x.FailedRetryCount = 5;
    x.ConsumerThreadCount = 1;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

app.UseCapDashboard();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
