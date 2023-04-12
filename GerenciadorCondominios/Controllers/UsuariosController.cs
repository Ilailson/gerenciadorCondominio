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
        private readonly IFuncaoRepositorio _funcaoRepositorio;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsuariosController(
            IUsuarioRepositorio usuarioRepositorio,
            IWebHostEnvironment webHostEnvironment,
            IFuncaoRepositorio funcaoRepositorio
            )
        {
            _usuarioRepositorio = usuarioRepositorio;
            _webHostEnvironment = webHostEnvironment;
            _funcaoRepositorio = funcaoRepositorio;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _usuarioRepositorio.PegarTodos()); /*listarUsuarios -formulario*/
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
                    return RedirectToAction(nameof(RedefinirSenha), usuario);
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

        public IActionResult Reprovado(string nome)
        {
            return View(nome);
        }


        //=============================FUNÇÕES=================================
        public async Task<JsonResult> AprovarUsuario(string usuarioId)
        {
            Usuario usuario = await _usuarioRepositorio.PegarPeloId(usuarioId);
            usuario.Status = StatusConta.Aprovado;
            await _usuarioRepositorio.IncluirUsuarioEmFuncao(usuario, "Morador");
            await _usuarioRepositorio.AtualizarUsuario(usuario);

            return Json(true);
        }

        public async Task<JsonResult> ReprovarUsuario(string usuarioId)
        {
            Usuario usuario = await _usuarioRepositorio.PegarPeloId(usuarioId);
            usuario.Status = StatusConta.Reprovado;
            await _usuarioRepositorio.AtualizarUsuario(usuario);

             return Json(true);
        }

        //================================Gerenciamento de usuários ==================

        [HttpGet]
        public async Task<IActionResult> Gerenciarusuario(string usuarioId, string nome)
        {

            if (usuarioId == null) return NotFound();

            //armazena informações - controller enviar - view
            TempData["usuarioId"] = usuarioId;
            ViewBag.Nome = nome;
            Usuario usuario = await _usuarioRepositorio.PegarPeloId(usuarioId);

            if(usuario == null) return NotFound();

            List<FuncaoUsuarioViewModel> viewModel = new List<FuncaoUsuarioViewModel>();

            foreach(Funcao funcao in await _funcaoRepositorio.PegarTodos())
            {
                FuncaoUsuarioViewModel model = new FuncaoUsuarioViewModel
                {
                    FuncaoId = funcao.Id,
                    Nome = funcao.Name,
                    Descricao = funcao.Descricao
                };
                //checando - usuario - funcao
                if (await _usuarioRepositorio.VerificarSeUsuarioEstaEmFuncao(usuario, funcao.Name ))
                {
                    model.isSelecionado= true;
                }
                else
                    model.isSelecionado = false;
                viewModel.Add(model);
            }
            return View(viewModel);
        }

         
        //utilizará lista - mostrar funções - possiveis selecionar
        [HttpPost]
        public async Task<IActionResult> Gerenciarusuario(List<FuncaoUsuarioViewModel> model)
        {

            string usuarioId = TempData["usuarioId"].ToString();//usuarioId - metodo GET

            Usuario usuario = await _usuarioRepositorio.PegarPeloId(usuarioId);

            if (usuario == null) return NotFound();

            IEnumerable<string> funcoes = await _usuarioRepositorio.PegarFuncoesUsuario(usuario);
            IdentityResult resultado = await _usuarioRepositorio.RemoverFuncoesUsuario(usuario, funcoes);

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError("", "Não foi possível atualizar as funções do usuário");
                return View("GerenciarUsuario", usuarioId);
            }

            //salvar selecionados - forms
            resultado = await _usuarioRepositorio.IncluirUsuarioEmFuncoes(usuario,
                model.Where(x => x.isSelecionado == true).Select(x => x.Nome));

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError("", "Não foi possível atualizar as funções do usuário");
                return View("GerenciarUsuario", usuarioId);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MinhasInformacoes()
        {
            return View(await _usuarioRepositorio.PegarUsuarioPeloNome(User));
        }

        /*mostrar informações do morador para atualizar*/
        [HttpGet]
        public async Task<IActionResult> Atualizar(string id)
        {
            Usuario usuario = await _usuarioRepositorio.PegarPeloId(id);

            if(usuario == null) return NotFound();

            //instanciar viewModel para pegar atributos que precisamos. 

            AtualizarViewModel model = new AtualizarViewModel
            {
                UsuarioId= usuario.Id,
                Nome = usuario.UserName,
                CPF = usuario.CPF,
                Email = usuario.Email,
                Foto = usuario.Foto,
                Telefone = usuario.PhoneNumber
            };

            TempData["Foto"] = usuario.Foto;//salvando o caminho da foto. 

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Atualizar(AtualizarViewModel viewModel, IFormFile foto)
        {
            /*IFormFile é para a foto*/

            if (foto != null)
            {
                string directorioPasta = Path.Combine(_webHostEnvironment.WebRootPath, "Imagens");
                string nomeFoto = Guid.NewGuid().ToString() + foto.FileName;

                //salvando a foto
                using (FileStream fileAtream = new FileStream(Path.Combine(directorioPasta, nomeFoto), FileMode.Create))
                {
                    await foto.CopyToAsync(fileAtream);
                    viewModel.Foto = "~/Imagens/" + nomeFoto;
                }
            }

            else
                viewModel.Foto = TempData["Foto"].ToString(); /*permanece a mesma foto*/

            Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloId(viewModel.UsuarioId);
            usuario.UserName = viewModel.Nome;
            usuario.CPF = viewModel.CPF;
            usuario.PhoneNumber = viewModel.Telefone;
            usuario.Foto = viewModel.Foto;
            usuario.Email = viewModel.Email;

            await _usuarioRepositorio.AtualizarUsuario(usuario);

            if(await _usuarioRepositorio.VerificarSeUsuarioEstaEmFuncao(usuario, "Administrador")
                || await _usuarioRepositorio.VerificarSeUsuarioEstaEmFuncao(usuario, "Sindico"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(MinhasInformacoes));
            }
               
            return View(viewModel);

        }

        [HttpGet]
        public IActionResult RedefinirSenha(Usuario usuario)
        {
            LoginViewModel model = new LoginViewModel
            {
                Email = usuario.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RedefinirSenha(LoginViewModel model)
        {
            if(model != null)
            {
                Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloEmail(model.Email);
                model.Senha = _usuarioRepositorio.CodificarSenha(usuario, model.Senha);
                usuario.PasswordHash = model.Senha;
                usuario.PrimeiroAcesso = false;
                await _usuarioRepositorio.AtualizarUsuario(usuario);
                await _usuarioRepositorio.LogarUsuario(usuario, false);

                return RedirectToAction(nameof(MinhasInformacoes));
            }




            return View(model);
        }



    }
}
