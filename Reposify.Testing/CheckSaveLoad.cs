﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Reposify.Testing
{
    public static class CheckSaveLoadExtensions
    {
        public static CheckSaveLoad<TEntity> CheckSaveLoad<TEntity>(this IRepository repository, TEntity entity) where TEntity : class, IEntity
        {
            return new CheckSaveLoad<TEntity>(entity, repository);
        }

        public static CheckSaveLoad<TEntity> CheckSaveLoad<TEntity>(this TEntity entity, IRepository repository) where TEntity : class, IEntity
        {
            return new CheckSaveLoad<TEntity>(entity, repository);
        }

        public static CheckSaveLoad<TEntity> ExcludeProperties<TEntity>(this CheckSaveLoad<TEntity> checkSaveLoad, params string[] excludedProperties) where TEntity : class, IEntity
        {
            checkSaveLoad.SetExcludedProperties(excludedProperties);
            return checkSaveLoad;
        }
    }

    public class CheckSaveLoad<TEntity> : CheckSaveLoad where TEntity : class, IEntity
    {
        private IRepository         _repository;
        private TEntity             _entity;

        public CheckSaveLoad(TEntity entity, IRepository repository)
        {
            _entity = entity;
            _repository = repository;
        }

        public virtual TEntity Check()
        {
            _repository.Save(_entity);
            (_repository as IUnitOfWork)?.Flush();

            var originalEntity = _entity;

            if (_repository is IIdentityMapClearable)
            {
                // clear the repository if it supports identity map
                (_repository as IIdentityMapClearable).Clear();
            }
            else
            {
                originalEntity = DeepClone(_entity);
                (_repository as IIdentityMapReloadable)?.ReloadAll();
            }

            var loadedEntity = _repository.Load<TEntity>(_entity.Id);
            var visitor = new PropertyVisitor(CheckProperty);
            Check(originalEntity, loadedEntity, visitor);
            return loadedEntity;
        }
    }

    public class CheckSaveLoad
    {
        public static PropertyVisitor.PropertyCheck CustomCheck = (prop, orig, comp, name) => { };

        private IList<string> _excludedProperties = new string[0];

        public class SaveLoadException : Exception
        {
            public SaveLoadException(string message) : base(message) { }
        }

        public void SetExcludedProperties(params string[] excludedProperties)
        {
            _excludedProperties = excludedProperties;
        }

        public virtual void Check(object originalEntity, object loadedEntity, PropertyVisitor visitor)
        {
            if (ReferenceEquals(originalEntity, loadedEntity))
                throw new SaveLoadException($"loaded entity {loadedEntity} should be a different instance to {loadedEntity}");

            visitor.Check(originalEntity, loadedEntity);
        }

        protected virtual void CheckProperty(PropertyInfo property, object original, object compare, string rootName)
        {
            var name = rootName + property.Name;

            if (_excludedProperties.Contains(name))
                return;

            var originalValue = property.GetValue(original, null);

            var type = property.PropertyType;
            var isValueType = type.IsValueType;

            if (!isValueType && originalValue == null)
                throw new SaveLoadException($"property {name} on {original} should be set to a not-null value to test persistence");

            if (isValueType)
            {
                var defaultValue = Activator.CreateInstance(property.PropertyType);

                if (originalValue.Equals(defaultValue))
                    throw new SaveLoadException($"property {name} on {original} should be set to a not-default value to test persistence");
            }

            var compareValue = property.GetValue(compare, null);

            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);

            if (isEnumerable)
            {
                var originalCount = (originalValue as IEnumerable).Cast<object>().Count();

                if (originalCount == 0)
                    throw new SaveLoadException($"property {name} on {original} should not be empty to test persistence");

                var compareCount = (compareValue as IEnumerable).Cast<object>().Count();

                if (originalCount != compareCount)
                    throw new SaveLoadException($"property {name} on {original} was {originalValue} which does not match length of {compareValue} on {compare}");
            }

            if ((isValueType || type == typeof(string)) && !originalValue.Equals(compareValue))
                throw new SaveLoadException($"property {name} on {original} was {originalValue} which does not match {compareValue} on {compare}");

            var isEntity = typeof(IEntity).IsAssignableFrom(type);

            if (isEntity)
            {
                var idProperty = originalValue.GetType().GetProperty("Id");
                var originalId = idProperty.GetValue(originalValue, null);
                var compareId = idProperty.GetValue(compareValue, null);

                if (!originalId.Equals(compareId))
                    throw new SaveLoadException($"property {name}.Id on {original} was {originalId} which does not match {compareValue} with Id {compareId}");
            }

            CustomCheck(property, original, compare, rootName);
        }

        protected virtual T DeepClone<T>(T original)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, original);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public class PropertyVisitor
        {
            public delegate void PropertyCheck(PropertyInfo property, object original, object compare, string rootName);

            private PropertyCheck _check;

            public PropertyVisitor(PropertyCheck check)
            {
                _check = check;
            }

            public virtual void Check(object original, object compare, string rootName = "")
            {
                var properties = original.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var property in properties)
                    _check(property, original, compare, rootName);
            }
        }
    }
}
