using System;

namespace Ec.Sar.Common.Domain
{
  public class StatusFlag : IValueObject<bool>
  {
    private bool _value;
    public static StatusFlag Of(bool? value)
    {
      if (!value.HasValue)
      {
        throw new ArgumentNullException("value", "Can not be null.");
      }
      return new StatusFlag((bool)value);
    }
    protected StatusFlag(bool value) { _value = value; }
    public bool Value { get { return _value; } }
    public bool IsTrue() => Value; 
    public bool IsFalse() => !Value; 
    public bool ToBoolean() => Value; 
    public override string ToString() => Value.ToString();
    public bool Equals(IValueObject<bool> other)
    {
      return this.Value == other.Value;
    }
  }
}