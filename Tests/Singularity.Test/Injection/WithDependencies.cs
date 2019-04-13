﻿using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class WithDependencies
    {
        [Fact]
        public void GetInstance_1Deep_DependenciesAreCorrectlyInjected()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<ITestService11>();

            //ASSERT
            Assert.IsType<TestService11>(value);
            Assert.IsType<TestService10>(value.TestService10);
        }

        [Fact]
        public void GetInstance_2Deep_DependenciesAreCorrectlyInjected()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();
            config.Register<ITestService12, TestService12>();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<ITestService12>();

            //ASSERT
            Assert.IsType<TestService12>(value);
            Assert.IsType<TestService11>(value.TestService11);
            Assert.IsType<TestService10>(value.TestService11.TestService10);
        }

        [Fact]
        public void GetInstance_1Deep_ReturnsNewInstancePerCall()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<ITestService11>();
            var value2 = container.GetInstance<ITestService11>();

            //ASSERT
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotSame(value1, value2);

            Assert.NotNull(value1.TestService10);
            Assert.NotNull(value2.TestService10);
            Assert.NotSame(value1.TestService10, value2.TestService10);
        }

        [Fact]
        public void GetInstance_1DeepAndPerContainerLifetime_ReturnsSameInstancePerCall()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>().With(Lifetime.PerContainer);
            config.Register<ITestService11, TestService11>();

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<ITestService11>();
            var value2 = container.GetInstance<ITestService11>();

            //ASSERT
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotSame(value1, value2);

            Assert.NotNull(value1.TestService10);
            Assert.NotNull(value2.TestService10);
            Assert.Same(value1.TestService10, value2.TestService10);
        }

        [Fact]
        public void GetInstance_2Deep_ReturnsNewInstancePerCall()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();
            config.Register<ITestService12, TestService12>();

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<ITestService12>();
            var value2 = container.GetInstance<ITestService12>();

            //ASSERT
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotSame(value1, value2);

            Assert.NotNull(value1.TestService11);
            Assert.NotNull(value2.TestService11);
            Assert.NotSame(value1.TestService11, value2.TestService11);

            Assert.NotNull(value1.TestService11.TestService10);
            Assert.NotNull(value2.TestService11.TestService10);
            Assert.NotSame(value1.TestService11.TestService10, value2.TestService11.TestService10);
        }

        [Fact]
        public void GetInstance_2DeepAndPerContainerLifetime_ReturnsNewInstancePerCall()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>().With(Lifetime.PerContainer);
            config.Register<ITestService11, TestService11>();
            config.Register<ITestService12, TestService12>();

            var container = new Container(config);

            //ACT
            var value1 = container.GetInstance<ITestService12>();
            var value2 = container.GetInstance<ITestService12>();

            //ASSERT
            Assert.NotNull(value1);
            Assert.NotNull(value2);
            Assert.NotSame(value1, value2);

            Assert.NotNull(value1.TestService11);
            Assert.NotNull(value2.TestService11);
            Assert.NotSame(value1.TestService11, value2.TestService11);

            Assert.NotNull(value1.TestService11.TestService10);
            Assert.NotNull(value2.TestService11.TestService10);
            Assert.Same(value1.TestService11.TestService10, value2.TestService11.TestService10);
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<TestService11>();

            //ASSERT
            Assert.IsType<TestService11>(value);
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_WithConcreteDependency_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<TestService11WithConcreteDependency>();

            //ASSERT
            Assert.IsType<TestService11WithConcreteDependency>(value);
            Assert.NotNull(value.TestService10);
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_WithConcreteDependency_2Deep_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<TestService12WithConcreteDependency>();

            //ASSERT
            Assert.IsType<TestService12WithConcreteDependency>(value);
            Assert.NotNull(value.TestService11);
        }

        [Fact]
        public void GetInstance_GetDependencyByConcreteType_WithMixedConcreteDependency_2Deep_ReturnsCorrectDependency()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();
            config.Register<ITestService11, TestService11>();

            var container = new Container(config);

            //ACT
            var value = container.GetInstance<TestService12WithMixedConcreteDependency>();

            //ASSERT
            Assert.IsType<TestService12WithMixedConcreteDependency>(value);
            var testService11 = Assert.IsType<TestService11>(value.TestService11);
            Assert.IsType<TestService10>(testService11.TestService10);
        }

        [Fact]
        public void GetInstance_1DeepAndUsingDependencyFromParentContainer_CorrectDependencyIsReturned()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var nestedConfig = new BindingConfig();
            nestedConfig.Register<ITestService11, TestService11>();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig);

            //ACT
            var value = container.GetInstance<ITestService10>();
            var nestedValue = nestedContainer.GetInstance<ITestService11>();

            //ASSERT
            Assert.IsType<TestService10>(value);
            Assert.IsType<TestService11>(nestedValue);
        }

        [Fact]
        public void GetInstance_2DeepAndUsingDependencyFromParentContainer_CorrectDependencyIsReturned()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<ITestService10, TestService10>();

            var nestedConfig = new BindingConfig();
            nestedConfig.Register<ITestService11, TestService11>();

            var nestedConfig2 = new BindingConfig();
            nestedConfig2.Register<ITestService12, TestService12>();

            var container = new Container(config);
            Container nestedContainer = container.GetNestedContainer(nestedConfig);
            Container nestedContainer2 = nestedContainer.GetNestedContainer(nestedConfig2);

            //ACT
            var value = container.GetInstance<ITestService10>();
            var nestedValue = nestedContainer.GetInstance<ITestService11>();
            var nestedValue2 = nestedContainer2.GetInstance<ITestService12>();

            //ASSERT

            Assert.IsType<TestService10>(value);

            Assert.IsType<TestService11>(nestedValue);
            Assert.IsType<TestService10>(nestedValue.TestService10);

            Assert.IsType<TestService12>(nestedValue2);
            Assert.IsType<TestService11>(nestedValue2.TestService11);
            Assert.IsType<TestService10>(nestedValue2.TestService11.TestService10);
        }
    }
}
