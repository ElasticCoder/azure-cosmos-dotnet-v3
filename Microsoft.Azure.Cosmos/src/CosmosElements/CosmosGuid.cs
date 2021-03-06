﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Microsoft.Azure.Cosmos.CosmosElements
{
    using System;
    using Microsoft.Azure.Cosmos.Json;

    internal abstract partial class CosmosGuid : CosmosElement
    {
        protected CosmosGuid()
            : base(CosmosElementType.Guid)
        {
        }

        public abstract Guid Value
        {
            get;
        }

        public static CosmosGuid Create(
            IJsonNavigator jsonNavigator,
            IJsonNavigatorNode jsonNavigatorNode)
        {
            return new LazyCosmosGuid(jsonNavigator, jsonNavigatorNode);
        }

        public static CosmosGuid Create(Guid value)
        {
            return new EagerCosmosGuid(value);
        }

        public override void WriteTo(IJsonWriter jsonWriter)
        {
            if (jsonWriter == null)
            {
                throw new ArgumentNullException($"{nameof(jsonWriter)}");
            }

            jsonWriter.WriteGuidValue(this.Value);
        }
    }
}
