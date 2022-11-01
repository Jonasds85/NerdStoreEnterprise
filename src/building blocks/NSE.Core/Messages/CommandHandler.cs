using FluentValidation.Results;
using NSE.Core.Data;

namespace NSE.Core.Messages
{
    public abstract class CommandHandler
    {
        protected ValidationResult validationResult;
        protected CommandHandler()
        {
            validationResult = new ValidationResult();
        }

        protected void AdicionarErro(string mensagem)
        {
            validationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }

        protected async Task<ValidationResult> PersistirDados(IUnitOfWork uow)
        {
            if (!await uow.Commit()) AdicionarErro("Houve um erro ao persistir os dados");

            return validationResult;
        }
    }
}
