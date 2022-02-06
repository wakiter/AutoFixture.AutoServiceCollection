using System;
using System.Linq;
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

            actualOne.InterfaceObj.GetType().FullName.Should().StartWithEquivalentOf("Castle.Proxies.ObjectProxy");
        }

        [Fact]
        public void It_removes_previous_instances_of_this_customisation()
        {
            var previousCustomisation = _sut.Customizations.First(x => x is ServiceCollectionConnector);

            _sut.Customize(new AutoServiceCollectionCustomization());

            _sut.Customizations.Should().Contain(x => x is ServiceCollectionConnector);
            _sut.Customizations.Should().NotContain(previousCustomisation);
        }

        [Fact]
        public void It_throws_an_exception_when_fixture_passed_is_null()
        {
            var customisation = new AutoServiceCollectionCustomization();
            Action act = () => customisation.Customize(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void It_does_nothing_when_customised_previously()
        {
            var customisation = new AutoServiceCollectionCustomization();
            var sut = new Fixture();
            sut.Customize(customisation);

            var customisationsBeforeSecondInvocation = sut.Customizations.ToArray();

            customisation.Customize(sut);

            sut.Customizations.Should().BeEquivalentTo(customisationsBeforeSecondInvocation);
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
