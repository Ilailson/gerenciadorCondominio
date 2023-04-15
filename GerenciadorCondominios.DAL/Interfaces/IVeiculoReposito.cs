using GerenciadorCondominios.BLL.Models;

namespace GerenciadorCondominios.DAL.Interfaces
{
    public interface IVeiculoReposito : IRepositorioGenerico<Veiculo>
    {
        Task<IEnumerable<Veiculo>> PegarVeiculosPorUsuario(string usuarioId);

    }
}
