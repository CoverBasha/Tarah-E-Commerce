using Tarah.API.Data;
using Microsoft.EntityFrameworkCore;
using Tarah.API.Repositories;
using Tarah.API.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tarah.API.Middleware;
using Tarah.API.Services;
using MassTransit;
using Microsoft.OpenApi.Models;
using Hangfire;
using Tarah.API.Services.Jobs;
using Hangfire.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TarahDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<DeletedUsersCleanupJob>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("Dev", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<ICategoriesRepository,CategoriesRepository>();
builder.Services.AddScoped<ICartsRepository, CartsRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<ILocalUsersRepository, LocalUserRepository>();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<CheckoutTransaction>();
builder.Services.AddScoped<ProductsService>();
builder.Services.AddScoped<CategoriesService>();
builder.Services.AddScoped<ProfilesService>();
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<CartsService>();

builder.Services.AddScoped<HttpClient>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();
    x.AddConsumer<UserDeletedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Dev");
}

var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
    OperatingSystem.IsWindows()
        ? "Egypt Standard Time"
        : "Africa/Cairo");

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate(
        "deleted-users-cleanup",
        Job.FromExpression<DeletedUsersCleanupJob>(
            job => job.ExecuteAsync()),
        Cron.Daily(0, 0),
        new RecurringJobOptions
        {
            TimeZone = timeZone
        });
}


app.UseHangfireDashboard("/hangfire");

app.UseMiddleware<GlobalExceptionHandler>();

app.UseMiddleware<CheckUserExists>();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
