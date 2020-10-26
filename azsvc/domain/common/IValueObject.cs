namespace Ec.Sar.Common.Domain {
  public interface IValueObject<T> {
    T Value { get; }
    string ToString();
    bool Equals(IValueObject<T> other);
  }
}