using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Transacao;

namespace ResiGa.Bkd.Domain.Interfaces.Repositories;

public interface ITransacaoRepository
{
    Task<Transacao> CreateTransacaoAsync(Transacao transacao);
    Task<PaginatedResult<Transacao>> GetTransacoesAsync(ListTransacoes listTransacoes);
    Task<Transacao?> FindTransacaoByIdAsync(Guid transacaoId);
    Task UpdateTransacaoAsync(Transacao transacao);
    Task DeleteTransacaoAsync(Guid transacaoId);
}
