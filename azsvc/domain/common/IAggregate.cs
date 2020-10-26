namespace Ec.Sar.Common.Domain
{
  public interface IAggregate : IReferenceObject
  {
    ActiveFlag Active { get; }
    Timestamp CreatedAt { get; }
    Timestamp ModifiedAt { get; }
    AggregateVersion Version { get; }
  }
}