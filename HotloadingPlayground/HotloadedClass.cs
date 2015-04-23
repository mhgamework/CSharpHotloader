using MHGameWork.TheWizards.Tests.Features.Core.Hotloading;

namespace MHGameWork.Hotloading.Playground
{
    public class HotloadedClass : ITestHotloadingFacade
    {
        public string GetString()
        {
            return "Magic5!";

        }
    }
}