using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorCondominios.Controllers
{

    public class ServicosController : Controller
    {
        private readonly IServicoRepositorio _servicoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public ServicosController(IServicoRepositorio servicoRepositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            _servicoRepositorio = servicoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
            Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloNome(User);

           if(await _usuarioRepositorio.VerificarSeUsuarioEstaEmFuncao(usuario, "Morador"))
            {
                return View(await _servicoRepositorio.PegarServicosPeloUsuario(usuario.Id));
            }
            //se for sindico
            return View(await _servicoRepositorio.PegarTodos());

           
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloNome(User);

            //instanciando um evento
            Servico servico = new Servico
            {
                UsuarioId = usuario.Id
            }; //preenchendo usuario na view
            return View(servico); 

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicoId,Nome,Valor,status,UsuarioId")] Servico servico)
        {
            if(servico != null){
                servico.Status = StatusServico.Pendente;
                await _servicoRepositorio.inserir(servico);
                TempData["NovoRegistro"] = $"Servico {servico.Nome} solicitado";
                return RedirectToAction(nameof(Index));
            }
            return View(servico); 
           
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Servico servico = await _servicoRepositorio.PegarPeloId(id);
            if(servico == null)
            {
                return NotFound();
            }
            return View(servico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicoId,Nome,Valor,status,UsuarioId")] Servico servico)
        {
            if (id != servico.ServicoId)
            {
                return NotFound();
            }
            else
            {
                await _servicoRepositorio.Atualizar(servico);
                TempData["Atualizacao"] = $"Servico {servico.Nome} atualizado";
                return RedirectToAction(nameof(Index));
            }

            return View(servico);
          
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            await _servicoRepositorio.Excluir(id);
            TempData["Exclusao"] = $"Servico excluído";
            return Json("Serviço excluído");
        }

    }
}
