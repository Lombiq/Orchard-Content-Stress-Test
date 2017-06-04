
using Bogus;
using Lombiq.OrchardContentStressTest.Constants;
using Lombiq.OrchardContentStressTest.ViewModels;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Layouts.Elements;
using Orchard.Layouts.Models;
using Orchard.Layouts.Services;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Tags.Models;
using System.Net;
using System.Web.Http;

namespace Lombiq.OrchardContentStressTest.Controllers.ApiControllers
{
    public class CreateContentController : ApiController
    {
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly ILayoutSerializer _layoutSerializer;
        private readonly IElementManager _elementManager;

        public Localizer T { get; set; }


        public CreateContentController(IAuthorizer authorizer, IContentManager contentManager, ILayoutSerializer layoutSerializer, IElementManager elementManager)
        {

            _authorizer = authorizer;
            _contentManager = contentManager;
            _layoutSerializer = layoutSerializer;
            _elementManager = elementManager;
        }


        [HttpPost]
        public IHttpActionResult Post([FromBody]CreateContentRequestViewModel viewModel)
        {
            if (!_authorizer.Authorize(StandardPermissions.SiteOwner))
                return Content(HttpStatusCode.Unauthorized, T("You're not allowed to create test content.").Text);

            if (viewModel == null)
            {
                return Content(HttpStatusCode.BadRequest, T("Bad request.").Text);
            }
            var faker = new Faker();
            var remainingCount = viewModel.Count - viewModel.CurrentCount;
            switch (viewModel.Type)
            {
                case "Page":
                    for (int i = 0; i < (remainingCount < Config.BatchCount ? remainingCount : Config.BatchCount); i++)
                    {
                        var page = _contentManager.New("Page");
                        page.As<TitlePart>().Title = faker.Lorem.Sentence();
                        var canvas = _elementManager.ActivateElement<Canvas>();
                        var html = _elementManager.ActivateElement<Html>();
                        canvas.Elements.Add(html);
                        html.Content = faker.Lorem.Paragraphs();
                        page.As<LayoutPart>().LayoutData = _layoutSerializer.Serialize(new[] { canvas });
                        page.As<TagsPart>().CurrentTags = faker.Commerce.Categories(10);
                        _contentManager.Create(page);
                    }

                    break;
                default:
                    return Content(HttpStatusCode.BadRequest, T("Unsupported content type.").Text);
            }

            return Content(HttpStatusCode.OK, T("The batch were successfully created.").Text);
        }
    }
}