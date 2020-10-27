using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http;

namespace Ec.Sar.TodoDemo.App
{
  public interface ITodoService
  {
    void CancelTodo(string id);
    Task<List<ITodoResource>> ListAllTodos();
    ITodoResource RecordTodo(ITodoResource todoInfo);
    ITodoResource RenameTodo(ITodoResource todoInfo);
    ITodoResource ToggleTodo(ITodoResource todoInfo);
  }

  // TODO: Better Error Handling/Logging.
  // TODO: Auth
  public class TodoController
  {
    private ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
      _todoService = todoService;
    }

    [FunctionName("GetTodoList")]
    public async Task<IActionResult> GetTodoList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a GET request.");

      try
      {
        List<ITodoResource> todoList = await _todoService.ListAllTodos();
        return (ActionResult)new OkObjectResult(todoList);
      }
      catch (Exception ex)
      {
        return HandleError(ex, log);
      }
    }

    [FunctionName("PostTodo")]
    public async Task<IActionResult> PostTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todos")] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a POST request.");

      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        var newTodo = _todoService.RecordTodo(ToResource(data));

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
    public async Task<IActionResult> PatchTodoTitle(
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

        var updated = _todoService.RenameTodo(toRename);

        return (ActionResult)new OkObjectResult(updated);
      }
      catch (Exception ex)
      {
        return HandleError(ex, log);
      }
    }

    [FunctionName("PatchTodoCompletion")]
    public async Task<IActionResult> PatchTodoCompletion(
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

        var updated = _todoService.ToggleTodo(toToggle);

        return (ActionResult)new OkObjectResult(updated);
      }
      catch (System.Exception ex)
      {
        return HandleError(ex, log);
      }
    }

    [FunctionName("DeleteTodo")]
    public IActionResult DeleteTodo(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todos/{id}")] HttpRequest req,
    ILogger log,
    string id)
    {
      log.LogInformation("C# HTTP trigger function processed a DELETE request.");

      try
      {
        _todoService.CancelTodo(id);
        return (ActionResult)new OkResult();
      }
      catch (Exception ex)
      {
        return HandleError(ex, log);
      }
    }

    private ITodoResource ToResource(dynamic data)
    {
      return new TodoResource()
      {
        id = data.id,
        title = data.title,
        completed = data.completed
      };
    }

    // TODO: Move this and make it better (proper logging, etc...).
    private IActionResult HandleError(Exception ex, ILogger log)
    {
      log.LogError(ex, ex.Message);

      if (ex.GetType() == typeof(System.ComponentModel.DataAnnotations.ValidationException))
      {
        return (ActionResult)new BadRequestObjectResult(
          new { error = ex.Message }
        );
      }

      if (ex.Message.Contains("E11000"))
      {
        return (ActionResult)new BadRequestObjectResult(
          new { error = "Duplicate." }
        );
      }

      return (ActionResult)new InternalServerErrorResult();
    }
  }
}