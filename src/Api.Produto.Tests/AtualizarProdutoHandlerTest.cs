using Domain.Handlers;
using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using MassTransit;
using Moq;
using System.Linq.Expressions;

namespace Tests
{
    public class AtualizarProdutoHandlerTest : BaseConfig
    {

        private readonly Mock<IRepository<Product>> _repository;
        private readonly Mock<IBobStorageService> _bobStorageService;
        private readonly Mock<IPublishEndpoint> _massTransient;
        private readonly UpdateProductHandler _handler;
        public AtualizarProdutoHandlerTest()
        {
            InitializeMediatrService();

            _massTransient = new Mock<IPublishEndpoint>();
            _bobStorageService = new Mock<IBobStorageService>();
            _repository = new Mock<IRepository<Product>>();
            _handler = new UpdateProductHandler(_repository.Object, _bobStorageService.Object, _massTransient.Object);
        }

        [Fact(DisplayName = "Teste 01 - Atualizar com sucesso")]
        [Trait("Produtoservice", " ProductUpdateHandler")]
        public async Task Test1()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id =  Guid.NewGuid(),
                Name = "Produtos 01",
                Price = 10.5m,
                Active = true
            };

            // Configura o callback para o método FindOne
            _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Product, bool>>>(), null))
                       .Callback<Expression<Func<Product, bool>>, FindOptions?>((predicate, options) =>
                       { })
                       .Returns<Expression<Func<Product, bool>>, FindOptions?>((predicate, options) =>
                       {
                           var product = new Product
                           {
                               Id = command.Id,
                               Name = "Produtos Teste",
                               Price = 20.00m,
                               Active = true,
                               CreatAt = DateTime.Now
                           };
                           // Se o predicate for válido, retorne o Produtos
                           return predicate.Compile().Invoke(product) ? product : null;
                       });


            _repository.Setup(r => r.Update(It.IsAny<Product>()))
               .Callback<Product>(p =>
               { })
               .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.True(result);

            _repository.Verify(r => r.FindOne(It.IsAny<Expression<Func<Product, bool>>>(), null), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
        }

        [Fact(DisplayName = "Teste 02 - Atualizar erro")]
        [Trait("Produtoservice", " ProductUpdateHandler")]
        public async Task Test2()
        {
            // Arrange

            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                Name = "Produtos 01",
                Price = 10.5m,
                Active = true
            };


            // Configura o callback para o método FindOne
            _repository.Setup(r => r.FindOne(It.IsAny<Expression<Func<Product, bool>>>(), null))
                       .Callback<Expression<Func<Product, bool>>, FindOptions?>((predicate, options) =>
                       { })
                       .Returns<Expression<Func<Product, bool>>, FindOptions?>((predicate, options) =>
                       {
                           var product = new Product
                           {
                               Id = Guid.NewGuid(),
                               Name = "Produtos Teste",
                               Price = 20.00m,
                               Active = true,
                               CreatAt = DateTime.Now
                           };
                           // Se o predicate for válido, retorne o Produtos
                           return predicate.Compile().Invoke(product) ? product : null;
                       });


            _repository.Setup(r => r.Update(It.IsAny<Product>()))
               .Callback<Product>(p =>
               { })
               .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.False(result);
            _repository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        }
    }
}