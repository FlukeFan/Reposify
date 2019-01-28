using System;
using System.Collections.Generic;
using System.Linq;

namespace Reposify
{
    public abstract class AbstractHandlers
    {
        private string _executionHandlerInterfaceName = "IDbExecution";
        private string _queryHandlerInterfaceName = "IDbExecution";

        private IDictionary<Type, Type> _executionHandlers  = new Dictionary<Type, Type>();
        private IDictionary<Type, Type> _queryHandlers      = new Dictionary<Type, Type>();

        public Func<Type, object> HandlerFactory = t => Activator.CreateInstance(t);

        protected void AddHandlersFromAssemblyForType<T>(
            Func<Type, bool>    isExecutionHandler,
            string              executionHandlerInterfaceName,
            Func<Type, bool>    isQueryHandler,
            string              queryHandlerInterfaceName)
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

                    if (isExecutionHandler(genericType))
                        _executionHandlers[intrface.GenericTypeArguments[0]] = type;

                    if (isQueryHandler(genericType))
                        _queryHandlers[intrface.GenericTypeArguments[0]] = type;
                }
            }

            _executionHandlerInterfaceName = executionHandlerInterfaceName;
            _queryHandlerInterfaceName = queryHandlerInterfaceName;
        }

        public virtual void Execute(IDbExecutor executor, IDbExecution dbExecution)
        {
            if (dbExecution == null)
                throw new Exception("attempt to execute null query");

            var type = dbExecution.GetType();

            if (!_executionHandlers.ContainsKey(type))
                throw new Exception($"no handler found for {type} - ensure there is a handler registered that implements {_executionHandlerInterfaceName}<{type.Name}>");

            var handlerType = _executionHandlers[type];
            var handler = HandlerFactory(handlerType);
            var execute = handlerType.GetMethod("Execute");

            if (execute == null)
                execute = handlerType.GetInterfaces().Where(i => i.Name.StartsWith(_executionHandlerInterfaceName)).Single().GetMethod("Execute");

            execute.Invoke(handler, new object[] { executor, dbExecution });
        }

        public virtual TResult Execute<TResult>(IDbExecutor executor, IDbQuery<TResult> dbQuery)
        {
            if (dbQuery == null)
                throw new Exception("attempt to execute null query");

            var type = dbQuery.GetType();

            if (!_queryHandlers.ContainsKey(type))
                throw new Exception($"no handler found for {type} - ensure there is a handler registered that implements {_queryHandlerInterfaceName}<{type.Name}>");

            var handlerType = _queryHandlers[type];
            var handler = HandlerFactory(handlerType);
            var execute = handlerType.GetMethod("Execute");

            if (execute == null)
                execute = handlerType.GetInterfaces().Where(i => i.Name.StartsWith(_queryHandlerInterfaceName)).Single().GetMethod("Execute");

            var result = execute.Invoke(handler, new object[] { executor, dbQuery });
            return (TResult)result;
        }
    }
}
