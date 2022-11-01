using EasyNetQ;
using NSE.Cliente.API.Application.Commands;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.Messagebus;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace NSE.Cliente.API.Services;

public class RegistroclienteIntegrationHandler : BackgroundService
{
    private readonly IMessageBus _bus;
    private readonly IServiceProvider _serviceProvider;

    public RegistroclienteIntegrationHandler(IServiceProvider provider, IMessageBus bus)
    {
        _serviceProvider = provider;
        _bus = bus;
    }

    private void SetResponder()
    {
        _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request =>
            await RegistrarCliente(request));

        _bus.AdvancedBus.Connected += OnConnect;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();
        return Task.CompletedTask;
    }

    private void OnConnect(object s, EventArgs e)
    {
        SetResponder();
    }

    private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
    {
        var clienteCommand = new RegistrarClienteCommand(message.Id, message.Nome, message.Email, message.Cpf);

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
        var sucesso = await mediator.EnviarComando(clienteCommand);
        return new ResponseMessage(sucesso);
    }
}
