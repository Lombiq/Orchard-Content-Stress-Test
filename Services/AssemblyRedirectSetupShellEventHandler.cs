using Orchard.Environment;
using System;
using System.Reflection;

namespace Lombiq.OrchardContentStressTest.Services
{
    public class AssemblyRedirectSetupShellEventHandler : IOrchardShellEvents
    {
        public void Activated()
        {
            // Trying to remove first so no duplicate event registration can occur.
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveNewtonsoftAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveNewtonsoftAssembly;
        }

        public void Terminating()
        {
        }


        private Assembly ResolveNewtonsoftAssembly(object sender, ResolveEventArgs args)
        {
            // This is here instead of adding assembly redirects to the Web.config. 
            if (args.Name.Split(',')[0] == "Newtonsoft.Json")
            {
                return Assembly.Load("Newtonsoft.Json");
            }

            return null;
        }
    }
}