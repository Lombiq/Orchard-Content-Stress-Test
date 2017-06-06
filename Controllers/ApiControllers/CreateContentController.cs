using Bogus;
using Lombiq.OrchardContentStressTest.Constants;
using Lombiq.OrchardContentStressTest.Models;
using Lombiq.OrchardContentStressTest.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentPicker.Fields;
using Orchard.Core.Common.Fields;
using Orchard.Core.Title.Models;
using Orchard.Fields.Fields;
using Orchard.Layouts.Elements;
using Orchard.Layouts.Models;
using Orchard.Layouts.Services;
using Orchard.Localization;
using Orchard.MediaLibrary.Fields;
using Orchard.Security;
using Orchard.Tags.Models;
using Piedone.HelpfulLibraries.Contents;
using System.Net;
using System.Web.Http;
using System.Linq;
using System;
using Orchard.Exceptions;
using Orchard.Fields.Settings;
using Orchard.Core.Common.Models;
using Lombiq.OrchardContentStressTest.Services;
using Orchard.Comments.Models;
using System.IO;
using Orchard.MediaLibrary.Services;
using Orchard.Users.Models;
using Orchard.Services;
using Orchard.Taxonomies.Services;

namespace Lombiq.OrchardContentStressTest.Controllers.ApiControllers
{
    public class CreateContentController : ApiController
    {
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly ILayoutSerializer _layoutSerializer;
        private readonly IElementManager _elementManager;
        private readonly Faker _faker;
        private readonly ITestContentService _testContentService;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IMembershipService _membershipService;
        private readonly ITaxonomyService _taxonomyService;

        public Localizer T { get; set; }


        public CreateContentController(
            IAuthorizer authorizer,
            IContentManager contentManager,
            ILayoutSerializer layoutSerializer,
            IElementManager elementManager,
            ITestContentService testContentService,
            IMediaLibraryService mediaLibraryService,
            IMembershipService membershipService,
            ITaxonomyService taxonomyService)
        {
            _authorizer = authorizer;
            _contentManager = contentManager;
            _layoutSerializer = layoutSerializer;
            _elementManager = elementManager;
            _faker = new Faker();
            _testContentService = testContentService;
            _mediaLibraryService = mediaLibraryService;
            _membershipService = membershipService;
            _taxonomyService = taxonomyService;
        }


        [HttpPost]
        public IHttpActionResult Post([FromBody]CreateContentRequestViewModel viewModel)
        {
            if (!_authorizer.Authorize(StandardPermissions.SiteOwner))
                return Content(HttpStatusCode.Unauthorized, T("You're not allowed to create test content.").Text);

            var remainingCount = viewModel.Count - viewModel.CurrentCount;
            if (viewModel == null ||
                !Config.SupportedTypes.Contains(viewModel.Type) ||
                viewModel.Count < 1 ||
                viewModel.CurrentCount > viewModel.Count ||
                remainingCount < 0)
            {
                return Content(HttpStatusCode.BadRequest, T("Bad request.").Text);
            }

            for (int i = 0; i < (remainingCount < Config.BatchCount ? remainingCount : Config.BatchCount); i++)
            {
                try
                {
                    var contentItem = _contentManager.New(viewModel.Type);
                    switch (viewModel.Type)
                    {
                        case "Test":
                            SetTitlePart(contentItem);
                            SetLayoutPart(contentItem);
                            SetBooleanField(contentItem, nameof(TestPart), FieldNames.TestBooleanField);
                            SetContentPickerField(contentItem, nameof(TestPart), FieldNames.TestContentPickerField);
                            SetDateTimeField(contentItem, nameof(TestPart), FieldNames.TestDateTimeField);
                            SetEnumerationField(contentItem, nameof(TestPart), FieldNames.TestEnumerationField);
                            SetInputField(contentItem, nameof(TestPart), FieldNames.TestInputField);
                            SetLinkField(contentItem, nameof(TestPart), FieldNames.TestLinkField);
                            SetMediaLibraryPickerField(contentItem, nameof(TestPart), FieldNames.TestMediaLibraryPickerField);
                            SetNumericField(contentItem, nameof(TestPart), FieldNames.TestNumericField);
                            SetTextField(contentItem, nameof(TestPart), FieldNames.TestTextField);

                            break;
                        case "Page":
                            SetTitlePart(contentItem);
                            SetLayoutPart(contentItem);
                            SetTagsPart(contentItem);

                            break;
                        case "BlogPost":
                            SetTitlePart(contentItem);
                            SetBodyPart(contentItem);
                            SetTagsPart(contentItem);
                            SetContainer(contentItem, _testContentService.GetTestBlog());

                            break;
                        case "Comment":
                            var commentPart = contentItem.As<CommentPart>();
                            commentPart.CommentText = _faker.Lorem.Sentence();
                            commentPart.Author = _faker.Internet.UserName();
                            commentPart.Email = _faker.Internet.Email();
                            commentPart.Status = CommentStatus.Approved;
                            commentPart.CommentedOn = _testContentService.GetTestBlogPost().Id;

                            break;
                        case "Image":
                            var testImages = _testContentService.GetTestImages();
                            var file = testImages.ElementAt(_faker.Random.Number(0, testImages.Count() - 1));
                            using (var stream = new FileInfo(file.FullName).OpenRead())
                            {
                                _contentManager.Create(_mediaLibraryService.ImportMedia(stream, Config.TestImagesFolderName, file.Name));
                            }

                            break;
                        case "User":
                            _membershipService.CreateUser(new CreateUserParams(
                                                  _faker.Internet.UserName(),
                                                  "password",
                                                  _faker.Internet.Password(),
                                                  null,
                                                  null,
                                                  true));

                            break;
                        case "TaxonomyTerm":
                            var term = _taxonomyService.NewTerm(_testContentService.GetTestTaxonomy());
                            SetTitlePart(term.ContentItem);
                            _contentManager.Create(term);

                            break;
                        default:
                            return Content(HttpStatusCode.BadRequest, T("Unsupported content type.").Text);
                    }

                    _contentManager.Create(contentItem);
                }
                catch (Exception ex)
                {
                    if (ex.IsFatal()) throw;

                    return Content(HttpStatusCode.InternalServerError, ex.Message);
                }
            }

            return Content(HttpStatusCode.OK, T("The batch were successfully created.").Text);
        }


