namespace Reposify.NHibernate
{
    public class NhHandlers : AbstractHandlers
    {
        public NhHandlers UsingHandlersFromAssemblyForType<T>()
        {
            AddHandlersFromAssemblyForType<T>(
                isExecutionHandler: t => t == typeof(INhExecutionHandler<>),
                executionHandlerInterfaceName: "INhExecutionHandler",
                isQueryHandler: t => t == typeof(INhQueryHandler<,>),
                queryHandlerInterfaceName: "INhQueryHandler");

            return this;
        }
    }
}
