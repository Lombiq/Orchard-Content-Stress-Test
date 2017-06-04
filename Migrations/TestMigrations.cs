using Lombiq.OrchardContentStressTest.Constants;
using Lombiq.OrchardContentStressTest.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.MediaLibrary.Services;
using System.IO;
using System.Web;

namespace Lombiq.OrchardContentStressTest.Migrations
{
    public class TestMigrations : DataMigrationImpl
    {
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IContentManager _contentManager;
        private readonly HttpContextBase _httpContextBase;


        public TestMigrations(IMediaLibraryService mediaLibraryService, IContentManager contentManager, HttpContextBase httpContextBase)
        {
            _mediaLibraryService = mediaLibraryService;
            _contentManager = contentManager;
            _httpContextBase = httpContextBase;
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
                        .OfType("EnumerationField"))
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

            var testImagesDirectoryPath = _httpContextBase.Server.MapPath(Path.Combine("~/Modules", "Lombiq.OrchardContentStressTest", "Content", "Images"));
            foreach (var file in new DirectoryInfo(testImagesDirectoryPath).GetFiles())
            {
                using (FileStream stream = new FileInfo(file.FullName).OpenRead())
                {
                    _contentManager.Create(_mediaLibraryService.ImportMedia(stream, "Test Images", file.Name));
                }
            }

            return 1;
        }
    }
}