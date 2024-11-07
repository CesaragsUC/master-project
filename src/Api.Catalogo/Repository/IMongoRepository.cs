﻿using Api.Catalogo.Dtos;
using Api.Catalogo.Filters;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Api.Catalogo.Repository;

public interface IMongoRepository<TEntity> where TEntity : class
{
    Task<PagedResult<TEntity>> GetAll(int page = 1, int pageSize = 10, string sort = "asc");
    Task<TEntity> GetById(string field, Guid id);
    Task<PagedResult<TEntity>> GetByName(string field, string nome);
    Task Insert(TEntity obj);
    Task InsertMany(List<TEntity> obj);

    /// <summary>
    ///  Atualiza Multiplos campos
    /// </summary>
    /// <param name="whereCondition"></param>
    /// <param name="updates"></param>
    /// <returns></returns>
    Task UpdateAsync(string id, TEntity obj);

    /// <summary>
    ///  Atualiza um campo
    /// </summary>
    /// <param name="whereCondition"></param>
    /// <param name="updates"></param>
    /// <returns>
    /// Atualiza um campo
    /// </returns>
    Task UpdateAsync<TField>(
    Expression<Func<TEntity, bool>> whereCondition,
    Expression<Func<TEntity, TField>> field,
    TField value);


    Task Delete(string field, Guid id);
    Task DeleteByName(string field, string nome);
    Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> filter);
}