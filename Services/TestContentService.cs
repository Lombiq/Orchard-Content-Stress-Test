﻿using Lombiq.OrchardContentStressTest.Constants;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using System.IO;
using System.Linq;
using System.Web;

namespace Lombiq.OrchardContentStressTest.Services
{
    public class TestContentService : ITestContentService
    {
        private readonly IContentManager _contentManager;
        private readonly HttpContextBase _httpContextBase;
        private readonly ITaxonomyService _taxonomyService;


        public TestContentService(
            IContentManager contentManager,
            HttpContextBase httpContextBase,
            ITaxonomyService taxonomyService)
        {
            _contentManager = contentManager;
            _httpContextBase = httpContextBase;
            _taxonomyService = taxonomyService;
        }


        public IContent GetTestBlog()
        {
            return _contentManager
                .Query("Blog")
                .Where<TitlePartRecord>(titlePartRecord => titlePartRecord.Title == Config.TestBlogTitle)
                .List()
                .FirstOrDefault();
        }

        public IContent GetTestBlogPost()
        {
            return _contentManager
                .Query("BlogPost")
                .Where<TitlePartRecord>(titlePartRecord => titlePartRecord.Title == Config.TestBlogPostTitle)
                .List()
                .FirstOrDefault();
        }

        public FileInfo[] GetTestImages()
        {
            return
                new DirectoryInfo(
                    _httpContextBase
                        .Server
                        .MapPath(Path.Combine("~/Modules", "Lombiq.OrchardContentStressTest", "Content", "Images")))
                .GetFiles();
        }

        public TaxonomyPart GetTestTaxonomy()
        {
            return _taxonomyService.GetTaxonomyByName(Config.TestTaxonomyTitle);
        }
    }
}