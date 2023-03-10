using System;

namespace GerenciadorCondominios.BLL.Models
{
    public class ServicoPredio
    {
        public int ServicoPredioId { get; set; }

        //Relacionamento
        public int ServicoId { get; set; }
        public virtual Servico Servico { get; set; }
        //

        public DateTime DataExecucao { get; set; }
    }
}
