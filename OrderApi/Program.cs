using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Infrastructure;
using Prometheus;
using SharedModels;

var builder = WebApplication.CreateBuilder(args);

string rabbitmqConnectionString = "host=rabbitmq";

// Add services to the container.

builder.Services.AddDbContext<OrderApiContext>(opt => opt.UseInMemoryDatabase("OrdersDb"));

// Register repositories for dependency injection
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();

// Register database initializer for dependency injection
builder.Services.AddTransient<IDbInitializer, DbInitializer>();

builder.Services.AddSingleton<IMessagePublisher>(new MessagePublisher(rabbitmqConnectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Initialize the database.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<OrderApiContext>();
    //var dbInitializer = services.GetService<IDbInitializer>();
    //dbInitializer.Initialize(dbContext);
}

Task.Factory.StartNew(() =>
    new MessageListener(app.Services, rabbitmqConnectionString).Start());

//app.UseHttpsRedirection();
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

app.Run();
