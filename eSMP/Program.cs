using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using eSMP.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using eSMP.Services.BrandRepo;
using eSMP.Services.CategoryRepo;
using eSMP.Services.SpecificationRepo;
using eSMP.Services.StoreRepo;
using eSMP.Services.ItemRepo;
using eSMP.Services.UserRepo;
using eSMP.Services.TokenRepo;
using eSMP.Services.OrderRepo;
using eSMP.Services.ShipRepo;
using eSMP.Services.MomoRepo;
using eSMP.Services;
using eSMP.Services.StoreAssetRepo;
using eSMP.Services.ReportSaleRepo;
using eSMP.Services.ReportRepo;
using eSMP.Services.FileRepo;
using eSMP.Services.NotificationRepo;
using eSMP.Services.AddressRepo;
using eSMP.Services.StatusRepo;
using eSMP.Services.AutoService;
using eSMP.Services.DataExchangeRepo;
using eSMP.Services.AfterBuyServiceRepo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddTransient<WebContext>();

builder.Services.AddLazyResolution();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserReposity, UserRepository>();
builder.Services.AddScoped<IStoreReposity, StoreRepository>();
builder.Services.AddScoped<ICategoryReposity, CategoryRepository>();
builder.Services.AddScoped<ISpecificationReposity, SpecificationRepository>();
builder.Services.AddScoped<IItemReposity, ItemRepository>();
builder.Services.AddScoped<IBrandReposity, BrandRepository>();
builder.Services.AddScoped<IOrderReposity, OrderRepository>();
builder.Services.AddScoped<IShipReposity, ShipRepository>();
builder.Services.AddScoped<IMomoReposity, MomoRepository>();
builder.Services.AddScoped<IAssetReposity, AssetRepository>();
builder.Services.AddScoped<ISaleReportReposity, SaleReportRepository>();
builder.Services.AddScoped<IReportReposity, ReportRepository>();
builder.Services.AddScoped<IFileReposity, FileRepository>();
builder.Services.AddScoped<INotificationReposity, NotificationRepository>();
builder.Services.AddScoped<IAddressReposity, Addressrepository>();
builder.Services.AddScoped<IStatusReposity, StatusRepsitory>();
builder.Services.AddScoped<IDataExchangeReposity, DataExchangeRepository>();
builder.Services.AddScoped<IAfterBuyServiceReposity, AfterBuyServiceRepository>();

builder.Services.AddSingleton<IWorker, Worker>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var dbcontext = new WebContext();
dbcontext.Database.EnsureCreated();
/*
 dotnet tool install --global dotnet-ef
dotnet ef migrations add name
dotnet ef migrations list
dotnet ef migrations remove
donet ed database update
donet ed database drop
 */

//firebase
/*builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(@"firebase-config.json")
}));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (o) => { });*/

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(@"firebase-config.json")
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer("Firebase", opt =>
{
    opt.Authority = builder.Configuration["Jwt:Firebase:ValidIssuer"];
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Firebase:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:Firebase:ValidAudience"]
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("AuthDemo", opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:AuthDemo:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:AuthDemo:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AuthDemo:Key"]))
        };
    });

builder.Services.AddAuthorization(opt =>
{
    opt.DefaultPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes("Firebase", "AuthDemo")
    .RequireAuthenticatedUser()
    .Build();
});

builder.Services.AddHostedService<Processor>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Bearer xxxxxxxx",
        Name="Authorization",
        In=ParameterLocation.Header,
        Type=SecuritySchemeType.ApiKey,
        Scheme= "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
        new OpenApiSecurityScheme
        {
            Reference=new OpenApiReference
            {
                Type=ReferenceType.SecurityScheme,
                Id="Bearer"
            },
            Scheme="oauth2",
            Name="Bearer",
            In=ParameterLocation.Header,
        },
        new List<string>()
        }
    });
});
//add cors
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("corspolicy");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
