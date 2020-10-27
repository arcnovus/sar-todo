using System;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Ec.Sar.TodoDemo.Infrastructure;
using System.Web.Http;

namespace Ec.Sar.TodoDemo.App
{
  // TODO: Better Error Handling.
  // TODO: Use DocumentClient
  public static class TodoController
  {
    private static Lazy<TodoService> lazyTodoService = new Lazy<TodoService>(initTodoService);
    private static TodoService todoService = lazyTodoService.Value;
    private static TodoService initTodoService()
    {
      // TODO: Switch to Azure DocumentClient
      var dbClient = new MongoClient("mongodb://localhost:27017/?readPreference=primary&ssl=false");
      var db = dbClient.GetDatabase("local");
      var todoDb = db.GetCollection<BsonDocument>("todos");
      var todoRepo = new TodoRepository(todoDb);
      return new TodoService(todoRepo);
    }

    [FunctionName("GetTodoList")]
    public static async Task<IActionResult> GetTodoList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a GET request.");

      try
      {
        List<ITodoResource> todoList = await todoService.ListAllTodos();
        return (ActionResult)new OkObjectResult(todoList);
      }
      catch (Exception ex)
      {
        return HandleError(ex, log);
      }
    }

    [FunctionName("PostTodo")]
    public static async Task<IActionResult> PostTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todos")] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a POST request.");


      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        var newTodo = todoService.RecordTodo(ToResource(data));

        return (ActionResult)new CreatedResult(
            new Uri(String.Concat("/api/todos/", newTodo.id), UriKind.Relative),
            newTodo
          );
      }
      catch (Exception ex)
      {
        return HandleError(ex, log);
      }

    }
    [FunctionName("PatchTodoTitle")]
    public static async Task<IActionResult> PatchTodoTitle(
    [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "todos/{id}/title")] HttpRequest req,
    ILogger log,
    string id)
    {
      log.LogInformation("C# HTTP trigger function processed a PATCH title request.");
      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        var toRename = ToResource(data);
        toRename.id = id;
        var updated = todoService.RenameTodo(toRename);

        return (ActionResult)new OkObjectResult(updated);
      }
      catch (Exception ex)
      {
        return HandleError(ex, log);
      }
    }

    [FunctionName("PatchTodoCompletion")]
    public static async Task<IActionResult> PatchTodoCompletion(
    [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "todos/{id}/completion")] HttpRequest req,
    ILogger log,
    string id)
    {
      log.LogInformation("C# HTTP trigger function processed a PATCH completion request.");
      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        var toToggle = ToResource(data);
        toToggle.id = id;
        var updated = todoService.ToggleTodo(toToggle);

        return (ActionResult)new OkObjectResult(updated);
      }
      catch (System.Exception ex)
      {
        return HandleError(ex, log);
      }

    }

    [FunctionName("DeleteTodo")]
    public static IActionResult DeleteTodo(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todos/{id}")] HttpRequest req,
    ILogger log,
    string id)
    {
      log.LogInformation("C# HTTP trigger function processed a DELETE request.");
      try
      {
        todoService.CancelTodo(id);
        return (ActionResult)new OkResult();
      }
      catch (Exception ex)
      {
        return HandleError(ex, log);
      }
    }
    private static ITodoResource ToResource(dynamic data)
    {
      return new TodoResource()
      {
        id = data.id,
        title = data.title,
        completed = data.completed
      };
    }
    // TODO: Move this and make it better (proper logging, etc...).
    private static IActionResult HandleError(Exception ex, ILogger log)
    {
      log.LogError(ex,ex.Message);
      if(ex.GetType() == typeof(System.ComponentModel.DataAnnotations.ValidationException)) {
        return (ActionResult)new BadRequestObjectResult(new { error = ex.Message });
      }
      if (ex.Message.Contains("E11000"))
      {
        return (ActionResult)new BadRequestObjectResult(new { error = "Duplicate." });
      }
      else
      {
        return (ActionResult)new InternalServerErrorResult();
      }
    }
  }
}