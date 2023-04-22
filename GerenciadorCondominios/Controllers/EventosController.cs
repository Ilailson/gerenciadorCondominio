﻿using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GerenciadorCondominios.Controllers
{
 
    public class EventosController : Controller
    {
        private readonly IEventoRepositorio _eventoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public EventosController(IEventoRepositorio eventoRepositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            _eventoRepositorio = eventoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
            Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloNome(User);

           if(await _usuarioRepositorio.VerificarSeUsuarioEstaEmFuncao(usuario, "Morador"))
            {
                return View(await _eventoRepositorio.PegarEventosPeloId(usuario.Id));
            }
            //se for sindico
            return View(await _eventoRepositorio.PegarTodos());

           
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloNome(User);

            //instanciando um evento
            Evento evento = new Evento
            {
                UsuarioId = usuario.Id
            }; //preenchendo usuario na view
            return View(evento); 

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventoId, Nome,Data,UsuarioId")] Evento evento)
        {
            if(evento != null){
                await _eventoRepositorio.inserir(evento);
                TempData["NovoRegistro"] = $"Evento {evento.Nome} inserido com sucesso";
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
           
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Evento evento = await _eventoRepositorio.PegarPeloId(id);
            if(evento== null)
            {
                return NotFound();
            }
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventoId, Nome,Data,UsuarioId")] Evento evento)
        {
            if (id != evento.EventoId)
            {
                return NotFound();
            }
            else
            {
                await _eventoRepositorio.Atualizar(evento);
                TempData["Atualizado"] = $"Evento {evento.Nome} atualizado";
                return RedirectToAction(nameof(Index));
            }

            return View(evento);
          
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            await _eventoRepositorio.Excluir(id);
            TempData["Atualizado"] = $"Evento excluído";
            return Json("Evento excluído");
        }

    }
}
