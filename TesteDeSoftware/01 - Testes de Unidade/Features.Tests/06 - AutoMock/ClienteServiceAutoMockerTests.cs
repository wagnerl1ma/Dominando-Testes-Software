﻿using System.Linq;
using System.Threading;
using Features.Clientes;
using MediatR;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Features.Tests;

/// <summary>
/// A classe ClienteServiceAutoMockerFixtureTests é uma cópia dessa classe, porém
/// implementando o autoMocker de uma forma mais eficiente e menos manual
/// </summary>
[Collection(nameof(ClienteBogusCollection))]
public class ClienteServiceAutoMockerTests
{
    readonly ClienteTestsBogusFixture _clienteTestsBogus;

    public ClienteServiceAutoMockerTests(ClienteTestsBogusFixture clienteTestsFixture)
    {
        _clienteTestsBogus = clienteTestsFixture;
    }

    [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
    [Trait("Categoria", "Cliente Service AutoMock Tests")]
    public void ClienteService_Adicionar_DeveExecutarComSucesso()
    {
        // Arrange
        var cliente = _clienteTestsBogus.GerarClienteValido();

        //criando uma instancia de ClienteService com o AutoMocker
        var mocker = new AutoMocker();
        var clienteService = mocker.CreateInstance<ClienteService>();

        // Act
        clienteService.Adicionar(cliente);

        // Assert
        Assert.True(cliente.EhValido());
        mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Once);
        mocker.GetMock<IMediator>().Verify(r => r.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
    }

    [Fact(DisplayName = "Adicionar Cliente com Falha")]
    [Trait("Categoria", "Cliente Service AutoMock Tests")]
    public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
    {
        // Arrange
        var cliente = _clienteTestsBogus.GerarClienteInvalido();
        var mocker = new AutoMocker();
        var clienteService = mocker.CreateInstance<ClienteService>();

        // Act
        clienteService.Adicionar(cliente);

        // Assert
        Assert.False(cliente.EhValido());
        mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Never);
        mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
    }

    [Fact(DisplayName = "Obter Clientes Ativos")]
    [Trait("Categoria", "Cliente Service AutoMock Tests")]
    public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
    {
        // Arrange
        var mocker = new AutoMocker();
        var clienteService = mocker.CreateInstance<ClienteService>();

        mocker.GetMock<IClienteRepository>().Setup(c => c.ObterTodos())
            .Returns(_clienteTestsBogus.ObterClientesVariados());

        // Act
        var clientes = clienteService.ObterTodosAtivos();

        // Assert 
        mocker.GetMock<IClienteRepository>().Verify(r => r.ObterTodos(), Times.Once);
        Assert.True(clientes.Any());
        Assert.False(clientes.Count(c => !c.Ativo) > 0);
    }
}
