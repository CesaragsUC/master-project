using Catalog.Domain.Abstractions;
using Catalog.Infrastructure.Repository;
using MongoDB.Driver;
using Moq;

namespace Api.Catalogo.Tests
{
    public static class BaseConfig
    {
        /// <summary>
        /// Usado para retornar uma Instancia do MongoRepository e um Mock do IMongoCollection
        /// TEntity será o nome da coleção no MongoDB
        /// </summary>
        /// <typeparam name="TEntity">Tipo do objeto</typeparam>
        /// <returns>
        /// Tipo MongoRepository: Instacia.
        /// Tipo IMongoCollection: Mock.
        /// </returns>

        public static (MongoRepository<TEntity>, Mock<IMongoCollection<TEntity>>) 
            InitializeMongoRepository<TEntity>() where TEntity : class
        {
            var _mockCollection = new Mock<IMongoCollection<TEntity>>();

           var  _mockContext = new Mock<IMongoDbContext>();

            _mockContext
                .Setup(c => c.GetCollection<TEntity>(It.IsAny<string>()))
                .Returns(_mockCollection.Object);


            return (new MongoRepository<TEntity>(_mockContext.Object), _mockCollection);
        }

        /// <summary>
        /// Usado para retornar o Mock MongoRepository e IMongoCollection
        /// TEntity será o nome da coleção no MongoDB
        /// </summary>
        /// <typeparam name="TEntity">Tipo do objeto</typeparam>
        /// <returns>
        /// Tipo MongoRepository: Mock.
        /// Tipo IMongoCollection: Mock.
        /// </returns>
        public static (Mock<IMongoRepository<TEntity>>, Mock<IMongoCollection<TEntity>>)
            InitializeMongoRepositoryMock<TEntity>() where TEntity : class
        {
            var _mockCollection = new Mock<IMongoCollection<TEntity>>();

            var _mockContext = new Mock<IMongoDbContext>();

            _mockContext
                .Setup(c => c.GetCollection<TEntity>(It.IsAny<string>()))
                .Returns(_mockCollection.Object);

            var _repository = new Mock<IMongoRepository<TEntity>>();

            return (_repository, _mockCollection);
        }
    }
}
