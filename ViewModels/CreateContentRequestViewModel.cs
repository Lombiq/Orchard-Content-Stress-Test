using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.OrchardContentStressTest.ViewModels
{
    public class CreateContentRequestViewModel
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public int CurrentCount { get; set; }
    }
}