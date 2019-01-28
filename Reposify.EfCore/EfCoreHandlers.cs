namespace Reposify.EfCore
{
    public class EfCoreHandlers : AbstractHandlers
    {
        public EfCoreHandlers UsingHandlersFromAssemblyForType<T>()
        {
            AddHandlersFromAssemblyForType<T>(
                executionHandlerInterface: typeof(IEfCoreExecutionHandler<>),
                queryHandlerInterface: typeof(IEfCoreQueryHandler<,>));

            return this;
        }
    }
}
