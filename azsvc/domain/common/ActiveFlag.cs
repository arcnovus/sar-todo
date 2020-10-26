using System;

namespace Ec.Sar.Common.Domain
{
  public class ActiveFlag : StatusFlag
  {
    public new static ActiveFlag Of(bool? value)
    {
      if (!value.HasValue)
      {
        throw new ArgumentNullException("value", "Can not be null.");
      }
      return new ActiveFlag((bool)value);
    }
    private ActiveFlag(bool value) : base(value) { }
  }
}