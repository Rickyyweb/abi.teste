using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using FluentValidation.AspNetCore;     
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers
{
    public class WebApiModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            builder.AddDefaultLogging();

            builder.AddBasicHealthChecks();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddFluentValidation(c =>
            {
                c.RegisterValidatorsFromAssemblyContaining<ApplicationLayer>();
            });

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
        }
    }
}
