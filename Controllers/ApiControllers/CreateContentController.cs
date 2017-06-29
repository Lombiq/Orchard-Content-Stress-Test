using Bogus;
using Lombiq.OrchardContentStressTest.Constants;
using Lombiq.OrchardContentStressTest.Models;
using Lombiq.OrchardContentStressTest.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentPicker.Fields;
using Orchard.Core.Common.Fields;
using Orchard.Core.Title.Models;
using Orchard.Fields.Fields;
using LayoutElements = Orchard.Layouts.Elements;
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
using Orchard.Tags.Services;
using System.Collections.Generic;

namespace Lombiq.OrchardContentStressTest.Controllers.ApiControllers
{
    public class CreateContentController : ApiController
    {
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly ILayoutSerializer _layoutSerializer;
        private readonly IElementManager _elementManager;
        private readonly ITestContentService _testContentService;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IMembershipService _membershipService;
        private readonly ITaxonomyService _taxonomyService;
        private readonly ITagService _tagService;

        private Faker _faker;

        public Localizer T { get; set; }


        public CreateContentController(
            IAuthorizer authorizer,
            IContentManager contentManager,
            ILayoutSerializer layoutSerializer,
            IElementManager elementManager,
            ITestContentService testContentService,
            IMediaLibraryService mediaLibraryService,
            IMembershipService membershipService,
            ITaxonomyService taxonomyService,
            ITagService tagService,
            IFakerService fakerService)
        {
            _authorizer = authorizer;
            _contentManager = contentManager;
            _layoutSerializer = layoutSerializer;
            _elementManager = elementManager;
            _testContentService = testContentService;
            _mediaLibraryService = mediaLibraryService;
            _membershipService = membershipService;
            _taxonomyService = taxonomyService;
            _tagService = tagService;

            _faker = fakerService.CreateFaker();
        }


        [HttpPost]
        public IHttpActionResult Post([FromBody]CreateContentRequestViewModel viewModel)
        {
            if (!_authorizer.Authorize(StandardPermissions.SiteOwner))
                return Content(HttpStatusCode.Unauthorized, T("You're not allowed to create test content.").Text);

            if (viewModel == null)
                return Content(HttpStatusCode.BadRequest, T("Bad request.").Text);

            var remainingCount = viewModel.Count - viewModel.CurrentCount;
            if (!Config.SupportedTypes.Contains(viewModel.Type) ||
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
                    switch (viewModel.Type)
                    {
                        case "Test":
                            var test = _contentManager.New(ContentTypes.Test);

                            SetTitlePart(test);
                            SetLayoutPart(test);

                            _contentManager.Create(test);

                            SetBooleanField(test, nameof(TestPart), FieldNames.TestBooleanField);
                            SetContentPickerField(test, nameof(TestPart), FieldNames.TestContentPickerField);
                            SetDateTimeField(test, nameof(TestPart), FieldNames.TestDateTimeField);
                            SetEnumerationField(test, nameof(TestPart), FieldNames.TestEnumerationField);
                            SetInputField(test, nameof(TestPart), FieldNames.TestInputField);
                            SetLinkField(test, nameof(TestPart), FieldNames.TestLinkField);
                            SetMediaLibraryPickerField(test, nameof(TestPart), FieldNames.TestMediaLibraryPickerField);
                            SetNumericField(test, nameof(TestPart), FieldNames.TestNumericField);
                            SetTextField(test, nameof(TestPart), FieldNames.TestTextField);

                            break;
                        case "Page":
                            var page = _contentManager.New("Page");

                            SetTitlePart(page);
                            SetLayoutPart(page);
                            SetTagsPart(page);

                            _contentManager.Create(page);

                            break;
                        case "Blog Post":
                            var blogPost = _contentManager.New("BlogPost");

                            SetTitlePart(blogPost);
                            SetBodyPart(blogPost);
                            SetTagsPart(blogPost);
                            SetContainer(blogPost, _testContentService.GetTestBlog());

                            _contentManager.Create(blogPost);

                            break;
                        case "Comment":
                            var comment = _contentManager.New("Comment");

                            var commentPart = comment.As<CommentPart>();
                            commentPart.CommentText = _faker.Lorem.Sentence();
                            commentPart.Author = _faker.Internet.UserName();
                            commentPart.Email = _faker.Internet.Email();
                            commentPart.Status = CommentStatus.Approved;
                            commentPart.CommentedOn = _testContentService.GetTestBlogPost().Id;

                            _contentManager.Create(comment);

                            break;
                        case "Image":
                            var testImages = _testContentService.GetTestImages();
                            var file = testImages.ElementAt(_faker.Random.Number(0, testImages.Count() - 1));
                            using (var stream = new FileInfo(file.FullName).OpenRead())
                            {
                                _contentManager.Create(
                                    _mediaLibraryService.ImportMedia(stream, Config.TestImagesFolderName, file.Name));
                            }

                            break;
                        case "User":
                            _membershipService.CreateUser(new CreateUserParams(
                                _faker.Internet.UserName(),
                                "password",
                                _faker.Internet.Email(),
                                null,
                                null,
                                true));

                            break;
                        case "Taxonomy Term":
                            var term = _taxonomyService.NewTerm(_testContentService.GetTestTaxonomy());
                            SetTitlePart(term.ContentItem);

                            _contentManager.Create(term);

                            break;
                        default:
                            return Content(HttpStatusCode.BadRequest, T("Unsupported content type.").Text);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.IsFatal()) throw;

                    return Content(HttpStatusCode.InternalServerError, ex.Message);
                }
            }

