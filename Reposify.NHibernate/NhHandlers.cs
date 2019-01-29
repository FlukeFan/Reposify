namespace Reposify.NHibernate
{
    public class NhHandlers : AbstractHandlers
    {
        public NhHandlers UsingHandlersFromAssemblyForType<T>()
        {
            AddHandlersFromAssemblyForType<T>(
                executionHandlerInterface:      typeof(INhExecutionHandler<>),
                queryHandlerInterface:          typeof(INhQueryHandler<,>),
                executionAsyncHandlerInterface: null,
                queryAsyncHandlerInterface:     null);

            return this;
        }
    }
}
