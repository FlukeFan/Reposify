namespace Reposify.NHibernate
{
    public class NhHandlers : AbstractHandlers
    {
        public NhHandlers UsingHandlersFromAssemblyForType<T>()
        {
            AddHandlersFromAssemblyForType<T>(
                executionHandlerInterface: typeof(INhExecutionHandler<>),
                queryHandlerInterface: typeof(INhQueryHandler<,>));

            return this;
        }
    }
}
