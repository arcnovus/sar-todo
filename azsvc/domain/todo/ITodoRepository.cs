using System.Collections.Generic;
using System.Threading.Tasks;
using Ec.Sar.Common.Domain;

namespace Ec.Sar.TodoDemo.Domain
{
  public interface ITodoRepository
  {
    Task<List<ITodo>> FindAll(bool includeInactive = false);
    ITodo FindById(Id id);
    ITodo Insert(ITodo todo);
    ITodo Update(ITodo todo);
    void Delete(Id id);
  }
}