            return Content(HttpStatusCode.OK, T("The batch was successfully created.").Text);
        }


        private void SetTitlePart(ContentItem contentItem)
        {
            contentItem.As<TitlePart>().Title = _faker.Lorem.Sentence();
        }

        private void SetBodyPart(ContentItem contentItem)
        {
            contentItem.As<BodyPart>().Text = _faker.Lorem.Paragraphs();
        }

        private void SetLayoutPart(ContentItem contentItem)
        {
            var canvas = _elementManager.ActivateElement<LayoutElements.Canvas>();
            var html = _elementManager.ActivateElement<LayoutElements.Html>();
            canvas.Elements.Add(html);
            html.Content = _faker.Lorem.Paragraphs();
            contentItem.As<LayoutPart>().LayoutData = _layoutSerializer.Serialize(new[] { canvas });
        }

        private void SetTagsPart(ContentItem contentItem)
        {
            contentItem.As<TagsPart>().CurrentTags =
                _faker.Commerce.Categories(10).Select(category => _tagService.CreateTag(category).TagName);
        }

        private void SetContainer(ContentItem contentItem, IContent container)
        {
            contentItem.As<CommonPart>().Container = container;
        }

        private void SetBooleanField(ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<BooleanField>(partName, fieldName).Value = _faker.Random.Bool();
        }

        private void SetContentPickerField(ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<ContentPickerField>(partName, fieldName).Ids =
                new int[] { _faker.Random.Number(2, 100) };
        }

        private void SetDateTimeField(ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<DateTimeField>(partName, fieldName).DateTime = _faker.Date.Future();
        }

        private void SetEnumerationField(ContentItem contentItem, string partName, string fieldName)
        {
            var field = contentItem.AsField<EnumerationField>(partName, fieldName);
            var settings = field.PartFieldDefinition.Settings.GetModel<EnumerationFieldSettings>();
            field.SelectedValues = new string[]
                {
                    settings
                        .Options
                        .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                        [_faker.Random.Number(0, Config.TestEnumerationFieldOptionsNumber - 1)]
                };
        }

        private void SetInputField(ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<InputField>(partName, fieldName).Value = _faker.Lorem.Sentence();
        }

        private void SetLinkField(ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<LinkField>(partName, fieldName).Value = _faker.Internet.DomainName();
        }

        private void SetMediaLibraryPickerField(ContentItem contentItem, string partName, string fieldName)
        {
            var images = _contentManager.Query("Image").Slice(0, 100);
            contentItem.AsField<MediaLibraryPickerField>(partName, fieldName).Ids =
                new int[] { images.ElementAt(_faker.Random.Number(images.Count() - 1)).Id };
        }

        private void SetNumericField(ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<NumericField>(partName, fieldName).Value = _faker.Random.Number(0, 100);
        }

        private void SetTextField(ContentItem contentItem, string partName, string fieldName)
        {
            contentItem.AsField<TextField>(partName, fieldName).Value = _faker.Lorem.Word();
        }
    }
}