using System.Text;
using HospEaseHMS.Data;
using HospEaseHMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Org.BouncyCastle.Pqc.Crypto.Lms;

//builder pattern
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//add db connection string
builder.Services.AddDbContext<HospitalDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

//service should be registered
//dependency injection pattern
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();

//jwt
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]); //secure key 
//make sure the format Jwt:Key need to be same in appsettings.json
builder.Services.AddAuthentication(options=>
{
    //strategy pattern - allows different auth schemes to be configured interchangeably
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//authorization
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HospEaseHMS API",
        Version = "v1",
        Description = "ASP.NET Core Web API with JWT Authentication"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


//chatbot
//singleton pattern
builder.Services.AddHttpClient<WebSearchService>();

//cross origin resource sharing for angular 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy => policy.WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod());
});


var app = builder.Build();

//add authentication and authorization for jwt
//decorator pattern - each middleware adds functionality to the pipeline ensuring dynamic changes
app.UseAuthentication();
app.UseAuthorization();

//add routing for chatbot
app.UseRouting();

//use cors
app.UseCors("AllowAngular");

// Configure the HTTP request pipeline.
//middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
