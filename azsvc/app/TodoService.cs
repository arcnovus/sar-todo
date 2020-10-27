using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ec.Sar.Common.Domain;
using Ec.Sar.TodoDemo.Domain;
using Microsoft.Extensions.Logging;


namespace Ec.Sar.TodoDemo.App
{
  public interface ITodoRepository
  {
    Task<List<ITodo>> FindAll(bool includeInactive = false);
    ITodo FindById(Id id);
    ITodo Insert(ITodo todo);
    ITodo Update(ITodo todo);
    void Delete(Id id);
  }

  public interface ITodoResource
  {
    string id { get; set; }
    string title { get; set; }
    bool? completed { get; set; }
  }

  public class TodoService : ITodoService
  {
    ITodoRepository _repo;
    // TODO: Dependency Injection
    public TodoService(ITodoRepository repo)
    {
      this._repo = repo;
    }
    public async Task<List<ITodoResource>> ListAllTodos()
    {
      var allTodos = await this._repo.FindAll();
      return allTodos.Select(todo =>
          new TodoResource()
          {
            id = todo.Id.ToString(),
            title = todo.Title.ToString(),
            completed = todo.Completed.ToBoolean()
          }
        ).ToList<ITodoResource>();
    }
    public ITodoResource RecordTodo(ITodoResource todoInfo)
    {
      var newTodo = Todo.Record(Id.Of(todoInfo.id), Title.Of(todoInfo.title));
      var inserted = this._repo.Insert(newTodo);
      return ToResource(inserted);
    }
    public void CancelTodo(string id)
    {
      var todo = this._repo.FindById(Id.Of(id));
      todo.Cancel();
      this._repo.Update(todo);
    }

    public ITodoResource ToggleTodo(ITodoResource todoInfo)
    {
      var todo = this._repo.FindById(Id.Of(todoInfo.id));

      var title = todoInfo.title == null ? todo.Title : Title.Of(todoInfo.title);
      var completed = CompletedFlag.Of(todoInfo.completed);
      todo.SetCompletionState(completed, title);

      this._repo.Update(todo);
      return ToResource(todo);
    }
    public ITodoResource RenameTodo(ITodoResource todoInfo)
    {
      var todo = this._repo.FindById(Id.Of(todoInfo.id));

      var title = Title.Of(todoInfo.title);
      var completed = CompletedFlag.Of(todoInfo.completed);
      todo.Rename(title, completed);

      this._repo.Update(todo);
      return ToResource(todo);
    }
    private ITodoResource ToResource(ITodo todo)
    {
      return new TodoResource()
      {
        id = todo.Id.ToString(),
        title = todo.Title.ToString(),
        completed = todo.Completed.ToBoolean()
      };
    }
  }
}