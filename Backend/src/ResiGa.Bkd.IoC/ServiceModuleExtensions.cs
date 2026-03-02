using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Service;
using ResiGa.Bkd.Infra.Repositories;

namespace ResiGa.Bkd.Ioc;

/// <summary>
/// Classe de extensao para registro de dependencias no container de IoC.
/// Todos os servicos e repositorios sao registrados como Scoped (um por requisicao HTTP).
/// O TransacaoService depende de IPessoaRepository e ICategoriaRepository
/// para validar regras de negocio (menor de idade e compatibilidade de categoria).
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceModuleExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(configuration);

        // Registra servicos e repositorios de Pessoa
        services.AddScoped<IPessoaService, PessoaService>();
        services.AddScoped<IPessoaRepository, PessoaRepository>();

        // Registra servicos e repositorios de Categoria
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        // Registra servicos e repositorios de Transacao
        // Nota: TransacaoService recebe IPessoaRepository e ICategoriaRepository via DI
        // para validar regras de negocio na criacao/edicao de transacoes
        services.AddScoped<ITransacaoService, TransacaoService>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
    }
}
