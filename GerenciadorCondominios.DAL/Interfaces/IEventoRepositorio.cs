using GerenciadorCondominios.BLL.Models;

namespace GerenciadorCondominios.DAL.Interfaces
{
    public interface IEventoRepositorio : IRepositorioGenerico<Evento>
    {
        Task<IEnumerable<Evento>> PegarEventosPeloId(string usuarioId);
    }

}
