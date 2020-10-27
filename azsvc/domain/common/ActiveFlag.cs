using System;

namespace Ec.Sar.Common.Domain
{
  public class ActiveFlag : StatusFlag
  {
    public new static ActiveFlag Of(bool? isActive)
    {
      if (!isActive.HasValue)
      {
        throw new ArgumentNullException("isActive", "Can not be null.");
      }
      return new ActiveFlag((bool)isActive);
    }
    private ActiveFlag(bool value) : base(value) { }
  }
}