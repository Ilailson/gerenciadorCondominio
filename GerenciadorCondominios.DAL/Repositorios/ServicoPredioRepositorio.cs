using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCondominios.DAL.Repositorios
{
    public class ServicoPredioRepositorio : RepositorioGenerico<ServicoPredio>, IServicoPredioRepositorio
    {
        public ServicoPredioRepositorio(Contexto contexto) : base(contexto)
        {
        }
    }
}
