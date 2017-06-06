using Orchard;
using Orchard.ContentManagement;
using Orchard.Taxonomies.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Lombiq.OrchardContentStressTest.Services
{
    public interface ITestContentService : IDependency
    {
        IContent GetTestBlog();
        IContent GetTestBlogPost();
        FileInfo[] GetTestImages();
        TaxonomyPart GetTestTaxonomy();
    }
}