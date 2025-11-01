using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Lombiq.OrchardContentStressTest
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName => "admin";

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Content"), "1.4", menu => menu
                .Add(
                    T("Content Stress Test"),
                    "1",
                    item => item.Action("Index", "Admin", new { area = "Lombiq.OrchardContentStressTest" })
                .LocalNav()
                .Permission(StandardPermissions.SiteOwner)));
        }
    }
}
