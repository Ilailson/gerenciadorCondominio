using GerenciadorCondominios.BLL.Models;

namespace GerenciadorCondominios.DAL.Interfaces
{
    public interface IServicoRepositorio : IRepositorioGenerico<Servico>
    {
        Task<IEnumerable<Servico>> PegarServicosPeloUsuario(string usuarioId);
    }

}
