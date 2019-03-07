﻿namespace Microsoft.Azure.Cosmos.CosmosElements
{
    using Microsoft.Azure.Cosmos.Json;
    using System;

    internal sealed class LazyCosmosNumber : CosmosNumber, ILazyCosmosElement
    {
        private readonly IJsonNavigator jsonNavigator;
        private readonly IJsonNavigatorNode jsonNavigatorNode;

        // TODO: replace this with Number64 when the time comes.
        private readonly Lazy<double> lazyNumber;

        public LazyCosmosNumber(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
        {
            if (jsonNavigator == null)
            {
                throw new ArgumentNullException($"{nameof(jsonNavigator)} must not be null.");
            }

            if (jsonNavigatorNode == null)
            {
                throw new ArgumentNullException($"{nameof(jsonNavigatorNode)} must not be null.");
            }

            JsonNodeType type = jsonNavigator.GetNodeType(jsonNavigatorNode);
            if (type != JsonNodeType.Number)
            {
                throw new ArgumentException($"{nameof(jsonNavigatorNode)} must not be a number node. Got {type} instead.");
            }

            this.jsonNavigator = jsonNavigator;
            this.jsonNavigatorNode = jsonNavigatorNode;
            this.lazyNumber = new Lazy<double>(() => 
            {
                return this.jsonNavigator.GetNumberValue(this.jsonNavigatorNode);
            });
        }

        public override bool IsDouble
        {
            get
            {
                // Until we have Number64 a LazyCosmosNumber is always a double.
                return true;
            }
        }

        public override bool IsInteger
        {
            get
            {
                // Until we have Number64 a LazyCosmosNumber is always a double.
                return false;
            }
        }

        public override double? AsDouble()
        {
            return this.lazyNumber.Value;
        }

        public override long? AsLong()
        {
            return null;
        }

        public void WriteToWriter(IJsonWriter jsonWriter)
        {
            if (jsonWriter == null)
            {
                throw new ArgumentNullException($"{nameof(jsonWriter)} must not be null.");
            }

            jsonWriter.WriteJsonNode(this.jsonNavigator, jsonNavigatorNode);
        }
    }
}
