

using System.Linq.Expressions;

namespace NSE.Pedidos.Domain.Vouchers.Specs
{
    public class VoucherDataSpecification : NetDevPack.Specification.Specification<Voucher>
    {
        public override Expression<Func<Voucher, bool>> ToExpression()
        {
            return voucher => voucher.DataValidade >= DateTime.Now;
        }
    }

    public class VoucherQuantidadeSpecification : NetDevPack.Specification.Specification<Voucher>
    {
        public override Expression<Func<Voucher, bool>> ToExpression()
        {
            return voucher => voucher.Quantidade > 0;
        }
    }

    public class VoucherAtivoSpecification : NetDevPack.Specification.Specification<Voucher>
    {
        public override Expression<Func<Voucher, bool>> ToExpression()
        {
            return voucher => voucher.Ativo && !voucher.Utilizado;
        }
    }
}
