using System;

namespace Shared.DependencyManagers
{
    public class DependencyManager
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void SetServiceProvider(IServiceProvider serviceProviderInstance)
        {
            ServiceProvider = serviceProviderInstance;
        }
    }
}