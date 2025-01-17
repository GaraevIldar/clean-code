using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<HelperService>();

var app = builder.Build();

app.UseStaticFiles();  

app.UseRouting();
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();