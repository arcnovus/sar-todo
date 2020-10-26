using System;
using Ec.Sar.Common.Domain;

namespace Ec.Sar.TodoDemo.Domain
{
  public interface ITodo : IAggregate
  {
    Title Title { get; }
    CompletedFlag Completed { get; }
    void Cancel();
    void SetCompletionState(CompletedFlag completed, Title title = null);
    void MarkComplete(Title title = null);
    void MarkIncomplete(Title title = null);
    void Rename(Title newName, CompletedFlag completed = null);
  }

  public class Todo : ITodo
  {
    private Id _id;
    private Title _title;
    private CompletedFlag _completed;
    private ActiveFlag _active;
    private Timestamp _createdAt;
    private Timestamp _modifiedAt;
    private AggregateVersion _version;
    public static Todo Record(Id id, Title title)
    {
      return Todo.Of(
        id ?? Id.Next(),
        title,
        CompletedFlag.Of(false),
        Timestamp.Now(), // CreatedAt
        Timestamp.Now(), // ModifiedAt
        ActiveFlag.Of(true),
        AggregateVersion.Next()
      );
    }
    public static Todo Of(
      Id id,
      Title title,
      CompletedFlag completed,
      Timestamp createdAt,
      Timestamp modifiedAt,
      ActiveFlag active,
      AggregateVersion version)
    {
      return new Todo(
        id,
        title,
        completed,
        createdAt,
        modifiedAt,
        active,
        version);
    }
    private Todo(
      Id id,
      Title title,
      CompletedFlag completed,
      Timestamp createdAt,
      Timestamp modifiedAt,
      ActiveFlag active,
      AggregateVersion version)
    {
      _id = id;
      _title = title;
      _completed = completed;
      _active = active;
      _createdAt = createdAt;
      _modifiedAt = modifiedAt;
      _version = version;
    }
    public void SetCompletionState(CompletedFlag completed, Title title = null)
    {
      if (completed.IsTrue())
      {
        MarkComplete(title);
      }
      else
      {
        MarkIncomplete(title);
      }
    }
    public void MarkComplete(Title title = null)
    {
      if (title != null) EnsureTitleMatchesWhenTogglingCompletion(title);
      this._completed = CompletedFlag.Of(true);
      this._modifiedAt = Timestamp.Now();
    }
    public void MarkIncomplete(Title title = null)
    {
      if (title != null) EnsureTitleMatchesWhenTogglingCompletion(title);
      this._completed = CompletedFlag.Of(false);
      this._modifiedAt = Timestamp.Now();
    }
    private void EnsureTitleMatchesWhenTogglingCompletion(Title title)
    {
      if (!this.Title.Equals(title))
      {
        throw new ArgumentOutOfRangeException(
          "title", 
          "You can not rename a Todo while changing its completion state."
        );
      }
    }
    public void Rename(Title newName, CompletedFlag completed = null)
    {
      if (completed != null && !Completed.Equals(completed))
      {
        throw new ArgumentOutOfRangeException(
          "completed", 
          "You can not change a Todo's completion state while renaming it."
        );
      }
      this._title = newName;
      this._modifiedAt = Timestamp.Now();
    }
    public void Cancel()
    {
      this._active = ActiveFlag.Of(false);
      this._modifiedAt = Timestamp.Now();
    }
    public bool Equals(IReferenceObject other)
    {
      return this.Id.Equals(other.Id);
    }
    public Id Id { get { return _id; } }
    public Title Title { get { return _title; } }
    public CompletedFlag Completed { get { return _completed; } }
    public ActiveFlag Active { get { return _active; } }
    public Timestamp CreatedAt { get { return _createdAt; } }
    public Timestamp ModifiedAt { get { return _modifiedAt; } }
    public AggregateVersion Version { get { return _version; } }
  }
}