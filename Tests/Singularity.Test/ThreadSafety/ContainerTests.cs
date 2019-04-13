﻿using System;
using System.Collections.Generic;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.ThreadSafety
{
    public class ContainerThreadSafetyTests
    {
        [Fact]
        public void GetInstance()
        {
            var config = new BindingConfig();

            var registrations = new List<(Type abstractType, Type concreteType)>();

            registrations.Add((typeof(ITestService10), typeof(TestService10)));
            registrations.Add((typeof(ITestService11), typeof(TestService11)));
            registrations.Add((typeof(ITestService12), typeof(TestService12)));

            registrations.Add((typeof(ITestService20), typeof(TestService20)));
            registrations.Add((typeof(ITestService21), typeof(TestService21)));
            registrations.Add((typeof(ITestService22), typeof(TestService22)));

            registrations.Add((typeof(ITestService30), typeof(TestService30)));
            registrations.Add((typeof(ITestService31), typeof(TestService31)));
            registrations.Add((typeof(ITestService32), typeof(TestService32)));

            var disposeCount = 0;
            foreach ((Type abstractType, Type concreteType) registration in registrations)
            {
                config.Register(registration.abstractType, registration.concreteType).OnDeath(obj => disposeCount++);
            }

            disposeCount = 0;
            var container = new Container(config);
            var tester = new ThreadSafetyTester<(Type abstractType, Type concreteType)>();

            tester.Test(testCase =>
            {
                object concreteType = container.GetInstance(testCase.abstractType);
                Assert.Equal(testCase.concreteType, concreteType.GetType());
            }, registrations);

            container.Dispose();
            Assert.Equal(180, disposeCount);
        }
    }
}