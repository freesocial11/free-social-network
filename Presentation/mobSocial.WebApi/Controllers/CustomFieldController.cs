using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Data.Entity.CustomFields;
using mobSocial.Services.CustomFields;
using mobSocial.Services.Extensions;
using mobSocial.Services.Users;
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
        public CustomFieldController(ICustomFieldService customFieldService, IUserService userService)
        {
            _customFieldService = customFieldService;
            _userService = userService;
        }

        [HttpGet]
        [Route("{entityName}/get/all")]
        public async Task<IHttpActionResult> GetAllFields(string entityName)
        {
            if (!EntityHelpers.DoesEntitySupportCustomFields(entityName))
                return Respond(HttpStatusCode.NotFound);

            //retrieve all extra fields
            var customFields = await _customFieldService.Get(x => x.EntityName == entityName, orderBy: x => new { x.DisplayOrder }).ToListAsync();
            var model = customFields.Select(x => x.ToEntityModel());
            return RespondSuccess(new {
                CustomFields = model
            });
        }

        [HttpGet]
        [Route("{entityName}/get/displayable")]
        public async Task<IHttpActionResult> GetDisplayableFields(string entityName, int id = 0)
        {
            if (!EntityHelpers.DoesEntitySupportCustomFields(entityName))
                return Respond(HttpStatusCode.NotFound);

            List<CustomFieldModel> model = null;
            if (id > 0)
            {
                switch (entityName)
                {
                   case "user":
                        var user = _userService.Get(id);
                        if (user == null)
                            return Respond(HttpStatusCode.NotFound);
                        model = user.GetCustomFields().Select(x => x.Item1.ToEntityModel(x.Item2)).ToList();
                        break;
                    default:
                        return Respond(HttpStatusCode.NotFound);
                }
            }
            else
            {
                //retrieve all extra fields
                var customFields = await _customFieldService
                    .Get(x => x.EntityName == entityName &&
                             x.Visible == true, orderBy: x => x.DisplayOrder)
                    .ToListAsync();
                model = customFields.Select(x => x.ToEntityModel()).ToList();
            }

            //send to client now
            return RespondSuccess(new {
                CustomFields = model
            });
        }

        [HttpPost]
        [Route("{entityName}/post")]
        public IHttpActionResult Post(string entityName, CustomFieldModel[] entityModels)
        {
            if (!EntityHelpers.DoesEntitySupportCustomFields(entityName))
                return Respond(HttpStatusCode.NotFound);

            if (entityModels == null)
                return Respond(HttpStatusCode.BadRequest);

            //first retrieve all old fields
            var oldCustomFields = _customFieldService.Get(x => x.EntityName == entityName).ToList();
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
                        EntityName = entityName
                    }
                    : oldCustomFields.FirstOrDefault(x => x.Id == em.Id);
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
            if (!EntityHelpers.DoesEntitySupportCustomFields(entityName))
                return Respond(HttpStatusCode.NotFound);

            if (entityModel == null)
                return Respond(HttpStatusCode.BadRequest);


            var customField = entityModel.Id == 0
                ? new CustomField() {
                    EntityName = entityName
                }
                : _customFieldService.FirstOrDefault(x => x.Id == entityModel.Id);
            if (customField == null)
                return Respond(HttpStatusCode.NotFound); //an invalid id was submitted for the field, leave this field

            customField.DisplayOrder = entityModel.DisplayOrder;
            customField.Label = entityModel.Label;
            customField.Visible = entityModel.Visible;

            customField.Required = entityModel.Required;
            customField.Visible = entityModel.Visible;
            customField.FieldGeneratorMarkup = entityModel.FieldGeneratorMarkup;
            customField.FieldType = entityModel.FieldType;
            customField.DefaultValue = entityModel.DefaultValue;
            customField.ParentFieldId = entityModel.ParentFieldId;
            customField.AvailableValues = entityModel.AvailableValues;

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