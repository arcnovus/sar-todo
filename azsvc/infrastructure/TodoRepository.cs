using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Ec.Sar.Common.Domain;
using Ec.Sar.TodoDemo.Domain;

namespace Ec.Sar.TodoDemo.Infrastructure
{
  // TODO: Wrap Mongo errors to avoid leaking implementation details.
  public class TodoRepository : ITodoRepository
  {
    private IMongoCollection<BsonDocument> _todoDb;
    public TodoRepository(IMongoCollection<BsonDocument> todoDb)
    {
      _todoDb = todoDb;
    }

    public async Task<List<ITodo>> FindAll(bool includeInactive = false)
    {
      var todoList = new List<ITodo>();
      var filter = includeInactive ?
                      new BsonDocument() :
                      new BsonDocument { { "isActive", true } };

      await _todoDb
                .Find(filter)
                .SortByDescending(todo => todo["createdAt"])
                .ForEachAsync(todo => todoList.Add(FromBson(todo)));

      return todoList;
    }

    public ITodo FindById(Id id)
    {
      var todo = _todoDb.Find(new BsonDocument {
                                    { "_id", id.ToString() }
                                  }).First();

      return FromBson(todo);
    }

    public ITodo Insert(ITodo todo)
    {
      _todoDb.InsertOne(new BsonDocument {
        {"_id", todo.Id.ToString()},
        {"title", todo.Title.ToString()},
        { "completed", todo.Completed.ToBoolean()},
        {"isActive", todo.Active.ToBoolean() },
        {"createdAt", todo.CreatedAt.ToLong()},
        {"modifiedAt", todo.ModifiedAt.ToLong()},
        // BUG: MongoDB driver doesn't like decimals, 
        // so use a string. ref: https://jira.mongodb.org/browse/CSHARP-196
        {"version", todo.Version.ToString()}
      });

      return FindById(todo.Id);
    }

    public ITodo Update(ITodo todo)
    {
      var idFilter = Builders<BsonDocument>.Filter.Eq("_id", todo.Id.ToString());
      // Optimistic concurrency.
      var versionFilter = Builders<BsonDocument>.Filter.Eq("version", todo.Version.ToString());
      var filter = Builders<BsonDocument>.Filter.And(idFilter, versionFilter);

      var update = Builders<BsonDocument>.Update
                                        .Set("_id", todo.Id.ToString())
                                        .Set("title", todo.Title.ToString())
                                        .Set("completed", todo.Completed.ToBoolean())
                                        .Set("isActive", todo.Active.ToBoolean())
                                        .Set("createdAt", todo.CreatedAt.ToLong())
                                        .Set("modifiedAt", todo.ModifiedAt.ToLong())
                                        // BUG: MongoDB driver doesn't like decimals, 
                                        // so use a string https://jira.mongodb.org/browse/CSHARP-196
                                        .Set("version", AggregateVersion.Next().ToString());

      var options = new FindOneAndUpdateOptions<BsonDocument>
      {
        ReturnDocument = ReturnDocument.After,
        IsUpsert = false
      };

      var result = _todoDb.FindOneAndUpdate(
          filter,
          update,
          options);

      return FromBson(result);
    }

    // TODO: Optimistic concurrency.
    public void Delete(Id id)
    {
      _todoDb.DeleteOne(Builders<BsonDocument>.Filter.Eq("_id", id.ToString()));
    }

    // TODO: Use a mapper.
    private ITodo FromBson(BsonDocument todo)
    {
      return Todo.Of(
                     Id.Of(todo["_id"].AsString),
                     Title.Of(todo["title"].AsString),
                     CompletedFlag.Of(todo["completed"].AsBoolean),
                     Timestamp.Of(todo["createdAt"].AsInt64),
                     Timestamp.Of(todo["modifiedAt"].AsInt64),
                     ActiveFlag.Of(todo["isActive"].AsBoolean),
                     AggregateVersion.Of(todo["version"].ToDecimal())
                   );
    }
  }
}
