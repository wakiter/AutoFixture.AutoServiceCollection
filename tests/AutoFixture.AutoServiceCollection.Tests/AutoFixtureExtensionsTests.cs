using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace AutoFixture.AutoServiceCollection.Tests
{
    public sealed class AutoFixtureExtensionsTests
    {
        [Fact]
        public void Eject_removes_injected_instance()
        {
            var sut = new Fixture();
            var injectedInstance = new object();

            sut.Inject(injectedInstance);

            var actual = sut.Create<object>();
            actual.Should().BeSameAs(injectedInstance);

            sut.Eject<object>();

            actual = sut.Create<object>();

            actual.Should().NotBeSameAs(injectedInstance);
        }

        [Fact]
        public void Eject_removes_frozen_instance()
        {
            var sut = new Fixture();
            var frozenObject = sut.Freeze<object>();

            var actual = sut.Create<object>();
            actual.Should().BeSameAs(frozenObject);

            sut.Eject<object>();

            actual = sut.Create<object>();
            actual.Should().NotBeSameAs(frozenObject);
        }

        [Fact]
        public void Eject_removes_nothing_when_there_is_no_injected_instance_of_a_given_type()
        {
            var sut = new Fixture();
            sut.Freeze<int>();
            var customizations = sut.Customizations.ToArray();

            sut.Eject<object>();

            sut.Customizations.Should().BeEquivalentTo(customizations);
        }
    }
}
