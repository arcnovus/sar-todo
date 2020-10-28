using System;
using System.ComponentModel.DataAnnotations;

namespace Ec.Sar.Common.Domain
{
  public class Timestamp : IValueObject<long>
  {
    private long _value;
    
    public static Timestamp Now()
    {
      var utcNowOffset = ((DateTimeOffset)DateTime.UtcNow);
      return Timestamp.Of(utcNowOffset.ToUnixTimeMilliseconds());
    }

    public static Timestamp Of(long value)
    {
      var ts = new Timestamp(value);
      Validator.ValidateObject(ts, new ValidationContext(ts), true);
      return ts;
    }

    private Timestamp(long value) { _value = value; }
    
    [Required]
    public long Value { get { return _value; } }

    public long ToLong() { return Value; }
    public bool Equals(IValueObject<long> other)
    {
      return this.Value == other.Value;
    }
  }
}