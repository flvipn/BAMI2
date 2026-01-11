
using GrpcCustomersService;
using Microsoft.EntityFrameworkCore;
using Iepan_Flaviu_Lab2.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<LibraryContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryContext")));
builder.Services.AddGrpc();
var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcCRUDService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");
app.Run();