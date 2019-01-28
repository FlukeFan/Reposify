using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            Type executionHandlerInterface,
            Type queryHandlerInterface)
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

                    if (genericType == executionHandlerInterface)
                        _executionHandlers[intrface.GenericTypeArguments[0]] = type;

                    if (genericType == queryHandlerInterface)
                        _queryHandlers[intrface.GenericTypeArguments[0]] = type;
                }
            }

            _executionHandlerInterfaceName = executionHandlerInterface.Name.Split('`')[0];
            _queryHandlerInterfaceName = queryHandlerInterface.Name.Split('`')[0];
        }

        public virtual void Execute(IDbExecutor executor, IDbExecution dbExecution)
        {
            var handlerType = GetHandlerType(dbExecution, _executionHandlers, _executionHandlerInterfaceName);
            var handler = HandlerFactory(handlerType);
            var execute = GetExecuteMethod(handlerType, _executionHandlerInterfaceName, "Execute");

            execute.Invoke(handler, new object[] { executor, dbExecution });
        }

        public virtual TResult Execute<TResult>(IDbExecutor executor, IDbQuery<TResult> dbQuery)
        {
            var handlerType = GetHandlerType(dbQuery, _queryHandlers, _queryHandlerInterfaceName);
            var handler = HandlerFactory(handlerType);
            var execute = GetExecuteMethod(handlerType, _queryHandlerInterfaceName, "Execute");

            var result = execute.Invoke(handler, new object[] { executor, dbQuery });
            return (TResult)result;
        }

        protected virtual Type GetHandlerType(object action, IDictionary<Type, Type> handlers, string interfaceName)
        {
            if (action == null)
                throw new Exception("attempt to execute null query");

            var type = action.GetType();

            if (!handlers.ContainsKey(type))
                throw new Exception($"no handler found for {type} - ensure there is a handler registered that implements {interfaceName}<{type.Name}>");

            var handlerType = handlers[type];
            return handlerType;
        }

        protected virtual MethodInfo GetExecuteMethod(Type handlerType, string interfaceName, string methodName)
        {
            var execute = handlerType.GetMethod(methodName);

            if (execute == null)
                execute = handlerType.GetInterfaces().Where(i => i.Name.StartsWith(interfaceName)).Single().GetMethod(methodName);

            return execute;
        }
    }
}
