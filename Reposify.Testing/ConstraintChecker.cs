using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reposify.Testing
{
    public class ConstraintChecker
    {
        public static readonly DateTime MinSqlServerDateTime = new DateTime(1753, 1, 1, 0, 0, 0);

        private bool _isMsSql;

        public ConstraintChecker() : this(true) { }

        public ConstraintChecker(bool isMsSql)
        {
            _isMsSql = isMsSql;
        }

        public virtual void BeforeSave(object entity)
        {
            var type = entity.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
                CheckProperty(entity, property);
        }

        protected virtual void CheckProperty(object entity, PropertyInfo property)
        {
            var type = property.PropertyType;

            if (type == typeof(DateTime) && _isMsSql)
                CheckMsSqlDateTime(property.Name, (DateTime)property.GetValue(entity));

            if (type == typeof(DateTime?) && _isMsSql)
                CheckNullableMsSqlDateTime(property.Name, (DateTime?)property.GetValue(entity));
        }

        public void Check<T>(Expression<Func<T>> property, Action<T> validate)
        {
            var value = property.Compile().Invoke();
            Action validateAction = () => validate(value);
            Check(Builder.GetPropertyName(property.Body), validateAction);
        }

        public void Check(string propertyName, Action validate)
        {
            try {
                validate();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("property {0} is not valid: {1}", propertyName, e.Message), e);
            }
        }

        public void CheckMsSqlDateTime(Expression<Func<DateTime>> property)
        {
            CheckMsSqlDateTime(Builder.GetPropertyName(property.Body), property.Compile().Invoke());
        }

        public void CheckMsSqlDateTime(string propertyName, DateTime dateTime)
        {
            if (dateTime < MinSqlServerDateTime)
                throw new Exception(string.Format("DateTime property {0} with value {1} cannot be stored in SQL Server", propertyName, dateTime));
        }

        public void CheckNullableMsSqlDateTime(Expression<Func<DateTime?>> property)
        {
            CheckNullableMsSqlDateTime(Builder.GetPropertyName(property.Body), property.Compile().Invoke());
        }

        public void CheckNullableMsSqlDateTime(string propertyName, DateTime? dateTime)
        {
            if (dateTime.HasValue && dateTime < MinSqlServerDateTime)
                throw new Exception(string.Format("DateTime property {0} with value {1} cannot be stored in SQL Server", propertyName, dateTime));
        }

        public void CheckNotNull(Expression<Func<object>> property)
        {
            CheckNotNull(Builder.GetPropertyName(property.Body), property.Compile().Invoke());
        }

        public void CheckNotNull(string propertyName, object value)
        {
            if (value == null)
                throw new Exception(string.Format("property {0} cannot be null", propertyName));
        }

        public void CheckNotNullOrEmpty(Expression<Func<string>> property)
        {
            CheckNotNullOrEmpty(Builder.GetPropertyName(property.Body), property.Compile().Invoke());
        }

        public void CheckNotNullOrEmpty(string propertyName, string value)
        {
            CheckNotNull(propertyName, value);

            if (value == string.Empty)
                throw new Exception(string.Format("string property {0} cannot be empty", propertyName));
        }

        public void CheckMaxLength(Expression<Func<string>> property, int maxLength)
        {
            CheckMaxLength(Builder.GetPropertyName(property.Body), property.Compile().Invoke(), maxLength);
        }

        public void CheckMaxLength(string propertyName, string value, int maxLength)
        {
            if (value != null && value.Length > maxLength)
                throw new Exception(string.Format("string property {0} has length {1} which is larger than the maximum length of {2}", propertyName, value.Length, maxLength));
        }
    }
}
