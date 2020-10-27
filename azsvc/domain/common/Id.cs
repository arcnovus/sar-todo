using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ec.Sar.Common.Domain
{
  public class Id : IValueObject<string>
  {
    private string _value;
    public static Id Next() => new Id(Nanoid.Nanoid.Generate());
    public static Id Of(string value) { 
      Id newId = new Id(value); 
      Validator.ValidateObject(newId, new ValidationContext(newId), true);
      return newId;
    }
    private Id(string value)
    {
      this._value = value;
    }
    public override string ToString() => Value;
    
    [Required(ErrorMessage="An id must have a value.")]
    [MinLength(21, ErrorMessage = "The value of an ID must be at least 21 characters long.")]
    public string Value { get { return _value; } }
    public bool Equals(IValueObject<string> other) => this.Value == other.Value;
  }
}
