using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace TestCommon
{
    public abstract class UnitTestBase<TClassInTest> where TClassInTest : class
    {
        protected TClassInTest ClassInTest;

        protected IResolveConstraint ThrowsArgumentException(string paramName, string message)
        {
            var expected = new ArgumentException(message, paramName);
            return Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName))
                .EqualTo(expected.ParamName)
                .And
                .Property(nameof(ArgumentException.Message))
                .EqualTo(expected.Message);
        }

        protected IResolveConstraint ThrowsArgumentNullException(string paramName)
        {
            var expected = new ArgumentNullException(paramName);
            return Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName))
                .EqualTo(expected.ParamName)
                .And
                .Property(nameof(ArgumentException.Message))
                .EqualTo(expected.Message);
        }
    }

    public static class NunitExtensions
    {
        public static EqualConstraint WithPropEqual(this ConstraintExpression expr, string propName, object expectedVal)
        {
            return expr.With.Property(propName).EqualTo(expectedVal);
        }
    }
}