using Domain.Handlers;
using Domain.Handlers.Comands;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Moq;
using System.Linq.Expressions;

namespace Tests
{
    public class DeleteProductHandlerTest : BaseConfig
    {

        private readonly Mock<IRepository<Product>> _repository;
        private readonly Mock<IMediator> _mediator;
        private DeleteProductHandler _handler;
        public DeleteProductHandlerTest()
        {
            InitializeMediatrService();

            _repository = new Mock<IRepository<Product>>();
            _mediator = new Mock<IMediator>();

            _handler = new DeleteProductHandler(_repository.Object);
        }


        [Fact(DisplayName = "Teste 01 - Deletar com sucesso")]
        [Trait("Produtoservice", "ProductDeleteHandler")]
        public async Task Test1()
        {
            var command = new DeleteProductCommand
            {
                Id = Guid.NewGuid()
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


            _repository.Setup(r => r.Delete(It.IsAny<Product>()))
               .Callback<Product>(p =>
               { })
               .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.True(result);
            _repository.Verify(r => r.FindOne(It.IsAny<Expression<Func<Product, bool>>>(), null), Times.Once);
            _repository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
        }

        [Fact(DisplayName = "Teste 02 - Deletar com erro")]
        [Trait("Produtoservice", " ProductDeleteHandler")]
        public async Task Test2()
        {
            // Arrange

            var command = new DeleteProductCommand
            {
                Id = Guid.Empty
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act
            Assert.False(result);
            _repository.Verify(r => r.FindOne(It.IsAny<Expression<Func<Product, bool>>>(), null), Times.Never);
            _repository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
        }
    }
}