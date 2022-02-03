using System;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AutoFixture.AutoServiceCollection.Tests
{
    public class AutoServiceCollectionCustomizationTests
    {
        private readonly IFixture _sut = new Fixture()
            .Customize(new AutoServiceCollectionCustomization())
            .Customize(new AutoMoqCustomization());

        [Fact]
        public void It_uses_DI_definition_when_present()
        {
            var b = new LeafObj();
            
            var serviceCollection = _sut.Freeze<IServiceCollection>();
            var DIUsed = false;

            serviceCollection
                .AddSingleton(_ =>
                {
                    DIUsed = true;
                    return b;
                });
                
            var sp = _sut.Freeze<IServiceProvider>();
            
            var actualBFromServiceProvider = sp.GetService<LeafObj>();

            var actualOne = _sut.Create<RootObj>();
            var actualTwo = _sut.Create<RootObj>();
            var actualThree = _sut.Create<RootObj>();

            DIUsed.Should().BeTrue();

            actualOne.Should().NotBeNull();
            actualOne.LeafObj.Should().BeSameAs(b);

            actualTwo.LeafObj.Should().BeSameAs(b);
            actualThree.LeafObj.Should().BeSameAs(b);

            actualBFromServiceProvider.Should().BeSameAs(b);

            actualOne.InterfaceObj.GetType().FullName.Should().Be("Castle.Proxies.ObjectProxy");
        }
    }

    public sealed class RootObj
    {
        public LeafObj LeafObj { get; }

        public IAmInterfaceObj InterfaceObj { get; }

        public RootObj(LeafObj leafObj, IAmInterfaceObj interfaceObj)
        {
            LeafObj = leafObj;
            InterfaceObj = interfaceObj;
        }
    }

    public sealed class LeafObj
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public interface IAmInterfaceObj {}
}
