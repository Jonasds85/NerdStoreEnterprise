using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.API.Data;
using NSE.Carrinho.API.Model;
using NSE.WebAPI.Core.Controllers;
using NSE.WebAPI.Core.Usuario;

namespace NSE.Carrinho.API.Controllers
{    
    [Authorize]
    public class CarrinhoController : MainController
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly CarrinhoContext _context;

        public CarrinhoController(IAspNetUser aspNetUser, CarrinhoContext context)
        {
            _aspNetUser = aspNetUser;
            _context = context;
        }

        [HttpGet("carrinho")]
        public async Task<CarrinhoCliente> ObterCarrinho()
        {
            return await ObterCarrinhoCliente() ?? new CarrinhoCliente();
        }

        [HttpPost("carrinho")]
        public async Task<IActionResult> AdicionarItemCarrinho(CarrinhoItem carrinhoItem)
        {
            var carrinho = await ObterCarrinhoCliente();

            if(carrinho == null)            
                ManipulaNovoCarrinho(carrinhoItem);            
            else            
                ManipulaCarrinhoExistente(carrinho, carrinhoItem);
                        
            if (!OperacaoValida()) return CustomResponse();

            await PersistirDados();
            return CustomResponse();
        }

        [HttpPut("carrinho/{produtoId}")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, CarrinhoItem item)
        {
            var carrinho = await ObterCarrinhoCliente();
            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho, item);
            if (itemCarrinho is null) return CustomResponse();

            carrinho.AtualizarUnidades(itemCarrinho, item.Quantidade);

            ValidarCarrinho(carrinho);
            if (!OperacaoValida()) return CustomResponse();

            _context.CarrinhoItems.Update(itemCarrinho);
            _context.CarrinhoClientes.Update(carrinho);

            await PersistirDados();
            return CustomResponse();
        }

        [HttpDelete("carrinho/{produtoId}")]
        public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            var carrinho = await ObterCarrinhoCliente();
            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho);
            if (itemCarrinho is null) return CustomResponse();            

            ValidarCarrinho(carrinho);
            if (!OperacaoValida()) return CustomResponse();

            carrinho.RemoverItem(itemCarrinho);

            _context.CarrinhoItems.Remove(itemCarrinho);
            _context.CarrinhoClientes.Update(carrinho);

            await PersistirDados();
            return CustomResponse();
        }

        [HttpPost("carrinho/aplicar-voucher")]
        public async Task<IActionResult> AplicarVoucher(Voucher voucher)
        {
            var carrinho = await ObterCarrinhoCliente();

            carrinho.AplicarVoucher(voucher);

            _context.CarrinhoClientes.Update(carrinho);

            await PersistirDados();
            return CustomResponse();
        }        

        private async Task<CarrinhoCliente> ObterCarrinhoCliente()
        {
            var clienteId = _aspNetUser.ObterUserId();

            return await _context.CarrinhoClientes
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        }
        private void ManipulaNovoCarrinho(CarrinhoItem item)
        {
            var carrinho = new CarrinhoCliente(_aspNetUser.ObterUserId());
            carrinho.AdicionarItem(item);

            ValidarCarrinho(carrinho);
            _context.CarrinhoClientes.Add(carrinho);
        }
        private void ManipulaCarrinhoExistente(CarrinhoCliente carrinho, CarrinhoItem item)
        {
            var produtoItemExistente = carrinho.CarrinhoItemExistente(item);

            carrinho.AdicionarItem(item);
            ValidarCarrinho(carrinho);

            if (produtoItemExistente)
            {
                _context.CarrinhoItems.Update(carrinho.ObterItemPorProdutoId(item.ProdutoId));
            }
            else
            {
                _context.CarrinhoItems.Add(item);
            }

            _context.CarrinhoClientes.Update(carrinho);
        }
        private async Task<CarrinhoItem> ObterItemCarrinhoValidado(Guid produtoId, CarrinhoCliente carrinho, CarrinhoItem item = null )
        {
            if (item != null && produtoId != item.ProdutoId)
            {
                AdicionarErroProcessamento("O item não corresponde ao informado");
                return null;
            }

            if (carrinho is null)
            {
                AdicionarErroProcessamento("Carrinho não encontrado");
                return null;
            }

            var itemCarrinho = await _context.CarrinhoItems
                .FirstOrDefaultAsync(i => i.CarrinhoId == carrinho.Id && i.ProdutoId == produtoId);

            if (itemCarrinho == null || !carrinho.CarrinhoItemExistente(itemCarrinho))
            {
                AdicionarErroProcessamento("O item não está no carrinho");
                return null;
            }

            return itemCarrinho;
        }
        private async Task PersistirDados()
        {
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result <= 0) AdicionarErroProcessamento("Não foi possível persistir os dados no banco");

            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }
        }
        private bool ValidarCarrinho(CarrinhoCliente carrinho)
        {
            if (carrinho.EhValido()) return true;

            carrinho.ValidationResult.Errors.ToList().ForEach(e => AdicionarErroProcessamento(e.ErrorMessage));
            return false;
        }
    }
}