        private void SetTitlePart(Orchard.ContentManagement.ContentItem contentItem)
        {
            contentItem.As<TitlePart>().Title = _faker.Lorem.Sentence();
        }

        private void SetBodyPart(Orchard.ContentManagement.ContentItem contentItem)
        {
            contentItem.As<BodyPart>().Text = _faker.Lorem.Paragraphs();
        }

        private void SetLayoutPart(Orchard.ContentManagement.ContentItem contentItem)
        {
            var canvas = _elementManager.ActivateElement<Canvas>();
            var html = _elementManager.ActivateElement<Html>();
            canvas.Elements.Add(html);
            html.Content = _faker.Lorem.Paragraphs();
            contentItem.As<LayoutPart>().LayoutData = _layoutSerializer.Serialize(new[] { canvas });
        }

        private void SetTagsPart(Orchard.ContentManagement.ContentItem contentItem)
        {
            contentItem.As<TagsPart>().CurrentTags = _faker.Commerce.Categories(10);
        }

        private void SetContainer(Orchard.ContentManagement.ContentItem contentItem, IContent container)
        {
            contentItem.As<CommonPart>().Container = container;
        }

        private void SetBooleanField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<BooleanField>(partName, fieldName).Value = _faker.Random.Bool();
        }

        private void SetContentPickerField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<ContentPickerField>(partName, fieldName).Ids = new int[] { _faker.Random.Number(2, 100) };
        }

        private void SetDateTimeField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<DateTimeField>(partName, fieldName).DateTime = _faker.Date.Future();
        }

        private void SetEnumerationField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            var field = contentItem.AsField<EnumerationField>(partName, fieldName);
            var settings = field.PartFieldDefinition.Settings.GetModel<EnumerationFieldSettings>();
            field.SelectedValues = new string[] { settings.Options.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[_faker.Random.Number(0, Config.TestEnumerationFieldOptionsNumber - 1)] };
        }

        private void SetInputField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<InputField>(partName, fieldName).Value = _faker.Lorem.Sentence();
        }

        private void SetLinkField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<LinkField>(partName, fieldName).Value = _faker.Internet.DomainName();
        }

        private void SetMediaLibraryPickerField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            var images = _contentManager.Query("Image").Slice(0, 100);
            contentItem.AsField<MediaLibraryPickerField>(partName, fieldName).Ids = new int[] { images.ElementAt(_faker.Random.Number(images.Count() - 1)).Id };
        }

        private void SetNumericField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<NumericField>(partName, fieldName).Value = _faker.Random.Number(0, 100);
        }

        private void SetTextField(Orchard.ContentManagement.ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<TextField>(partName, fieldName).Value = _faker.Lorem.Word();
        }
    }
}