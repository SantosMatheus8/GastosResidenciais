using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Service;
using ResiGa.Bkd.Infra.Repositories;

namespace ResiGa.Bkd.Ioc;

[ExcludeFromCodeCoverage]
public static class ServiceModuleExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(configuration);

        services.AddScoped<IPessoaService, PessoaService>();
        services.AddScoped<IPessoaRepository, PessoaRepository>();

        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        services.AddScoped<ITransacaoService, TransacaoService>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
    }
}
