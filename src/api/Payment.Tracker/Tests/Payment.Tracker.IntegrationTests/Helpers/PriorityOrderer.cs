using System;
using System.Collections.Generic;
using System.Linq;
using Payment.Tracker.IntegrationTests.Attributes;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Payment.Tracker.IntegrationTests.Helpers
{
    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                var priority = 0;

                IEnumerable<IAttributeInfo> attributes = testCase
                    .TestMethod
                    .Method
                    .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName);
                foreach (IAttributeInfo attr in attributes)
                {
                    priority = attr.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority));
                }

                GetOrCreate(sortedMethods, priority).Add(testCase);
            }

            foreach (List<TTestCase> list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
            {
                list.Sort((x, y) => StringComparer
                    .OrdinalIgnoreCase
                    .Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
                foreach (TTestCase testCase in list)
                {
                    yield return testCase;
                }
            }
        }
        
        private static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            if (dictionary.TryGetValue(key, out TValue result))
            {
                return result;
            }

            result = new TValue();
            dictionary[key] = result;

            return result;
        }
    }
}