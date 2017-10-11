using System.Web.Http;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.MediaEntities;
using mobSocial.Data.Extensions;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Models.EntityProperty;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("entityproperties")]
    public class EntityPropertyController : RootApiController
    {
        private readonly IMediaService _mediaService;
        private readonly IUserService _userService;
        public EntityPropertyController(IMediaService mediaService, IUserService userService)
        {
            _mediaService = mediaService;
            _userService = userService;
        }

        [HttpPost]
        [Route("post")]
        [Authorize]
        public IHttpActionResult Post(EntityPropertyModel requestModel)
        {
            var entityName = requestModel.EntityName;
            var propertyName = requestModel.PropertyName;
            var propertyValue = requestModel.Value;
            var entityId = requestModel.EntityId;
            var currentUser = ApplicationContext.Current.CurrentUser;

            Media media = null;
            if (PropertyNames.IsMediaPropertyName(propertyName))
            {
                //the property value must be an integer
                var valueAsInteger = propertyValue.GetInteger(false);
                if (valueAsInteger == 0)
                    return BadRequest();
              
                //get media since this is media property, let's get the media first
                media = _mediaService.Get(valueAsInteger);
                //is the person trying to mess around actually is a capable person
                if (!currentUser.CanEditResource(media))
                {
                    return RespondFailure("Unauthorized", "post_entityproperty");
                }
            }
            //get valid system property name if available
            propertyName = PropertyNames.ParseToValidSystemPropertyName(propertyName) ?? propertyName;

            switch (entityName.ToLower())
            {
                case "user":
                    //somebody is trying to set the user's properties. He must be the user himself or administrator
                    var user = _userService.Get(entityId);
                    if(user == null || !currentUser.CanEditUser(user))
                        return NotFound();

                    user.SetPropertyValue(propertyName, propertyValue);
                    if (media != null)
                    {
                        //also attach the media to user so we can show them all at one place
                        //this way if user wants to delete the media, we'll simply detach them immediately to postpone till a service performs deletion
                        _mediaService.AttachMediaToEntity(user, media);
                    }
                    break;
            }

            return RespondSuccess();
        }
    }
}