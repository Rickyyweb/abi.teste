using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Helpers
{
    [CollectionDefinition(Name)]
    public class DockerIntegrationCollection : ICollectionFixture<EmptyFixture>
    {
        public const string Name = "DockerIntegration";
    }

    public class EmptyFixture { }
}
