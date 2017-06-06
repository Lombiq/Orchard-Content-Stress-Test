using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.OrchardContentStressTest.Services
{
    public interface ITestContentService : IDependency
    {
        IContent GetTestBlog();
        IContent GetTestBlogPost();
    }
}