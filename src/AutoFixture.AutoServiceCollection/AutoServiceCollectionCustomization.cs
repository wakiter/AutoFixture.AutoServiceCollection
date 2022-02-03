using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFixture.AutoServiceCollection
{
    public sealed class AutoServiceCollectionCustomization : ICustomization
    {
        private IServiceCollection _serviceCollection;

        public void Customize(IFixture fixture)
        {
            if (fixture == null)
            {
                throw new ArgumentNullException(nameof(fixture));
            }

            if (_serviceCollection != null)
            {
                return;
            }

            _serviceCollection = new ISCWrapper(new ServiceCollection(), fixture);
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            foreach (var specimenBuilder in fixture.Customizations.Where(x => x is ServiceCollectionConnector).ToArray())
            {
                fixture.Customizations.Remove(specimenBuilder);
            }

            fixture.Customizations.Insert(0, new ServiceCollectionConnector(serviceProvider));

            fixture.Inject<IServiceProvider>(serviceProvider);
            fixture.Inject(_serviceCollection);
        }
    }
}
