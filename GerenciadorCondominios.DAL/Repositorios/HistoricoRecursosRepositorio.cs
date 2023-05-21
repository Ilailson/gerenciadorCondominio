using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;

namespace GerenciadorCondominios.DAL.Repositorios
{
    public class HistoricoRecursosRepositorio : RepositorioGenerico<HistoricoRecursos>, IHistoricoRecursosRepositorio
    {
        public HistoricoRecursosRepositorio(Contexto contexto) : base(contexto)
        {
        }
    }
}