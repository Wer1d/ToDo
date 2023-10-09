using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using ToDo.Models;
using ToDo.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

builder.Services.AddControllers();

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("MariaDbConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnection"))));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>{
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo{
        Title = "ToDo API",
        Version = "v1"
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);


    option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme{
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        BearerFormat = "JWT",
        Scheme = "bearer",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http
    });
    option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme{
                Reference = new Microsoft.OpenApi.Models.OpenApiReference{
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "public",
        ValidIssuer = "ToDoApp",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecurityKey))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
        
    });
}

app.UseHttpsRedirection();
app.UseCors(Options => Options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();
// Program.Configure(app);
app.Run();

public partial class Program { // secretkey
   
    public static string SecurityKey = "7hR#2Lp$QW9*ZxYv3K@5sDfG6jM8tE1u";
    
    // public static void Configure(IApplicationBuilder app)
    // {
    //     using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
    //     {
    //         var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    //         dbContext.Database.Migrate();
    //     }
    // }
}
// check user authen by hash(password+salt) then check in database that does it related


