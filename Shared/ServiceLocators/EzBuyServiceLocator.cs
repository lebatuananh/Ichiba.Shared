using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.ServiceLocators
{
    public class EzBuyServiceLocator
    {
        private static ServiceProvider _serviceProvider;
        private readonly ServiceProvider _currentServiceProvider;

        public EzBuyServiceLocator(ServiceProvider currentServiceProvider)
        {
            _currentServiceProvider = currentServiceProvider;
        }

        public static EzBuyServiceLocator Current => new EzBuyServiceLocator(_serviceProvider);

        public static void SetLocatorProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetInstance(Type serviceType)
        {
            return _currentServiceProvider.GetService(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return _currentServiceProvider.GetService<TService>();
        }
    }
}