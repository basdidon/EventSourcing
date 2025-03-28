using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class BaseIntegrationTest:IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IServiceScope scope;
        protected readonly HttpClient client;

        public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            scope = factory.Services.CreateScope();
            client = factory.CreateClient();
        }


    }
}
