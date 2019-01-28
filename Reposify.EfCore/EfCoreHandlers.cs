namespace Reposify.EfCore
{
    public class EfCoreHandlers : AbstractHandlers
    {
        public EfCoreHandlers UsingHandlersFromAssemblyForType<T>()
        {
            AddHandlersFromAssemblyForType<T>(
                isExecutionHandler: t => t == typeof(IEfCoreExecutionHandler<>),
                executionHandlerInterfaceName: "IEfCoreExecutionHandler",
                isQueryHandler: t => t == typeof(IEfCoreQueryHandler<,>),
                queryHandlerInterfaceName: "IEfCoreQueryHandler");

            return this;
        }
    }
}
