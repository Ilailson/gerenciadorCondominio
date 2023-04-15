using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorCondominios.Controllers
{
    [Authorize]
    public class VeiculosController : Controller
    {
        private readonly IVeiculoReposito _veiculoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public VeiculosController(IVeiculoReposito veiculoRepositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            _veiculoRepositorio = veiculoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VeiculoId,Nome,Marca,Cor,Placa,UsuarioId")] Veiculo veiculo)
        {
            if (veiculo != null)
            {
                Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloNome(User);
                veiculo.UsuarioId = usuario.Id;
                await _veiculoRepositorio.inserir(veiculo);
                return RedirectToAction("MinhasInformacoes", "Usuarios");
            }

            return View(veiculo);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var veiculo = await _veiculoRepositorio.PegarPeloId(id);
            if(veiculo == null)
            {
                return NotFound();
            }
           
            return View(veiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VeiculoId,Nome,Marca,Cor,Placa,UsuarioId")] Veiculo veiculo)
        {
            if (id != veiculo.VeiculoId)
            {
                return NotFound();
            }

            if (veiculo != null)
            {
                await _veiculoRepositorio.Atualizar(veiculo);
                return RedirectToAction("MinhasInformacoes", "Usuarios");
            }

            return View(veiculo);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            /*Sera executado via AJAX  */
            await _veiculoRepositorio.Excluir(id);
            return Json("Veículo excluído com sucesso");
        }
    }
}
