using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Admin;
using System.Web.Mvc;

namespace Lombiq.OrchardContentStressTest.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IAuthorizer _authorizer;

        public Localizer T { get; set; }


        public AdminController(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
            T = NullLocalizer.Instance;
        }


        public ActionResult Index()
        {
            if (!_authorizer.Authorize(StandardPermissions.SiteOwner, T("You're not allowed to create test content.")))
                return new HttpUnauthorizedResult();


            return View();
        }
    }
}