﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Lombiq.OrchardContentStressTest.Constants;
using System.IO;

namespace Lombiq.OrchardContentStressTest.Services
{
    public class TestContentService : ITestContentService
    {
        private readonly IContentManager _contentManager;
        private readonly HttpContextBase _httpContextBase;


        public TestContentService(IContentManager contentManager, HttpContextBase httpContextBase)
        {
            _contentManager = contentManager;
            _httpContextBase = httpContextBase;
        }


        public IContent GetTestBlog()
        {
            return _contentManager.Query("Blog").Where<TitlePartRecord>(t => t.Title == Config.TestBlogTitle).List().FirstOrDefault();
        }

        public IContent GetTestBlogPost()
        {
            return _contentManager.Query("BlogPost").Where<TitlePartRecord>(t => t.Title == Config.TestBlogPostTitle).List().FirstOrDefault();
        }

        public FileInfo[] GetTestImages()
        {
            return new DirectoryInfo(_httpContextBase.Server.MapPath(Path.Combine("~/Modules", "Lombiq.OrchardContentStressTest", "Content", "Images"))).GetFiles();
        }
    }
}