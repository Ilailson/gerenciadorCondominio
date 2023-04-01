using GerenciadorCondominios.BLL.Models;
using Microsoft.AspNetCore.Identity;

namespace GerenciadorCondominios.DAL.Interfaces
{
    public interface IUsuarioRepositorio : IRepositorioGenerico<Usuario>
    {
        int VerificarSeExisteRegistro();
        Task LogarUsuario(Usuario usuario, bool lembrar);
        Task DeslogarUsario();
        Task<IdentityResult> CriarUsuario(Usuario usuario, string senha);
        Task IncluirUsuarioEmFuncao(Usuario usuario, string funcao);
        Task<Usuario> PegarUsuarioPeloEmail(string email);//pegar e-mail - formulario - consultar banco. string email

        Task AtualizarUsuario(Usuario usuario);
    }
}
 