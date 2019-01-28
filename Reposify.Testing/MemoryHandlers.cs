namespace Reposify.Testing
{
    public class MemoryHandlers : AbstractHandlers
    {
        public MemoryHandlers UsingHandlersFromAssemblyForType<T>()
        {
            AddHandlersFromAssemblyForType<T>(
                isExecutionHandler: t => t == typeof(IMemoryExecutionHandler<>),
                executionHandlerInterfaceName: "IMemoryExecutionHandler",
                isQueryHandler: t => t == typeof(IMemoryQueryHandler<,>),
                queryHandlerInterfaceName: "IMemoryQueryHandler");

            return this;
        }
    }
}
