using System;
using Ec.Sar.TodoDemo.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

[assembly: WebJobsStartup(typeof(Ec.Sar.TodoDemo.App.Startup))]
namespace Ec.Sar.TodoDemo.App
{
  public class Startup : IWebJobsStartup
  {
    public void Configure(IWebJobsBuilder builder)
    {

      builder.Services.AddSingleton<ITodoRepository>((s) =>
      {
        var dbClient = new MongoClient("mongodb://localhost:27017/?readPreference=primary&ssl=false");
        var db = dbClient.GetDatabase("local");
        var todoDb = db.GetCollection<BsonDocument>("todos");
        return new TodoRepository(todoDb);
      });

      builder.Services.AddScoped<ITodoService>((s) =>
      {
        ITodoRepository todoRepo = s.GetRequiredService<ITodoRepository>();
        return new TodoService(todoRepo);
      });
    }
  }
}