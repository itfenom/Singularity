﻿using System.Collections.Generic;
using Singularity.Collections;
using Xunit;

namespace Singularity.Test.ThreadSafety
{
    public class ThreadSafeDictionaryTests
    {
        [Fact]
        public void AddAndGet()
        {
            var testCases = new List<(int input, object output)>();

            for (var i = 0; i < 1000; i++)
            {
                testCases.Add((i, new object()));
            }

            var dic = new ThreadSafeDictionary<int, object>();

            var tester = new ThreadSafetyTester<(int input, object expectedOutput)>();

            tester.Test(testCase =>
            {
                dic.Add(testCase.input, testCase.expectedOutput);
                object output = dic.Search(testCase.input);
                Assert.Equal(testCase.expectedOutput, output);
            }, testCases);

            Assert.Equal(testCases.Count * tester.TaskCount ,dic.Count);
        }
    }
}
