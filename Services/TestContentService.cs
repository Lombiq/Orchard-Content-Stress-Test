using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Lombiq.OrchardContentStressTest.Constants;

namespace Lombiq.OrchardContentStressTest.Services
{
    public class TestContentService : ITestContentService
    {
        private readonly IContentManager _contentManager;


        public TestContentService(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }


        public IContent GetTestBlog()
        {
            return _contentManager.Query("Blog").Where<TitlePartRecord>(t => t.Title == Config.TestBlogTitle).List().FirstOrDefault();
        }

        public IContent GetTestBlogPost()
        {
            return _contentManager.Query("BlogPost").Where<TitlePartRecord>(t => t.Title == Config.TestBlogPostTitle).List().FirstOrDefault();
        }
    }
}