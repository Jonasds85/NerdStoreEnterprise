using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using NSE.Core.Communication;

namespace NSE.WebAPI.Core.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private ICollection<string> Erros = new List<string>();

        protected ActionResult CustomResponse(ResponseResult response)
        {
            ResponsePossuiErros(response);

            return CustomResponse();
        }

        protected bool ResponsePossuiErros(ResponseResult resposta)
        {
            if (resposta != null && !resposta.Errors.Mensagens.Any()) return false;

            foreach (var mensagens in resposta.Errors.Mensagens)
                AdicionarErroProcessamento(mensagens);

            return true;
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida()) return Ok(result);

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var erro in erros)
                AdicionarErroProcessamento(erro.ErrorMessage);

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ValidationResult validationResult)
        {
            var erros = validationResult.Errors;

            foreach (var erro in erros)
                AdicionarErroProcessamento(erro.ErrorMessage);

            return CustomResponse();
        }

        protected ActionResult CustomResponse(string erro)
        {
            AdicionarErroProcessamento(erro);
            return CustomResponse();
        }

        protected bool OperacaoValida()
        {
            return !Erros.Any();
        }

        protected void AdicionarErroProcessamento(string erro)
        {
            Erros.Add(erro);
        }

        protected void LimparErroProcessamento()
        {
            Erros.Clear();
        }
    }
}
