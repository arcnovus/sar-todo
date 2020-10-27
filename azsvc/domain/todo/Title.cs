using System;
using System.ComponentModel.DataAnnotations;
using Ec.Sar.Common.Domain;

namespace Ec.Sar.TodoDemo.Domain
{
  public class Title: IValueObject<string>
  {
    private string _value;

    public static Title Of(string value)
    {
      var newTitle = new Title(value?.Trim());
      Validator.ValidateObject(newTitle, new ValidationContext(newTitle), true);
      return newTitle;
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
  
    [Required(ErrorMessage="A title must have a value.")]
    [MinLength(1, ErrorMessage = "The value of a title must be at least 1 non-empty character in length.")]
    public string Value { get { return _value; } }
  }
}