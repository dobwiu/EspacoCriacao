using System.Web.Mvc;

namespace EcWebApp.Areas.Orc
{
    public class OrcAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Orc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Orc_default",
                "Orc/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}