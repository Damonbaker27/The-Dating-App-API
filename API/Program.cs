using API.Data;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatingAppConnectionString"));
});

builder.Services.AddCors(options => options.AddPolicy("DefaultCors", policy =>
    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Middleware for building request pipeline.
app.UseHttpsRedirection();

//app.UseAuthorization();

app.UseCors("DefaultCors");

app.MapControllers();

app.Run();
