using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.OrchardContentStressTest.Constants
{
    public class Config
    {
        public const int BatchCount = 50;
        public const int TestEnumerationFieldOptionsNumber = 10;
        public static string[] SupportedTypes = new string[] { "Test", "Page", "Blog Post", "Comment", "Image", "User", "Taxonomy Term" };
        public const string TestBlogTitle = "Lombiq.OrchardContentStressTest.TestBlog";
        public const string TestBlogPostTitle = "Lombiq.OrchardContentStressTest.TestBlogPost";
        public const string TestImagesFolderName = "Lombiq.OrchardContentStressTest.TestImages";
        public const string TestTaxonomyTitle = "Lombiq.OrchardContentStressTest.TestTaxonomy";
    }
}