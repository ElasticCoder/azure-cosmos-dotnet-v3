//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Azure.Cosmos.Sql;
    using static Microsoft.Azure.Cosmos.Linq.FromParameterBindings;

    internal class SqlBuiltinFunctionVisitor : BuiltinFunctionVisitor
    {
        public SqlBuiltinFunctionVisitor(string sqlName, bool isStatic, List<Type[]> argumentLists)
        {
            this.SqlName = sqlName;
            this.IsStatic = isStatic;
            this.ArgumentLists = argumentLists;
        }

        public string SqlName { get; private set; }

        public bool IsStatic { get; private set; }

        public List<Type[]> ArgumentLists { get; private set; }

        protected override SqlScalarExpression VisitExplicit(MethodCallExpression methodCallExpression, TranslationContext context)
        {
            // Try to match one if the argument lists
            if (this.ArgumentLists != null)
            {
                foreach (Type[] arguments in this.ArgumentLists)
                {
                    if (MatchArgumentLists(methodCallExpression.Arguments, arguments))
                    {
                        return VisitBuiltinFunction(methodCallExpression, context);
                    }
                }
            }

            return null;
        }

        protected override SqlScalarExpression VisitImplicit(MethodCallExpression methodCallExpression, TranslationContext context)
        {
            return null;
        }

        private bool MatchArgumentLists(ReadOnlyCollection<Expression> methodCallArguments, Type[] expectedArguments)
        {
            if (methodCallArguments.Count != expectedArguments.Length)
            {
                return false;
            }

            for (int i = 0; i < expectedArguments.Length; i++)
            {
                if (methodCallArguments[i].Type != expectedArguments[i] &&
                    !expectedArguments[i].IsAssignableFrom(methodCallArguments[i].Type))
                {
                    return false;
                }
            }

            return true;
        }

        private SqlScalarExpression VisitBuiltinFunction(MethodCallExpression methodCallExpression, TranslationContext context)
        {
            List<SqlScalarExpression> arguments = new List<SqlScalarExpression>();

            if (methodCallExpression.Object != null)
            {
                arguments.Add(ExpressionToSql.VisitNonSubqueryScalarExpression(methodCallExpression.Object, context));
            }

            foreach (Expression argument in methodCallExpression.Arguments)
            {
                arguments.Add(ExpressionToSql.VisitNonSubqueryScalarExpression(argument, context));
            }

            return SqlFunctionCallScalarExpression.CreateBuiltin(this.SqlName, arguments.ToArray());
        }
    }
}
