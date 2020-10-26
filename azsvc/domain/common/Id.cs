using System;
using System.ComponentModel.DataAnnotations;

namespace Ec.Sar.Common.Domain
{
  public class Id: IValueObject<string>
  {
    [MinLength(21, ErrorMessage = "The value of an ID must be at least 21 characters long.")]
    private string _value;
    public static Id Next() => new Id(Nanoid.Nanoid.Generate());
    public static Id Of(string value) => new Id(value);
    private Id(string value)
    {
      this._value = value;
    }
    public override string ToString() => Value;
    public string Value { get { return _value; } }
    public bool Equals(IValueObject<string> other) => this.Value == other.Value;
  }
}
