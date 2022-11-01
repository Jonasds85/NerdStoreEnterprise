using FluentValidation.Results;
using MediatR;
using NSE.Cliente.API.Application.Events;
using NSE.Cliente.API.Models;
using NSE.Core.Messages;

namespace NSE.Cliente.API.Application.Commands
{
    public class ClienteCommandHandler : CommandHandler, IRequestHandler<RegistrarClienteCommand, ValidationResult>
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ValidationResult> Handle(RegistrarClienteCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var cliente = new Clientes(message.Id, message.Name, message.Email, message.Cpf);                        

            if (await _clienteRepository.ClienteExiste(cliente.Cpf.Numero))
            {
                AdicionarErro("Este CPF já está em uso");
                return validationResult;
            }

            _clienteRepository.Adicionar(cliente);

            cliente.AdicionarEvento(
                new ClienteRegistradoEvent(message.Id, message.Name, message.Email, message.Cpf));

            return await PersistirDados(_clienteRepository.UnitOfWork);
        }
    }
}
