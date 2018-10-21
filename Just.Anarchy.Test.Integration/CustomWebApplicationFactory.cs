using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Just.Anarchy.Test.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly Action<IServiceCollection> _customServiceConfiguration;

        public CustomWebApplicationFactory(Action<IServiceCollection> customServiceConfiguration)
        {
            _customServiceConfiguration = customServiceConfiguration;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(new string[0])
                .UseStartup<Startup>().ConfigureServices(_customServiceConfiguration);
        }
    }
}
