using System;
using Ec.Sar.Common.Domain;

namespace Ec.Sar.TodoDemo.Domain
{
  public class Title: IValueObject<string>
  {
    private string _value;

    public static Title Of(string value)
    {
      return new Title(value);
    }
    private Title(string value)
    {
      this._value = value;
    }

    public bool Equals(IValueObject<string> other)
    {
      return this.Value == other.Value;
    }

    public override string ToString()
    {
      return this.Value;
    }
    public string Value { get { return _value; } }
  }
}