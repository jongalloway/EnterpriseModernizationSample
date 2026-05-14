using System;

namespace Fabrikam.EnterprisePizza.Core.Services
{
    public interface IBusinessClock
    {
        DateTime GetCurrentTime();
    }

    public class SystemBusinessClock : IBusinessClock
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}
