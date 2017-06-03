using Lombiq.OrchardContentStressTest.Constants;
using Lombiq.OrchardContentStressTest.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.OrchardContentStressTest.Migrations
{
    public class TestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(nameof(TestPart),
                part => part
                    .WithField("Test Boolean Field", cfg => cfg
                        .OfType("BooleanField"))
                    .WithField("Test Content Picker Field", cfg => cfg
                        .OfType("ContentPickerField"))
                    .WithField("Test Date Time Field", cfg => cfg
                        .OfType("DateTimeField"))
                    .WithField("Test Enumeration Field", cfg => cfg
                        .OfType("EnumerationField"))
                    .WithField("Test Input Field", cfg => cfg
                        .OfType("InputField"))
                    .WithField("Test Link Field", cfg => cfg
                        .OfType("LinkField"))
                    .WithField("Test Media Library Picker Field", cfg => cfg
                        .OfType("MediaLibraryPickerField"))
                    .WithField("Test Numeric Field", cfg => cfg
                        .OfType("NumericField"))
                    .WithField("Test Text Field", cfg => cfg
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

            return 1;
        }
    }
}