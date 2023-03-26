using GerenciadorCondominios.BLL.Models;
using Microsoft.AspNetCore.Identity;

namespace GerenciadorCondominios.DAL.Interfaces
{
    public interface IUsuarioRepositorio : IRepositorioGenerico<Usuario>
    {
        int VerificarSeExisteRegistro();
        Task LogarUsuario(Usuario usuario, bool lembrar);
        Task<IdentityResult> CriarUsuario(Usuario usuario, string senha);
        Task IncluirUsuarioEmFuncao(Usuario usuario, string funcao);
    }
}
