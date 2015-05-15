﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Arsenalcn.Core
{
    public interface IRepository
    {
        T Single<T>(object key) where T : class, IEntity;
        T Single<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity;

        IQueryable<T> All<T>() where T : class, IEntity;
        IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity;

        void Insert<T>(T instance, SqlTransaction trans = null) where T : class, IEntity;
        object InsertOutKey<T>(T instance, SqlTransaction trans = null) where T : class, IEntity;
        void Insert<T>(IEnumerable<T> instances, SqlTransaction trans = null) where T : class, IEntity;

        void Update<T>(T instance, SqlTransaction trans = null) where T : class, IEntity;
        void Update<T>(IEnumerable<T> instances, SqlTransaction trans = null) where T : class, IEntity;

        void Delete<T>(object key, SqlTransaction trans = null) where T : class, IEntity;
        void Delete<T>(T instance, SqlTransaction trans = null) where T : class, IEntity;
        void Delete<T>(Expression<Func<T, bool>> predicate, SqlTransaction trans = null) where T : class, IEntity;
    }
}