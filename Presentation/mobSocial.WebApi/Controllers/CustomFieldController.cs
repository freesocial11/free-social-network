using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Data.Entity.CustomFields;
using mobSocial.Services.CustomFields;
using mobSocial.Services.Extensions;
using mobSocial.Services.OAuth;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Helpers;
using mobSocial.WebApi.Models.CustomFields;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("custom-fields")]
    public class CustomFieldController : RootApiController
    {
        private readonly ICustomFieldService _customFieldService;
        private readonly IUserService _userService;
        private readonly IApplicationService _applicationService;

        public CustomFieldController(ICustomFieldService customFieldService, IUserService userService, IApplicationService applicationService)
        {
            _customFieldService = customFieldService;
            _userService = userService;
            _applicationService = applicationService;
        }

        [HttpGet]
        [Route("{entityName}/get/all")]
        public async Task<IHttpActionResult> GetAllFields(string entityName, int applicationId = 0)
        {
            //check if application is owned by the logged in user
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (applicationId != 0)
            {
                var application = _applicationService.Get(applicationId);
                if (application == null || currentUser.Id != application.UserId)
                    return Respond(HttpStatusCode.NotFound);
            }

            //retrieve all extra fields
            var customFields = await _customFieldService.Get(x => x.EntityName == entityName && x.ApplicationId == applicationId, orderBy: x => new { x.DisplayOrder }).ToListAsync();
            var model = customFields.Select(x => x.ToEntityModel());
            return RespondSuccess(new {
                CustomFields = model
            });
        }

        [HttpGet]
        [Route("{entityName}/get/displayable")]
        public async Task<IHttpActionResult> GetDisplayableFields(string entityName, string applicationId = "")
        {
            var currentApp = ApplicationContext.Current.CurrentOAuthApplication;
            var currentAppId = currentApp?.Id ?? 0;
            if (!string.IsNullOrEmpty(applicationId))
            {
                var application = _applicationService.FirstOrDefault(x => x.Guid == applicationId);
                if (currentApp == null || application == null || currentApp.Guid != applicationId)
                    return Respond(HttpStatusCode.NotFound);
            }

            List<CustomFieldModel> model = null;
            //retrieve all extra fields
            var customFields = await _customFieldService
                .Get(x => x.EntityName == entityName &&
                          x.ApplicationId == currentAppId &&
                          x.Visible == true, orderBy: x => x.DisplayOrder)
                .ToListAsync();
            model = customFields.Select(x => x.ToEntityModel()).ToList();

            //send to client now
            return RespondSuccess(new {
                CustomFields = model
            });
        }

        [HttpPost]
        [Route("{entityName}/post")]
        public IHttpActionResult Post(string entityName, CustomFieldModel[] entityModels, int applicationId = 0)
        {
            if (entityModels == null)
                return Respond(HttpStatusCode.BadRequest);

            //check if application is owned by the logged in user
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (applicationId != 0)
            {
                var application = _applicationService.Get(applicationId);
                if (application == null || currentUser.Id != application.UserId)
                    return Respond(HttpStatusCode.NotFound);
            }
            else
            {
                //only admin can add field if it's not app specific
                if (!currentUser.IsAdministrator())
                    return Respond(HttpStatusCode.NotFound);
            }

            //first retrieve all old fields
            var oldCustomFields = _customFieldService.Get(x => x.EntityName == entityName && x.ApplicationId == applicationId).ToList();
            //first allFieldIds
            var allSubmittedFieldIds = entityModels.Select(x => x.Id).Distinct().ToArray();

            //delete the fields which have been removed
            var fieldIdsToDelete = oldCustomFields.Select(x => x.Id).Except(allSubmittedFieldIds);
            _customFieldService.Delete(x => fieldIdsToDelete.Contains(x.Id));

            //save the fields
            foreach (var em in entityModels)
            {
                var customField = em.Id == 0
                    ? new CustomField() {
                        EntityName = entityName,
                        ApplicationId = applicationId
                    }
                    : oldCustomFields.FirstOrDefault(x => x.Id == em.Id && x.ApplicationId == applicationId);
                if (customField == null)
                    continue; //an invalid id was submitted for the field, leave this field
                customField.DisplayOrder = em.DisplayOrder;
                customField.Label = em.Label;
                customField.Required = em.Required;
                customField.Visible = em.Visible;
                customField.FieldGeneratorMarkup = em.FieldGeneratorMarkup;
                customField.FieldType = em.FieldType;
                customField.DefaultValue = em.DefaultValue;
                customField.ParentFieldId = em.ParentFieldId;
                customField.AvailableValues = em.AvailableValues;

                if (customField.Id == 0)
                    _customFieldService.Insert(customField);
                else
                    _customFieldService.Update(customField);


            }

            return RespondSuccess();
        }

        [HttpPost]
        [Route("{entityName}/post/single")]
        public IHttpActionResult PostSingle(string entityName, CustomFieldModel entityModel)
        {
            if (entityModel == null)
                return Respond(HttpStatusCode.BadRequest);
            var applicationId = entityModel.ApplicationId;
            //check if application is owned by the logged in user
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (applicationId != 0)
            {
                var application = _applicationService.Get(applicationId);
                if (application == null || currentUser.Id != application.UserId)
                    return Respond(HttpStatusCode.NotFound);
            }
            else
            {
                //only admin can add field if it's not app specific
                if(!currentUser.IsAdministrator())
                    return Respond(HttpStatusCode.NotFound);
            }

            var customField = entityModel.Id == 0
                ? new CustomField() {
                    EntityName = entityName,
                    ApplicationId = applicationId
                }
                : _customFieldService.FirstOrDefault(x => x.Id == entityModel.Id && x.ApplicationId == applicationId);
            if (customField == null)
                return Respond(HttpStatusCode.NotFound); //an invalid id was submitted for the field, leave this field

            customField.DisplayOrder = entityModel.DisplayOrder;
            customField.Label = entityModel.Label;
            customField.Required = entityModel.Required;
            customField.Visible = entityModel.Visible;
            customField.FieldGeneratorMarkup = entityModel.FieldGeneratorMarkup;
            customField.FieldType = entityModel.FieldType;
            customField.DefaultValue = entityModel.DefaultValue;
            customField.ParentFieldId = entityModel.ParentFieldId;
            customField.AvailableValues = entityModel.AvailableValues;
            customField.Description = entityModel.Description;
            if (customField.Id == 0)
                _customFieldService.Insert(customField);
            else
                _customFieldService.Update(customField);

            return RespondSuccess();
        }

        [HttpDelete]
        [Route("delete/{customFieldId:int}")]
        public IHttpActionResult Delete(int customFieldId)
        {
            //check if the field exists
            var customField = _customFieldService.Get(customFieldId);
            if (customField == null)
                return Respond(HttpStatusCode.NotFound);

            var currentUser = ApplicationContext.Current.CurrentUser;
            //it it appspecific?
            if (customField.ApplicationId == 0)
            {
                if (!currentUser.IsAdministrator())
                    return Respond(HttpStatusCode.Unauthorized);
            }
            else
            {
                //get the application and check if the current user can delete the application or not
                var application = _applicationService.Get(customField.ApplicationId);
                if(application == null || application.UserId != currentUser.Id)
                    return Respond(HttpStatusCode.NotFound);
            }

            //find all the fields which have this field as parent
            var childFields = _customFieldService.Get(x => x.ParentFieldId == customField.Id).ToList();
            //update and set parent id to zero for all
            foreach (var cf in childFields)
            {
                cf.ParentFieldId = null;
                _customFieldService.Update(cf);
            }
            //and delete
            _customFieldService.Delete(customField);
            return RespondSuccess();
        }
    }
}