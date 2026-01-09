using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using IncidentManagement.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var sqlConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("SQL_CONNECTION") ?? "Server=db,1433;Database=IncidentsDb;User Id=sa;Password=Your_password123;";
var storageConn = builder.Configuration["AzureStorageConnectionString"] ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? "UseDevelopmentStorage=true";

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(sqlConnection));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new BlobServiceClient(storageConn));
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// Ensure DB created (for prototype)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
