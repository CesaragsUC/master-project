using Domain.Handlers;
using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using MassTransit;
using MediatR;
using Moq;

//https://goatreview-com.cdn.ampproject.org/c/s/goatreview.com/mediatr-quickly-test-handlers-with-unit-tests/amp/

namespace Tests
{
    public class ProductoCreateHandlerTest : BaseConfig
    {
        private readonly Mock<IRepository<Produtos>> _repository;
        private readonly Mock<IPublishEndpoint> _massTransient;
        private readonly Mock<IBobStorageService> _bobStorageService;
        private readonly ProdutoCreateHandler _handler;
        public ProductoCreateHandlerTest()
        {
            InitializeMediatrService();

            _repository = new Mock<IRepository<Produtos>>();
            _massTransient = new Mock<IPublishEndpoint>();
            _bobStorageService = new Mock<IBobStorageService>();
            _handler = new ProdutoCreateHandler(_repository.Object, _massTransient.Object, _bobStorageService.Object);
        }

        [Fact(DisplayName = "Teste 01 - Com sucesso")]
        [Trait("Produtoservice", "ProductoCreateHandler")]
        public async Task Test1()
        {
            // Arrange

            var command = new CreateProdutoCommand
            {
                Nome = "Produtos 01",
                Preco = 10.5m,
                Active = true
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.True(result);

            _repository.Verify(r => r.Add(It.IsAny<Produtos>()), Times.Once);
        }

        [Fact(DisplayName = "Teste 02 - Com Falha")]
        [Trait("Produtoservice", "ProductoCreateHandler")]
        public async Task Test2()
        {
            // Arrange

            var command = new CreateProdutoCommand();

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.False(result);
            _repository.Verify(r => r.Add(It.IsAny<Produtos>()), Times.Never);
        }
    }
}