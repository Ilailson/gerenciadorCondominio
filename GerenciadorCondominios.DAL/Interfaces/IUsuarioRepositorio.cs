using GerenciadorCondominios.BLL.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

        /*Gerenciamento de usuário*/


        Task<bool> VerificarSeUsuarioEstaEmFuncao(Usuario usuario, string funcao);

        Task<IList<string>> PegarFuncoesUsuario(Usuario usuario);//IEnumerable - listar funcoes - usuarios

        Task<IdentityResult> RemoverFuncoesUsuario(Usuario usuario, IEnumerable<string> funcoes);

        Task<IdentityResult> IncluirUsuarioEmFuncoes(Usuario usuario, IEnumerable<string> funcoes);

        Task<Usuario> PegarUsuarioPeloNome(ClaimsPrincipal usuario);

        Task<Usuario> PegarUsuarioPeloId(string usuarioId);





    }
}
 