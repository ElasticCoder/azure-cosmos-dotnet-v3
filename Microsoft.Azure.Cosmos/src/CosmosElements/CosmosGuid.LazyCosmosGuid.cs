﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Microsoft.Azure.Cosmos.CosmosElements
{
    using System;
    using Microsoft.Azure.Cosmos.Json;

    internal abstract partial class CosmosGuid : CosmosElement
    {
        private sealed class LazyCosmosGuid : CosmosGuid
        {
            private readonly Lazy<Guid> lazyGuid;

            public LazyCosmosGuid(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
            {
                if (jsonNavigator == null)
                {
                    throw new ArgumentNullException($"{nameof(jsonNavigator)}");
                }

                if (jsonNavigatorNode == null)
                {
                    throw new ArgumentNullException($"{nameof(jsonNavigatorNode)}");
                }

                JsonNodeType type = jsonNavigator.GetNodeType(jsonNavigatorNode);
                if (type != JsonNodeType.Guid)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(jsonNavigatorNode)} must be a {JsonNodeType.Guid} node. Got {type} instead.");
                }

                this.lazyGuid = new Lazy<Guid>(() =>
                {
                    return jsonNavigator.GetGuidValue(jsonNavigatorNode);
                });
            }

            public override Guid Value
            {
                get
                {
                    return this.lazyGuid.Value;
                }
            }
        }
    }
}
