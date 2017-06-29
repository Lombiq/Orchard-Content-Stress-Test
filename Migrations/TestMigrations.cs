using Lombiq.OrchardContentStressTest.Constants;
using Lombiq.OrchardContentStressTest.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Extensions;
using Orchard.Core.Title.Models;
using Orchard.Data.Migration;
using Orchard.MediaLibrary.Services;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;
using Lombiq.OrchardContentStressTest.Services;

namespace Lombiq.OrchardContentStressTest.Migrations
{
    public class TestMigrations : DataMigrationImpl
    {
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IContentManager _contentManager;
        private readonly ITestContentService _testContentService;


        public TestMigrations(
            IMediaLibraryService mediaLibraryService,
            IContentManager contentManager,
            ITestContentService testContentService)
        {
            _mediaLibraryService = mediaLibraryService;
            _contentManager = contentManager;
            _testContentService = testContentService;
        }


        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(nameof(TestPart),
                part => part
                    .WithField(FieldNames.TestBooleanField, field => field
                        .OfType("BooleanField"))
                    .WithField(FieldNames.TestContentPickerField, field => field
                        .OfType("ContentPickerField"))
                    .WithField(FieldNames.TestDateTimeField, field => field
                        .OfType("DateTimeField"))
                    .WithField(FieldNames.TestEnumerationField, field => field
                        .OfType("EnumerationField")
                        .WithSetting(
                            "EnumerationFieldSettings.Options",
                            string.Join(
                                Environment.NewLine,
                                Enumerable
                                    .Range(0, Config.TestEnumerationFieldOptionsNumber)
                                    .Select(number => "Option" + number))))
                    .WithField(FieldNames.TestInputField, field => field
                        .OfType("InputField"))
                    .WithField(FieldNames.TestLinkField, field => field
                        .OfType("LinkField"))
                    .WithField(FieldNames.TestMediaLibraryPickerField, field => field
                        .OfType("MediaLibraryPickerField"))
                    .WithField(FieldNames.TestNumericField, field => field
                        .OfType("NumericField"))
                    .WithField(FieldNames.TestTextField, field => field
                        .OfType("TextField"))
                );

            ContentDefinitionManager.AlterTypeDefinition(ContentTypes.Test,
                cfg => cfg
                    .WithPart(nameof(TestPart))
                    .WithPart("TitlePart")
                    .WithPart("LayoutPart")
                    .WithPart("CommonPart", builder => builder
                        .WithSetting("DateEditorSettings.ShowDateEditor", "True"))
                    .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "True")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "False")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{\"Name\":\"Title\",\"Pattern\":\"{Content.Slug}\",\"Description\":\"my-test\"}]"))
                    .WithPart("LocalizationPart")
                    .WithPart("PublishLaterPart")
                    .Creatable()
                    .Draftable()
                    .Listable()
                );

            // Importing test images.
            foreach (var file in _testContentService.GetTestImages())
            {
                using (var stream = new FileInfo(file.FullName).OpenRead())
                {
                    _contentManager.Create(_mediaLibraryService.ImportMedia(stream, Config.TestImagesFolderName, file.Name));
                }
            }

            // Creating a test Blog for BlogPosts.
            var blog = _contentManager.New("Blog");
            blog.As<TitlePart>().Title = Config.TestBlogTitle;
            _contentManager.Create(blog);

            // Creating a test BlogPost for Comments.
            var blogPost = _contentManager.New("BlogPost");
            blogPost.As<TitlePart>().Title = Config.TestBlogPostTitle;
            blogPost.As<CommonPart>().Container = _testContentService.GetTestBlog();
            _contentManager.Create(blogPost);

            // Creating a test Taxonomy for TaxonomyTerms.
            var taxonomy = _contentManager.New("Taxonomy");
            taxonomy.As<TitlePart>().Title = Config.TestTaxonomyTitle;
            _contentManager.Create(taxonomy);

            return 1;
        }
    }
}