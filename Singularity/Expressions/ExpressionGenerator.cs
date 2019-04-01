﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Graph;

namespace Singularity.Expressions
{
    internal class ExpressionGenerator
    {
        private static readonly MethodInfo AddMethod = typeof(ObjectActionList).GetRuntimeMethod(nameof(ObjectActionList.Add), new[] { typeof(object) });

        public ResolvedDependency GenerateDependencyExpression(Dependency dependency, Scoped scope)
        {
            Expression expression = dependency.Binding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : dependency.Binding.Expression;
            var expressionVisitor = new ReplaceExpressionVisitor();
            foreach (ParameterExpression unresolvedParameter in expression.GetParameterExpressions())
            {
                var nestedDependency = dependency.Dependencies.First(x => x.Binding.DependencyType == unresolvedParameter.Type);

                expression = expressionVisitor.Visit(expression, unresolvedParameter, nestedDependency.ResolvedDependency!.Expression);
            }

            if (dependency.Binding.OnDeathAction != null || dependency.Binding.Decorators.Length > 0)
            {
                var body = new List<Expression>();
                ParameterExpression instanceParameter = Expression.Variable(dependency.Binding.DependencyType, $"{expression.Type} instance");
                body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependency.Binding.DependencyType)));

                if (dependency.Binding.OnDeathAction != null)
                {
                    ObjectActionList actionList = scope.GetActionList(dependency.Binding.Expression.Type, dependency.Binding.OnDeathAction);
                    body.Add(Expression.Call(Expression.Constant(actionList), AddMethod, instanceParameter));
                }

                if (dependency.Binding.Decorators.Length > 0)
                {
                    Expression previousDecorator = instanceParameter;
                    foreach (Expression decorator in dependency.Binding.Decorators)
                    {
                        Expression decoratorExpression = decorator;
                        foreach (ParameterExpression unresolvedParameter in decorator.GetParameterExpressions())
                        {
                            if (unresolvedParameter.Type == instanceParameter.Type)
                            {
                                decoratorExpression = expressionVisitor.Visit(decoratorExpression, unresolvedParameter, previousDecorator);
                            }
                            else
                            {
                                var decoratorDependency = dependency.Dependencies.First(x => x.Binding.DependencyType == unresolvedParameter.Type);
                                decoratorExpression = expressionVisitor.Visit(decoratorExpression, unresolvedParameter, decoratorDependency.ResolvedDependency!.Expression);
                            }

                        }

                        previousDecorator = decoratorExpression;
                    }
                    body.Add(previousDecorator);
                }

                if (body.Last().Type == typeof(void)) body.Add(instanceParameter);
                expression = body.Count == 1 ? expression : Expression.Block(new[] { instanceParameter }, body);
            }

            if (dependency.Binding.CreationMode is CreationMode.Singleton)
            {
                Delegate action = Expression.Lambda(expression).Compile();
                object value = action.DynamicInvoke();
                expression = Expression.Constant(value, dependency.Binding.DependencyType);
                return new ResolvedDependency(expression, () => value);
            }

            var instanceFactory = (Func<object>)Expression.Lambda(expression).Compile();
            return new ResolvedDependency(expression, instanceFactory);
        }
    }
}
