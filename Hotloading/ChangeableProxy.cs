using Castle.DynamicProxy;

namespace MHGameWork.Hotloading
{
    public class ChangeableProxy<T> where T : class
    {
        public T Proxy { get; private set; }
        public T Target { get { return interceptor.Target; } set { interceptor.Target = value; } }

        private ChangeableTargetInterceptor<T> interceptor;

        public ChangeableProxy(ProxyGenerator gen)
        {

            interceptor = new ChangeableTargetInterceptor<T>();
            Proxy = gen.CreateInterfaceProxyWithTargetInterface<T>(null, interceptor);
            interceptor.Proxy = Proxy;
        }

        private class ChangeableTargetInterceptor<T> : IInterceptor
        {
            public T Target { get; set; }
            public T Proxy { get; set; }
            public void Intercept(IInvocation invocation)
            {
                ((IChangeProxyTarget)invocation).ChangeInvocationTarget(Target);

                invocation.Proceed();
            }

        }
    }
}