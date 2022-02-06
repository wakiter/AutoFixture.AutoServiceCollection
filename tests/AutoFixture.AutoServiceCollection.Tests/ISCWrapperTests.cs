using System;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AutoFixture.AutoServiceCollection.Tests
{
    public sealed class ISCWrapperTests
    {
        private readonly IFixture _helperFixture = new Fixture().Customize(new AutoMoqCustomization());

        private readonly ISCWrapper _sut;

        private readonly Mock<IServiceCollection> _inner;

        private readonly Mock<IFixture> _fixture;

        public ISCWrapperTests()
        {
            _inner = _helperFixture.Freeze<Mock<IServiceCollection>>();
            _fixture = _helperFixture.Freeze<Mock<IFixture>>();
            _helperFixture.Inject(_fixture.Object);

            _sut = _helperFixture.Create<ISCWrapper>();
        }

        [Fact]
        public void GetEnumerator_invokes_decorated_instance()
        {
            var actual = _sut.GetEnumerator();

            actual.Should().NotBeNull();

            _inner.Verify(x => x.GetEnumerator());
        }

        [Fact]
        public void Contains_invokes_decorated_instance()
        {
            var serviceDescriptor = _helperFixture.Create<ServiceDescriptor>();
            var unused = _sut.Contains(serviceDescriptor);

            _inner.Verify(x => x.Contains(serviceDescriptor));
        }

        [Fact]
        public void CopyTo_invokes_decorated_instance()
        {
            var arrayToCopyTo = new ServiceDescriptor[0];
            _sut.CopyTo(arrayToCopyTo, 0);

            _inner.Verify(x => x.CopyTo(arrayToCopyTo, 0));
        }

        [Fact]
        public void Count_invokes_decorated_instance()
        {
            var unused = _sut.Count;

            _inner.Verify(x => x.Count);
        }

        [Fact]
        public void IsReadOnly_invokes_decorated_instance()
        {
            var unused = _sut.IsReadOnly;

            _inner.Verify(x => x.IsReadOnly);
        }

        [Fact]
        public void IndexOf_invokes_decorated_instance()
        {
            var serviceDescriptor = _helperFixture.Create<ServiceDescriptor>();
            var unused = _sut.IndexOf(serviceDescriptor);

            _inner.Verify(x => x.IndexOf(serviceDescriptor));
        }

        [Fact]
        public void Indexer_on_get_invokes_decorated_instance()
        {
            var serviceDescriptor = _helperFixture.Create<ServiceDescriptor>();
            _sut.Add(serviceDescriptor);
            _inner.Setup(x => x[0]).Returns(serviceDescriptor);

            var unused = _sut[0];

            _inner.Verify(x => x[0]);
        }
        
        [Fact]
        public void Add_invokes_decorated_instance_and_registers_a_new_service_provider()
        {
            var serviceDescriptor = _helperFixture.Create<ServiceDescriptor>();
            
            _sut.Add(serviceDescriptor);

            _inner.Verify(x => x.Add(serviceDescriptor));
            _fixture.Verify(x => x.Customize(It.IsAny<Func<ICustomizationComposer<ServiceProvider>, ISpecimenBuilder>>()));
        }

        [Fact]
        public void Clear_invokes_decorated_instance_and_registers_a_new_service_provider()
        {
            _sut.Clear();

            _inner.Verify(x => x.Clear());
            _fixture.Verify(x => x.Customize(It.IsAny<Func<ICustomizationComposer<ServiceProvider>, ISpecimenBuilder>>()));
        }

        [Fact]
        public void Remove_invokes_decorated_instance_and_registers_a_new_service_provider()
        {
            var serviceDescriptor = _helperFixture.Create<ServiceDescriptor>();
            _sut.Add(serviceDescriptor);

            _sut.Remove(serviceDescriptor);

            _inner.Verify(x => x.Remove(serviceDescriptor));
            _fixture.Verify(x => x.Customize(It.IsAny<Func<ICustomizationComposer<ServiceProvider>, ISpecimenBuilder>>()));
        }

        [Fact]
        public void Insert_invokes_decorated_instance_and_registers_a_new_service_provider()
        {
            var serviceDescriptor = _helperFixture.Create<ServiceDescriptor>();
            _sut.Insert(0, serviceDescriptor);

            _inner.Verify(x => x.Insert(0, serviceDescriptor));
            _fixture.Verify(x => x.Customize(It.IsAny<Func<ICustomizationComposer<ServiceProvider>, ISpecimenBuilder>>()));
        }

        [Fact]
        public void RemoveAt_invokes_decorated_instance_and_registers_a_new_service_provider()
        {
            _sut.RemoveAt(0);

            _inner.Verify(x => x.RemoveAt(0));
            _fixture.Verify(x => x.Customize(It.IsAny<Func<ICustomizationComposer<ServiceProvider>, ISpecimenBuilder>>()));
        }

        [Fact]
        public void Indexer_on_set_invokes_decorated_instance_and_registers_a_new_service_provider()
        {
            var serviceDescriptor = _helperFixture.Create<ServiceDescriptor>();
            _sut[0] = serviceDescriptor;

            _inner.VerifySet(x => x[0] = serviceDescriptor);
            _fixture.Verify(x => x.Customize(It.IsAny<Func<ICustomizationComposer<ServiceProvider>, ISpecimenBuilder>>()));
        }
    }
}
