using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Services;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(ApplicationLayer)));
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(CreateSaleCommand).Assembly,
                typeof(GetSaleQuery).Assembly
            );
        });

        builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

        builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(ApplicationLayer)));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
    }
}