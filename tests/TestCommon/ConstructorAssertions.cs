using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;

namespace TestCommon
{
    [ExcludeFromCodeCoverage]
    public static class ConstructorAssertions
    {
        public static void ConstructsWithMockedParameters<TClassInTest>() where TClassInTest : class
        {
            var constructors = typeof(TClassInTest).GetConstructors();

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                var mocked = parameters.Select(s => MockType(s.ParameterType)).ToArray();

                Assert.DoesNotThrow(() => constructor.Invoke(mocked));
            }
        }

        public static void ClassIsGuardedAgainstNull<TClassInTest>() where TClassInTest : class
        {
            var constructors = typeof(TClassInTest).GetConstructors();

            foreach (var constructor in constructors)
            {
                ConstructorIsGuardedAgainstNull(constructor);
            }
        }

        private static void ConstructorIsGuardedAgainstNull(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();

            for (var i = 0; i < parameters.Length; i++)
            {
                var parametersMocked = parameters.Select(s => MockType(s.ParameterType)).ToArray();

                parametersMocked[i] = null;

                var ex = Assert.Throws<TargetInvocationException>(() => constructorInfo.Invoke(parametersMocked));

                Assert.IsInstanceOf<ArgumentNullException>(ex.InnerException);

                var argumentNullException = (ArgumentNullException)ex.InnerException;

                Assert.AreEqual(parameters.ElementAt(i).Name, argumentNullException?.ParamName);
            }
        }

        private static object MockType(Type typeToMock)
        {
            if (typeToMock.IsValueType)
                return Activator.CreateInstance(typeToMock);

            if (!typeToMock.IsInterface && !typeToMock.IsClass)
                throw new NotSupportedException($"{typeToMock.Name} cannot be mocked.");

            var mockMethod = typeof(Mock).GetMethods()
                .Single(s => s.Name == nameof(Mock.Of) && s.IsGenericMethod && !s.GetParameters().Any());

            var genericMethod = mockMethod.MakeGenericMethod(typeToMock);

            return genericMethod.Invoke(null, null);
        }
    }
}