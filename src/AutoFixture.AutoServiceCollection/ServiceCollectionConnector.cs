using System;
using AutoFixture.Kernel;

namespace AutoFixture.AutoServiceCollection
{
    internal sealed class ServiceCollectionConnector : ISpecimenBuilder
    {
        public IServiceProvider ServiceProvider { get; internal set; }

        public ServiceCollectionConnector(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (!(request is Type type))
            {
                return new NoSpecimen();
            }

            var obj = ServiceProvider?.GetService(type);

            if (obj != null)
            {
                return obj;
            }

            return new NoSpecimen();
        }
    }
}