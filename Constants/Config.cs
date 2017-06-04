using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.OrchardContentStressTest.Constants
{
    public class Config
    {
        public const int BatchCount = 50;
        public static string[] SupportedTypes = new string[] { "Test", "Page" };
    }
}