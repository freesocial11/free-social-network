﻿using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Seo;

namespace Nop.Plugin.Widgets.MobSocial
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {

            routes.MapLocalizedRoute("PopulateUrlSlugs",
                            "MobSocial/PopulateUrlSlugs",
                            new { controller = "mobSocial", action = "PopulateUrlSlugs" },
                            new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" });


            routes.MapLocalizedRoute("Plugin.Widgets.mobSocial.Configure",
                 "Plugins/mobSocial/Configure",
                 new { controller = "mobSocial", action = "Configure" },
                 new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" }
            );

            routes.MapLocalizedRoute("Plugin.Widgets.mobSocial.PublicInfo",
                 "Plugins/mobSocial/PublicInfo",
                 new { controller = "mobSocial", action = "PublicInfo" },
                 new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" }
            );


            routes.MapRoute("AdminTeam",
                  "Team",
                  new { controller = "Team", action = "Index" },
                  new[] { "Nop.Plugin.Widgets.MobSocial.Controllers" }
            );

            routes.MapLocalizedRoute("TeamPage",
                  "Team/{teamId}",
                  new { controller = "mobSocial", action = "Team" },
                  new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" }
            );

            routes.MapLocalizedRoute("AddFriend",
                  "MobSocial/AddFriend/{toCustomerId}",
                  new { controller = "mobSocial", action = "AddFriend" },
                  new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" }
            );

            routes.MapLocalizedRoute("FriendRequests",
                  "MobSocial/FriendRequests",
                  new { controller = "mobSocial", action = "FriendRequests" },
                  new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" }
            );

            routes.MapLocalizedRoute("ConfirmFriend",
                "MobSocial/ConfirmFriend/{friendCustomerId}",
                new { controller = "mobSocial", action = "ConfirmFriend" },
                new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" }
            );


            routes.MapLocalizedRoute("PeopleSearchAutoComplete",
                            "MobSocial/SearchTermAutoComplete",
                            new { controller = "mobSocial", action = "SearchTermAutoComplete" },
                            new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" });


            routes.MapLocalizedRoute("EventPageSearchAutoComplete",
                            "EventPage/EventPageSearchAutoComplete",
                            new { controller = "EventPage", action = "EventPageSearchAutoComplete" },
                            new[] { "Nop.Plugin.Widgets.mobSocial.Controllers" });


           
            


        }

        public int Priority
        {
            get
            {
                return 100;
            }
        }
    }
}
