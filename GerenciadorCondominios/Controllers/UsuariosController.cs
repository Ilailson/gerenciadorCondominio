using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using GerenciadorCondominios.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorCondominios.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsuariosController(IUsuarioRepositorio usuarioRepositorio, IWebHostEnvironment webHostEnvironment)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async  Task<IActionResult> Registro(RegistroViewModel model, IFormFile foto  )
        {
            //if (ModelState.IsValid)
            //{
                if(foto != null)
                {
                    string directorioPasta = Path.Combine(_webHostEnvironment.WebRootPath, "Imagens");
                    string nomeFoto = Guid.NewGuid().ToString() + foto.FileName;

                    //salvando a foto
                    using (FileStream fileAtream = new FileStream(Path.Combine(directorioPasta, nomeFoto), FileMode.Create))
                    {
                        await foto.CopyToAsync(fileAtream);
                        model.Foto = "~/Imagens/" + nomeFoto;
                    } 
                }
                Usuario usuario = new Usuario();
                IdentityResult usuariocriado;

                if(_usuarioRepositorio.VerificarSeExisteRegistro() == 0)
                {
                    usuario.UserName = model.Nome;
                    usuario.CPF= model.CPF;
                    usuario.Email = model.Email;
                    usuario.PhoneNumber = model.Telefone;
                    usuario.Foto = model.Foto;
                    usuario.PrimeiroAcesso = false;
                    usuario.Status = StatusConta.Aprovado;

                    usuariocriado = await _usuarioRepositorio.CriarUsuario(usuario, model.Senha);

                    if (usuariocriado.Succeeded)
                    {
                        await _usuarioRepositorio.IncluirUsuarioEmFuncao(usuario, "Administrador");
                        await _usuarioRepositorio.LogarUsuario(usuario, false);
                        return RedirectToAction("Index", "Usuarios");
                    }
                }

                usuario.UserName = model.Nome;
                usuario.CPF = model.CPF;
                usuario.Email = model.Email;
                usuario.PhoneNumber = model.Telefone;
                usuario.Foto = model.Foto;
                usuario.PrimeiroAcesso = true;
                usuario.Status = StatusConta.Analisando;

                usuariocriado = await _usuarioRepositorio.CriarUsuario(usuario, model.Senha);

                if(usuariocriado.Succeeded)
                {
                    return View("Analise", usuario.UserName);
                }
                else
                {
                    foreach(IdentityError erro in usuariocriado.Errors)
                    {
                        ModelState.AddModelError("", erro.Description);
                    }
                    return View(model);
                }

            //}
            return View(model); 
        }

        [HttpGet]
        public async  Task<IActionResult> Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                await _usuarioRepositorio.DeslogarUsario();
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloEmail(model.Email);
            if (usuario != null)
            {
                if (usuario.Status == StatusConta.Analisando)
                {
                    return View("Analise", usuario.UserName);
                }
                else if(usuario.Status == StatusConta.Reprovado)
                {
                    return View("Reprovado", usuario.UserName);
                }
                else if (usuario.PrimeiroAcesso == true)
                {
                    return View("RedefinirSenha", usuario);
                }
                else
                {
                    PasswordHasher<Usuario> passwordHasher = new PasswordHasher<Usuario>();

                    //checando se a senha está correta
                    if(passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, model.Senha) != PasswordVerificationResult.Failed)
                    {
                        await _usuarioRepositorio.LogarUsuario(usuario, false);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Usuario e/ou senhas inválidos");
                        return View(model);
                    }
                }

            }
            else
            {
                ModelState.AddModelError("", "Usuario e/ou senhas inválidods");
                return View(model);
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _usuarioRepositorio.DeslogarUsario();
            return RedirectToAction("Login");
        }

        public IActionResult Analise(string nome)
        {
            return View(nome);
        }
    }
}
