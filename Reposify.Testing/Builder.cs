using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reposify.Testing
{
    public class Builder
    {
        /// <summary>
        ///  Utility method to create objects with protected constructor
        ///  e.g., Apple apple = ObjectBuilder.Create&lt;Apple&gt;();
        /// </summary>
        public static T Create<T>()
        {
            return Create<T>(false);
        }
        
        public static T Create<T>(bool allowPublicConstructor)
        {
            var flags = allowPublicConstructor
                ? BindingFlags.Instance | BindingFlags.Public
                : BindingFlags.Instance | BindingFlags.NonPublic;

            var constructors = typeof(T).GetConstructors(flags);

            if ((constructors == null) || (constructors.Length == 0))
            {
                var message = allowPublicConstructor
                    ? "Couldn't find default constructor on type " + typeof(T).Name
                    : "Couldn't find non-public default constructor on type " + typeof(T).Name;

                throw new System.Exception(message);
            }

            return (T)constructors[0].Invoke(null);
        }

        public static Builder<T> Modify<T>(T instance)
        {
            return new Builder<T>(instance);
        }

        protected static PropertyInfo GetPropertyInfo(Expression body)
        {
            MemberExpression me;

            if (body is UnaryExpression)
                body = (body as UnaryExpression).Operand;

            me = (MemberExpression)body;
            return (PropertyInfo)me.Member;
        }

        public static string GetPropertyName(Expression body)
        {
            return GetPropertyInfo(body).Name;
        }

        public static void SetField(object instance, string fieldName, object value)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(instance, value);
        }

        public static object GetField(object instance, string fieldName)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            var value = field.GetValue(instance);
            return value;
        }
    }

    public class Builder<T> : Builder
    {
        protected T     _instance;
        protected bool  _allowPublicMutators;

        public Builder() : this(Create<T>()) { }

        public Builder(T instance, bool allowPublicMutators = false)
        {
            if (instance == null)
                throw new ArgumentNullException("Cannot have null instance when constructing " + GetType());

            _instance = instance;
            _allowPublicMutators = allowPublicMutators;
        }

        public Builder<T> With<U>(Expression<Func<T, U>> propertyFunction, U value)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyFunction.Body);

            if (!_allowPublicMutators && propertyInfo.GetSetMethod() != null)
                throw new System.Exception("Property '" + propertyInfo.Name + "' is not protected on " + _instance.GetType());

            if (!propertyInfo.CanWrite)
                throw new System.Exception("Property '" + propertyInfo.Name + "' does not have a mutator on " + _instance.GetType());

            propertyInfo.SetValue(_instance, value, null);
            return this;
        }

        public Builder<T> WithField(string fieldName, object value)
        {
            SetField(_instance, fieldName, value);
            return this;
        }

        public T Value()
        {
            return _instance;
        }
    }
}
