using eSMPWebAPI.Authentication;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using eSMP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Google.Apis.Auth.OAuth2;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<WebContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"));
});
/*
 dotnet tool install --global dotnet-ef
dotnet ef migrations add name
dotnet ef migrations list
dotnet ef migrations remove
donet ed database update
donet ed database drop
 */

builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(@"firebase-config.json")
}));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (o) => { });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
