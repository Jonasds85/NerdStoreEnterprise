
using NSE.Core.DomainObjects;
using NSE.Pedidos.Domain.Vouchers.Specs;

namespace NSE.Pedidos.Domain.Vouchers
{
    public class Voucher : Entity, IAggregateRoot
    {
        public Voucher(
            string codigo, 
            decimal? percentual, 
            decimal? valorDesconto, 
            int quantidade, 
            TipoDescontoVoucher tipoDesconto, 
            DateTime dataCriacao, 
            DateTime? dataUtilizacao, 
            DateTime dataValidade, 
            bool ativo, 
            bool utilizado)
        {
            Codigo = codigo;
            Percentual = percentual ?? null;
            ValorDesconto = valorDesconto ?? null;
            Quantidade = quantidade;
            TipoDesconto = tipoDesconto;
            DataCriacao = dataCriacao;
            DataUtilizacao = dataUtilizacao ?? null;
            DataValidade = dataValidade;
            Ativo = ativo;
            Utilizado = utilizado;
        }

        protected Voucher()
        { }

        public string Codigo { get; private set; }
        public decimal? Percentual { get; private set; }
        public decimal? ValorDesconto { get; private set; }
        public int Quantidade { get; private set; }
        public TipoDescontoVoucher TipoDesconto { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataUtilizacao { get; private set; }
        public DateTime DataValidade { get; private set; }
        public bool Ativo { get; private set; }
        public bool Utilizado { get; private set; }

        public bool EstaValidoParaUtilizacao()
        {
            var spec = new VoucherAtivoSpecification()
                .And(new VoucherDataSpecification())
                .And(new VoucherQuantidadeSpecification());

            return spec.IsSatisfiedBy(this);
        }
    }

    public enum TipoDescontoVoucher
    {
        Porcentagem = 0,
        Valor = 1
    }
}
