using Domain.Handlers;
using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using MassTransit;
using MediatR;
using Moq;
using System.Linq.Expressions;

namespace Tests
{
    public class ProductUpdateHandlerTest : BaseConfig
    {

        private readonly Mock<IRepository<Produtos>> _repository;
        private readonly Mock<IBobStorageService> _bobStorageService;
        private readonly Mock<IPublishEndpoint> _massTransient;
        private readonly ProdutoUpdateHandler _handler;
        public ProductUpdateHandlerTest()
        {
            InitializeMediatrService();

            _massTransient = new Mock<IPublishEndpoint>();
            _bobStorageService = new Mock<IBobStorageService>();
            _repository = new Mock<IRepository<Produtos>>();
            _handler = new ProdutoUpdateHandler(_repository.Object, _bobStorageService.Object, _massTransient.Object);
        }

        [Fact(DisplayName = "Teste 01 - Atualizar com sucesso")]
        [Trait("Produtoservice", " ProductUpdateHandler")]
        public async Task Test1()
        {
            // Arrange
            var command = new UpdateProdutoCommand
            {
                Id =  Guid.NewGuid(),
                Nome = "Produtos 01",
                Preco = 10.5m,
                Active = true
            };

            // Configura o callback para o método FindOne
            _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Produtos, bool>>>(), null))
                       .Callback<Expression<Func<Produtos, bool>>, FindOptions?>((predicate, options) =>
                       { })
                       .Returns<Expression<Func<Produtos, bool>>, FindOptions?>((predicate, options) =>
                       {
                           var Produtos = new Produtos
                           {
                               Id = command.Id,
                               Nome = "Produtos Teste",
                               Preco = 20.00m,
                               Active = true,
                               CreatAt = DateTime.Now
                           };
                           // Se o predicate for válido, retorne o Produtos
                           return predicate.Compile().Invoke(Produtos) ? Produtos : null;
                       });


            _repository.Setup(r => r.Update(It.IsAny<Produtos>()))
               .Callback<Produtos>(p =>
               { })
               .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.True(result);

            _repository.Verify(r => r.FindOne(It.IsAny<Expression<Func<Produtos, bool>>>(), null), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<Produtos>()), Times.Once);
        }

        [Fact(DisplayName = "Teste 02 - Atualizar erro")]
        [Trait("Produtoservice", " ProductUpdateHandler")]
        public async Task Test2()
        {
            // Arrange

            var command = new UpdateProdutoCommand
            {
                Id = Guid.NewGuid(),
                Nome = "Produtos 01",
                Preco = 10.5m,
                Active = true
            };


            // Configura o callback para o método FindOne
            _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Produtos, bool>>>(), null))
                       .Callback<Expression<Func<Produtos, bool>>, FindOptions?>((predicate, options) =>
                       { })
                       .Returns<Expression<Func<Produtos, bool>>, FindOptions?>((predicate, options) =>
                       {
                           var Produtos = new Produtos
                           {
                               Id = Guid.NewGuid(),
                               Nome = "Produtos Teste",
                               Preco = 20.00m,
                               Active = true,
                               CreatAt = DateTime.Now
                           };
                           // Se o predicate for válido, retorne o Produtos
                           return predicate.Compile().Invoke(Produtos) ? Produtos : null;
                       });


            _repository.Setup(r => r.Update(It.IsAny<Produtos>()))
               .Callback<Produtos>(p =>
               { })
               .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.False(result);
            _repository.Verify(r => r.Update(It.IsAny<Produtos>()), Times.Never);
        }
    }
}