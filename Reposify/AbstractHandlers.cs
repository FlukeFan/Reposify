using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Reposify
{
    public abstract class AbstractHandlers
    {
        private string _executionHandlerInterfaceName       = "unregistered interface IDbExecution";
        private string _queryHandlerInterfaceName           = "unregistered interface IDbQuery";
        private string _executionAsyncHandlerInterfaceName  = "unregistered interface async IDbExecution";
        private string _queryAsyncHandlerInterfaceName      = "unregistered interface async IDbQuery";

        private IDictionary<Type, Type> _executionHandlers      = new Dictionary<Type, Type>();
        private IDictionary<Type, Type> _queryHandlers          = new Dictionary<Type, Type>();
        private IDictionary<Type, Type> _executionAsyncHandlers = new Dictionary<Type, Type>();
        private IDictionary<Type, Type> _queryAsyncHandlers     = new Dictionary<Type, Type>();

        public Func<Type, object> HandlerFactory = t => Activator.CreateInstance(t);

        protected void AddHandlersFromAssemblyForType<T>(
            Type executionHandlerInterface,
            Type queryHandlerInterface,
            Type executionAsyncHandlerInterface,
            Type queryAsyncHandlerInterface)
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

                    if (executionHandlerInterface != null && genericType == executionHandlerInterface)
                        AddHandler(_executionHandlers, intrface.GenericTypeArguments[0], type);

                    if (queryHandlerInterface != null && genericType == queryHandlerInterface)
                        AddHandler(_queryHandlers, intrface.GenericTypeArguments[0], type);

                    if (executionAsyncHandlerInterface != null && genericType == executionAsyncHandlerInterface)
                        AddHandler(_executionAsyncHandlers, intrface.GenericTypeArguments[0], type);

                    if (queryAsyncHandlerInterface != null && genericType == queryAsyncHandlerInterface)
                        AddHandler(_queryAsyncHandlers, intrface.GenericTypeArguments[0], type);
                }
            }

            if (executionHandlerInterface != null)
                _executionHandlerInterfaceName = executionHandlerInterface.Name.Split('`')[0];

            if (queryHandlerInterface != null)
                _queryHandlerInterfaceName = queryHandlerInterface.Name.Split('`')[0];

            if (executionAsyncHandlerInterface != null)
                _executionAsyncHandlerInterfaceName = executionAsyncHandlerInterface.Name.Split('`')[0];

            if (queryAsyncHandlerInterface != null)
                _queryAsyncHandlerInterfaceName = queryAsyncHandlerInterface.Name.Split('`')[0];
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

        public virtual Task ExecuteAsync(IDbExecutor executor, IDbExecution dbExecution)
        {
            var handlerType = GetHandlerType(dbExecution, _executionAsyncHandlers, _executionAsyncHandlerInterfaceName);
            var handler = HandlerFactory(handlerType);
            var execute = GetExecuteMethod(handlerType, _executionAsyncHandlerInterfaceName, "ExecuteAsync");

            var result = execute.Invoke(handler, new object[] { executor, dbExecution });
            return (Task)result;
        }

        public virtual Task<TResult> ExecuteAsync<TResult>(IDbExecutor executor, IDbQuery<TResult> dbQuery)
        {
            var handlerType = GetHandlerType(dbQuery, _queryAsyncHandlers, _queryAsyncHandlerInterfaceName);
            var handler = HandlerFactory(handlerType);
            var execute = GetExecuteMethod(handlerType, _queryAsyncHandlerInterfaceName, "ExecuteAsync");

            var result = execute.Invoke(handler, new object[] { executor, dbQuery });
            return (Task<TResult>)result;
        }

        protected virtual void AddHandler(IDictionary<Type, Type> executionHandlers, Type queryObject, Type handler)
        {
            if (executionHandlers.ContainsKey(queryObject))
                throw new Exception($"Duplicate handler for {queryObject}: {executionHandlers[queryObject]} and {handler}");

            executionHandlers.Add(queryObject, handler);
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
