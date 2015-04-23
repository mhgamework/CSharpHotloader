using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Engine;
using System.Linq;

namespace MHGameWork.Hotloading
{
    public class HotloadingKernel
    {
        private DirectoryInfo cacheDirectory;

        public HotloadingKernel(DirectoryInfo cacheDirectory)
        {
            this.cacheDirectory = cacheDirectory;
        }

        /// <summary>
        /// Loads the first type implementing T from 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyFile"></param>
        /// <returns></returns>
        public T HotloadInstanceFromAssembly<T>(FileInfo assemblyFile) where T : class
        {
            var proxy = new ChangeableProxy<T>(new ProxyGenerator());

            var loader = new AssemblyHotloader(
               assemblyFile, //TODO
               cacheDirectory);

            Action updateTarget = () =>
                {
                    Thread.Sleep(1000);
                    var assembly = loader.LoadCopied();
                    proxy.Target = Activator.CreateInstance(findHotloadTarget<T>(assembly)) as T;
                };
            loader.Changed += updateTarget;

            updateTarget();

            return proxy.Proxy;
        }

        public T HotloadInstanceFromProject<T>(FileInfo project, FileInfo projectOutputFile) where T : class
        {
            var watcher = new FileSystemWatcher(project.Directory.FullName);

            //watcher.Filter = "*.cs|*.csproj";
            watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                {
                    //TODO: this seems to compile multiple times on a single file write? Add compilation delay?
                    Console.WriteLine(e.FullPath);
                    Console.WriteLine(e.ChangeType);
                    Task.Factory.StartNew(() =>
                        {
                            Console.WriteLine("Compiling {0}...", project.Name);
                            CompileProject(project);
                        });


                };
            watcher.EnableRaisingEvents = true;

            return HotloadInstanceFromAssembly<T>(projectOutputFile);
        }

        private Type findHotloadTarget<T>(Assembly assembly)
        {
            return assembly.GetTypes().First(t => typeof(T).IsAssignableFrom(t));
        }

        public void CompileProject(FileInfo project)
        {
            lock (this)
            {
                var msbuild = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";

                var pi = new ProcessStartInfo(msbuild,
                                              "/fileLogger /p:BuildProjectReferences=false" + " \"" + project.FullName + "\"");
                pi.UseShellExecute = false;

                pi.CreateNoWindow = true;

                var p = Process.Start(pi);

                p.WaitForExit();

                //Console.WriteLine(p.ExitCode); 
            }

        }



    }
}