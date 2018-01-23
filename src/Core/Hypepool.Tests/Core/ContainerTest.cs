using System;
using FluentAssertions;
using Hypepool.Core.Internals.Bootstrap;
using Xunit;

namespace Hypepool.Tests.Core
{
    public class ContainerTest
    {
        [Fact]
        public void VerifyContainer()
        {
            var bootstrapper = new Bootstrapper(); // IoC kernel bootstrapper.
            Action act = () => bootstrapper.Container.Verify(); // verify the container.
            act.ShouldNotThrow();
        }
    }
}
