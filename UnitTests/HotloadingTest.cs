using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Castle.DynamicProxy;
using MHGameWork.Hotloading;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core.Hotloading
{
    /// <summary>
    /// Manual test
    /// </summary>
    [TestFixture]
    public class HotloadingTest
    {
        private FileInfo projectFile;
        private HotloadingKernel kernel;
        private FileInfo projectOutputFile;

        public HotloadingTest()
        {
            //TODO: this should be DI.
            projectFile = new FileInfo("../../../HotloadingPlayground/HotloadingPlayGround.csproj");


            // End DI?
            var cacheDirectory = new DirectoryInfo("Cache");
            cacheDirectory.Create();
            kernel = new HotloadingKernel(cacheDirectory);
            projectOutputFile = new FileInfo(@"../../../HotloadingPlayground\bin\Debug\HotloadingPlayground.dll");
        }

        [Test]
        public void TestHotloadDll()
        {
            var facade = kernel.HotloadInstanceFromAssembly<ITestHotloadingFacade>(
                projectOutputFile);

            //TODO: facade should return a dynamic proxy that hotloads behind the scenes.

            while (true)
            {
                Console.WriteLine(facade.GetString());
                Thread.Sleep(1000);
            }

        }

        [Test]
        public void TestHotloadProject()
        {
            var facade = kernel.HotloadInstanceFromProject<ITestHotloadingFacade>(projectFile,projectOutputFile);

            //TODO: facade should return a dynamic proxy that hotloads behind the scenes.

            while (true)
            {
                Console.WriteLine(facade.GetString());
                Thread.Sleep(1000);
            }

        }

        [Test]
        public void TestCompile()
        {
            kernel.CompileProject(projectFile);
        }
    }

    public class Facade1 : ITestHotloadingFacade
    {
        public string GetString()
        {
            return "One";
        }
    }
    public class Facade2 : ITestHotloadingFacade
    {
        public string GetString()
        {
            return "Two";
        }
    }
}