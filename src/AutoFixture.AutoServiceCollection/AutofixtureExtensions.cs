using System.Linq;
using AutoFixture.Dsl;

namespace AutoFixture.AutoServiceCollection
{
    public static class AutofixtureExtensions
    {
        public static void Eject<T>(this IFixture fixture)
        {
            var composer = fixture.Customizations.FirstOrDefault(x => x is NodeComposer<T>);
            if (composer == null)
            {
                return;
            }

            fixture.Customizations.Remove(composer);
        }
    }
}