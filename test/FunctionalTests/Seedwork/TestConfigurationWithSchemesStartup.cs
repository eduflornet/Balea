﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Acheve.AspNetCore.TestHost.Security;
using Acheve.TestHost;
using Balea;

namespace FunctionalTests.Seedwork
{
    public class TestConfigurationWithSchemesStartup
    {
        private readonly IConfiguration configuration;

        public TestConfigurationWithSchemesStartup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddBalea(options =>
                {
                    options.DefaultClaimTypeMap = new DefaultClaimTypeMap
                    {
                        SubjectClaimType = JwtClaimTypes.Subject
                    };
                    options.AddAuthenticationSchemes("scheme2");
                })
                .AddConfigurationStore(configuration)
                .Services
                .AddAuthentication(setup =>
                {
                    setup.DefaultAuthenticateScheme = "scheme1";
                    setup.DefaultChallengeScheme = "scheme1";
                })
                .AddTestServer("scheme1")
                .AddTestServer("scheme2", options =>
                {
                    options.RoleClaimType = "sourceRole";
                })
                .AddTestServer("scheme3")

                .Services
                .AddAuthorization(options =>
                {
                    options.AddPolicy(Policies.Custom, builder =>
                    {
                        builder.RequireAuthenticatedUser();
                        builder.AddAuthenticationSchemes("scheme3");
                    });
                })
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
        }
    }
}
