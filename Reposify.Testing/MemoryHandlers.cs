namespace Reposify.Testing
{
    public class MemoryHandlers : AbstractHandlers
    {
        public MemoryHandlers UsingHandlersFromAssemblyForType<T>()
        {
            AddHandlersFromAssemblyForType<T>(
                executionHandlerInterface:      typeof(IMemoryExecutionHandler<>),
                queryHandlerInterface:          typeof(IMemoryQueryHandler<,>),
                executionAsyncHandlerInterface: typeof(IMemoryExecutionAsyncHandler<>),
                queryAsyncHandlerInterface:     typeof(IMemoryQueryAsyncHandler<,>));

            return this;
        }
    }
}
