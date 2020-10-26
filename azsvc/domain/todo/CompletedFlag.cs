using System;
using Ec.Sar.Common.Domain;

namespace Ec.Sar.TodoDemo.Domain
{
  public class CompletedFlag : StatusFlag
  {
        public new static CompletedFlag Of(bool? value)
    {
      if (!value.HasValue)
      {
        throw new ArgumentNullException("value", "Can not be null.");
      }
      return new CompletedFlag((bool)value);
    }
    public static CompletedFlag Of(bool value) => new CompletedFlag(value);
    private CompletedFlag(bool value) : base(value) { }
  }
}