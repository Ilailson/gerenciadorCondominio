using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCondominios.DAL.Repositorios
{
    public class VeiculoRepositorio : RepositorioGenerico<Veiculo>, IVeiculoReposito
    {
        private readonly Contexto _contexto;
        public VeiculoRepositorio(Contexto contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Veiculo>> PegarVeiculosPorUsuario(string usuarioId)
        {
            try
            {
                /*Retornar veiculos apenas de um usuario*/
                return await _contexto.Veiculos.Where(v => v.UsuarioId == usuarioId).ToListAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
