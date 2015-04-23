using Castle.DynamicProxy;
using MHGameWork.Hotloading;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core.Hotloading
{
    [TestFixture]
    public class ChangeableProxyTest
    {
        public ChangeableProxyTest()
        {
        }

        [Test]
        public void TestChangeableProxy()
        {
            var interceptor = new ChangeableProxy<ITestHotloadingFacade>(new ProxyGenerator());
            var proxy = interceptor.Proxy;

            interceptor.Target = new Facade1();

            Assert.AreEqual("One", proxy.GetString());

            interceptor.Target = new Facade2();
            
            Assert.AreEqual("Two", proxy.GetString());


        }
    }
}