using System;

namespace GerenciadorCondominios.BLL.Models
{
    public class Pagamento
    {
        public int PagamentoId { get; set; }

        //relacionamentos
        public string UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public int AluguelId { get; set; }
        public Aluguel Aluguel { get; set; }
        //

        public DateTime? DataPagamento { get; set; }//data pode ser nula _?_

        public StatusPagamento Status { get; set; }
    }

    public enum StatusPagamento
    {
        Pago, Pendente, Atrasado
    }
}
