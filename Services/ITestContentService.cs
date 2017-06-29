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
    /// <summary>
    /// Service for managing test contents.
    /// </summary>
    public interface ITestContentService : IDependency
    {
        /// <summary>
        /// Gets the test blog created in the migration.
        /// </summary>
        IContent GetTestBlog();

        /// <summary>
        /// Gets the test blog post created in the migration.
        /// </summary>
        IContent GetTestBlogPost();

        /// <summary>
        /// Gets the test images from the file system.
        /// </summary>
        FileInfo[] GetTestImages();

        /// <summary>
        /// Gets the test taxonomy created in the migration.
        /// </summary>
        TaxonomyPart GetTestTaxonomy();
    }
}