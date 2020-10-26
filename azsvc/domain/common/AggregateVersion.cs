using System;
namespace Ec.Sar.Common.Domain
{
  public class AggregateVersion : IValueObject<decimal>
  {
    private decimal _value;
    public decimal Value { get { return _value; } }
    public static AggregateVersion Next()
    {
      var rand = new Random();
      return AggregateVersion.Of(
        Timestamp.Now().ToLong() + decimal.Parse(rand.NextDouble().ToString())
      );
    }
    public static AggregateVersion Of(decimal value)
    {
      return new AggregateVersion(value);
    }
    private AggregateVersion(decimal value)
    {
      this._value = value;
    }
    public decimal ToDecimal() => Value;
    public bool Equals(IValueObject<decimal> other) => this.Value == other.Value;
  }
}