using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QTip.Application.Abstractions;
using QTip.Infrastructure.Persistence;
using QTip.Infrastructure.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR
builder.Services.AddMediatR(typeof(IApplicationDbContext).Assembly);

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(IApplicationDbContext).Assembly);

// DbContext
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? builder.Configuration["QTIP_CONNECTION_STRING"]
                      ?? "Host=localhost;Port=5432;Database=qtip;Username=qtip;Password=qtip";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

// Application services
builder.Services.AddScoped<IEmailDetectionService, EmailDetectionService>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

// CORS - relaxed for this challenge; can be tightened with specific origin later.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

// Ensure database is created with the current schema.
using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.MapControllers();

app.Run();
