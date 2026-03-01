using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Transacao;

namespace ResiGa.Bkd.Domain.Interfaces.Services;

public interface ITransacaoService
{
    Task<Transacao> CreateTransacaoAsync(Transacao transacao);
    Task<PaginatedResult<Transacao>> GetTransacoesAsync(ListTransacoes listTransacoes);
    Task<Transacao> FindTransacaoByIdAsync(Guid transacaoId);
    Task<Transacao> UpdateTransacaoAsync(Transacao updateTransacaoRequest, Guid transacaoId);
    Task DeleteTransacaoAsync(Guid transacaoId);
}
