using GerenciadorCondominios.DAL.Interfaces;
using GerenciadorCondominios.DAL.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorCondominios.ViewComponents
{
    public class VeiculosViewComponent : ViewComponent
    {
        private readonly IVeiculoReposito _veiculoRepositorio;

        public VeiculosViewComponent(IVeiculoReposito veiculoRepositorio)
        {
            _veiculoRepositorio = veiculoRepositorio;
        }

    

        /*metodo chamado view - minhas informações*/
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            return View(await _veiculoRepositorio.PegarVeiculosPorUsuario(id));
        }
        

    }

}
