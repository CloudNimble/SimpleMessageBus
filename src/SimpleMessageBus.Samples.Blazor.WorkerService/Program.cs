using System;

namespace SimpleMessageBus.Samples.Blazor.WorkerService
{


    public class Program
    {

        static void Main()
        {
            if (!OperatingSystem.IsBrowser())
            {
                throw new PlatformNotSupportedException("Can only be run in the browser!");
            }
        }

    }

}
