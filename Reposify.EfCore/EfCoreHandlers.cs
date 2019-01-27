using System;
using System.Collections.Generic;
using System.Linq;

namespace Reposify.EfCore
{
    public class EfCoreHandlers
    {
        protected IDictionary<Type, Type>   _executionHandlers  = new Dictionary<Type, Type>();
        protected IDictionary<Type, Type>   _queryHandlers      = new Dictionary<Type, Type>();

        public Func<Type, object> HandlerFactory = t => Activator.CreateInstance(t);

        public EfCoreHandlers UsingHandlersFromAssemblyForType<T>()
        {
            var assembly = typeof(T).Assembly;
            var types = assembly.GetTypes()
                .Where(t => !t.IsAbstract)
                .ToList();

            foreach (var type in types)
            {
                foreach (var intrface in type.GetInterfaces())
                {
                    if (!intrface.IsGenericType)
                        continue;

                    var genericType = intrface.GetGenericTypeDefinition();

                    if (genericType == typeof(IEfCoreExecutionHandler<>))
                        _executionHandlers[intrface.GenericTypeArguments[0]] = type;

                    if (genericType == typeof(IEfCoreQueryHandler<,>))
                        _queryHandlers[intrface.GenericTypeArguments[0]] = type;
                }
            }

            return this;
        }

        public virtual void Execute(EfCoreRepository repository, IDbExecution dbExecution)
        {
            if (dbExecution == null)
                throw new Exception("attempt to execute null query");

            var type = dbExecution.GetType();

            if (!_executionHandlers.ContainsKey(type))
                throw new Exception($"no handler found for {type} - ensure there is a handler registered that implements IEfCoreExecutionHandler<{type.Name}>");

            var handlerType = _executionHandlers[type];
            var handler = HandlerFactory(handlerType);
            var execute = handlerType.GetMethod("Execute");

            if (execute == null)
                execute = handlerType.GetInterfaces().Where(i => i.Name.StartsWith("IEfCoreExecutionHandler")).Single().GetMethod("Execute");

            execute.Invoke(handler, new object[] { repository, dbExecution });
        }

        public virtual TResult Execute<TResult>(EfCoreRepository repository, IDbQuery<TResult> dbQuery)
        {
            if (dbQuery == null)
                throw new Exception("attempt to execute null query");

            var type = dbQuery.GetType();

            if (!_queryHandlers.ContainsKey(type))
                throw new Exception($"no handler found for {type} - ensure there is a handler registered that implements IEfCoreQueryHandler<{type.Name}>");

            var handlerType = _queryHandlers[type];
            var handler = HandlerFactory(handlerType);
            var execute = handlerType.GetMethod("Execute");

            if (execute == null)
                execute = handlerType.GetInterfaces().Where(i => i.Name.StartsWith("IEfCoreQueryHandler")).Single().GetMethod("Execute");

            var result = execute.Invoke(handler, new object[] { repository, dbQuery });
            return (TResult)result;
        }
    }
}
