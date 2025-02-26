﻿
using AutoMapper;
using EasyMongoNet.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Product;
using Product.Application.MongoEntities;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class ProductAddedEventHandler :
    IRequestHandler<ProductAddedEvent, bool>
{
    private readonly IMongoRepository<Products> _repository;
    private readonly IMapper _mapper;

    public ProductAddedEventHandler(IMongoRepository<Products> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(ProductAddedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var product = _mapper.Map<Products>(request);

            await _repository.InsertOneAsync(product);

            Log.Information("Adicionado produto nome: {Nome} - {Data}", request.Name, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Falha ao integrar produto", ex);
            return false;
        }

    }
}
