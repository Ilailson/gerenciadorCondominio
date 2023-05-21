using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using GerenciadorCondominios.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorCondominios.Controllers
{

    public class ServicosController : Controller
    {
        private readonly IServicoRepositorio _servicoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IServicoPredioRepositorio _servicoPredioRepositorio;
        private readonly IHistoricoRecursosRepositorio _historicoRecursosRepositorio;

        public ServicosController
        (
            IServicoRepositorio servicoRepositorio,IUsuarioRepositorio usuarioRepositorio,
            IServicoPredioRepositorio servicoPredioRepositorio, IHistoricoRecursosRepositorio historicoRecursosRepositorio
        )
        {
            _servicoRepositorio = servicoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _servicoPredioRepositorio = servicoPredioRepositorio;
            _historicoRecursosRepositorio = historicoRecursosRepositorio;

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

        [HttpGet]
        public async Task<IActionResult> AprovarServico(int id)
        {
            //pegando servido aprovado - id
            Servico servico = await _servicoRepositorio.PegarPeloId(id);

            //instanciando viewModel - com serviços aprovado
            ServicoAprovadoViewModel viewModel = new ServicoAprovadoViewModel
            {
                ServicoId = servico.ServicoId,
                Nome = servico.Nome
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarServico(ServicoAprovadoViewModel viewModel)
        {
            if(viewModel != null)
            {
                /*pegar servido será aprovado - id*/
                Servico servico = await _servicoRepositorio.PegarPeloId(viewModel.ServicoId);
                servico.Status = StatusServico.Aceito;//atualizando status
                await _servicoRepositorio.Atualizar(servico);//atualizando banco


                //instanciando servico predio - inserir - banco

                ServicoPredio servicoPredio = new ServicoPredio
                {
                    ServicoId = viewModel.ServicoId,
                    DataExecucao = viewModel.Data

                };
                await _servicoPredioRepositorio.inserir(servicoPredio); //salvando  banco


                /*Instanciando historico recurso - inserir - banco*/
                HistoricoRecursos hr = new HistoricoRecursos
                {
                    Valor = servico.Valor,
                    MesId = servicoPredio.DataExecucao.Month,
                    Dia = servicoPredio.DataExecucao.Day,
                    Ano = servicoPredio.DataExecucao.Year,
                    Tipo = Tipos.Saida
                };

                await _historicoRecursosRepositorio.inserir(hr);//salvando - banco
                TempData["NovoRegistro"] = $"{servico.Nome} escalado com sucesso";
                return RedirectToAction(nameof(Index));
            }
            
            return View(viewModel);
        }

        public async Task<IActionResult> RecusarServico(int id)
        {
            Servico servico = await _servicoRepositorio.PegarPeloId(id); //pegando - idservico existente

            if (servico == null) return NotFound();

            servico.Status = StatusServico.Recusado;//atualizando status
            await _servicoRepositorio.Atualizar(servico); //atualizando  banco
            TempData["Exclusao"] = $"{servico.Nome} recusado";

            return RedirectToAction(nameof(Index));


        }

    }
}
