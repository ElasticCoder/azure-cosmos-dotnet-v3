﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary> 
    /// This class serve as LINQ query provider implementing IQueryProvider.
    /// </summary> 
    internal sealed class CosmosLinqQueryProvider : IQueryProvider
    {
        private readonly CosmosContainerCore container;
        private readonly CosmosQueryClient queryClient;
        private readonly CosmosJsonSerializer cosmosJsonSerializer;
        private readonly QueryRequestOptions cosmosQueryRequestOptions;
        private readonly bool allowSynchronousQueryExecution;

        public CosmosLinqQueryProvider(
           CosmosContainerCore container,
           CosmosJsonSerializer cosmosJsonSerializer,
           CosmosQueryClient queryClient,
           QueryRequestOptions cosmosQueryRequestOptions,
           bool allowSynchronousQueryExecution)
        {
            this.container = container;
            this.cosmosJsonSerializer = cosmosJsonSerializer;
            this.queryClient = queryClient;
            this.cosmosQueryRequestOptions = cosmosQueryRequestOptions;
            this.allowSynchronousQueryExecution = allowSynchronousQueryExecution;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new CosmosLinqQuery<TElement>(
                this.container,
                this.cosmosJsonSerializer,
                this.queryClient,
                this.cosmosQueryRequestOptions,
                expression,
                this.allowSynchronousQueryExecution);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type expressionType = TypeSystem.GetElementType(expression.Type);
            Type documentQueryType = typeof(CosmosLinqQuery<bool>).GetGenericTypeDefinition().MakeGenericType(expressionType);
            return (IQueryable)Activator.CreateInstance(
                documentQueryType,
                this.container,
                this.cosmosJsonSerializer,
                this.queryClient,
                this.cosmosQueryRequestOptions,
                expression,
                this.allowSynchronousQueryExecution);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Type cosmosQueryType = typeof(CosmosLinqQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof(TResult));
            CosmosLinqQuery<TResult> cosmosLINQQuery = (CosmosLinqQuery<TResult>)Activator.CreateInstance(
                cosmosQueryType,
                this.container,
                this.cosmosJsonSerializer,
                this.queryClient,
                this.cosmosQueryRequestOptions,
                expression,
                this.allowSynchronousQueryExecution);

            return cosmosLINQQuery.ToList().FirstOrDefault();
        }

        //Sync execution of query via direct invoke on IQueryProvider.
        public object Execute(Expression expression)
        {
            Type cosmosQueryType = typeof(CosmosLinqQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof(object));
            CosmosLinqQuery<object> documentQuery = (CosmosLinqQuery<object>)Activator.CreateInstance(
                cosmosQueryType,
                this.container,
                this.cosmosJsonSerializer,
                this.queryClient,
                this.cosmosQueryRequestOptions,
                this.allowSynchronousQueryExecution);

            return documentQuery.ToList().FirstOrDefault();
        }
    }
}
