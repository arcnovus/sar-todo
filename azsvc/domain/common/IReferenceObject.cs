namespace Ec.Sar.Common.Domain {
  public interface IReferenceObject {
    Id Id { get; }
    bool Equals(IReferenceObject other);
  }
}