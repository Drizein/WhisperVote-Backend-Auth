﻿using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Interfaces;

public interface _IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> SelectAsync();
    Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> p);
    Task<T?> FindByAsync(Expression<Func<T, bool>> predicate);
    void Add(T item);
    Task<bool> SaveChangesAsync();
    T? Attach(T item);
}