﻿using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ScopeTests
    {
        [Fact]
        public void BeginScope_GetInstance_DisposeScope()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().With(Lifetime.PerScope).OnDeath(x => x.Dispose());

            var container = new Container(config);

            //ACT
            Scoped scope = container.BeginScope();
            var disposable = (Disposable)scope.GetInstance<IDisposable>();
            bool isDisposedBefore = disposable.IsDisposed;
            scope.Dispose();
            bool isDisposedAfter = disposable.IsDisposed;

            //ASSERT
            Assert.False(isDisposedBefore);
            container.Dispose();
            Assert.True(isDisposedAfter);
        }

        [Fact]
        public void BeginScope_GetInstance_AlwaysReturnsSameInstanceForTheSameScope()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IDisposable, Disposable>().With(Lifetime.PerScope).OnDeath(x => x.Dispose());

            var container = new Container(config);

            //ACT
            Scoped scope1 = container.BeginScope();
            var disposable1 = scope1.GetInstance<IDisposable>();
            var disposable2 = scope1.GetInstance<IDisposable>();

            Scoped scope2 = container.BeginScope();
            var disposable3 = scope2.GetInstance<IDisposable>();
            var disposable4 = scope2.GetInstance<IDisposable>();


            //ASSERT
            Assert.Same(disposable1, disposable2);
            Assert.Same(disposable3, disposable4);
            Assert.NotSame(disposable1, disposable3);
        }
    }
}