
namespace Ec.Sar.TodoDemo.App
{
  public class TodoResource : ITodoResource
  {
    public string id { get; set; }
    public string title { get; set; }
    public bool? completed { get; set; }
  }
}