using System;

namespace Ec.Sar.Common.Domain
{
  public class Timestamp : IValueObject<long>
  {
    private long _value;
    public static Timestamp Now()
    {
      return Timestamp.Of(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds());
    }
    public static Timestamp Of(long value)
    {
      return new Timestamp(value);
    }
    private Timestamp(long value) { _value = value; }
    public long Value { get { return _value; } }

    public long ToLong() { return Value; }
    public bool Equals(IValueObject<long> other)
    {
      return this.Value == other.Value;
    }
  }
}