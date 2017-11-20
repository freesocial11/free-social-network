using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Data.Entity.Education;
using mobSocial.Services.Education;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.OAuth;
using mobSocial.WebApi.Configuration.Security.Attributes;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Education;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("educations")]
    public class EducationController : RootApiController
    {
        #region fields
        private readonly IEducationService _educationService;
        private readonly ISchoolService _schoolService;
        private readonly IMediaService _mediaService;
        #endregion

        #region ctor
        public EducationController(IEducationService educationService, ISchoolService schoolService, IMediaService mediaService)
        {
            _educationService = educationService;
            _schoolService = schoolService;
            _mediaService = mediaService;
        }
        #endregion

        #region actions
        [Route("get")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByUser(int userId = 0)
        {

            var currentUser = ApplicationContext.Current.CurrentUser;
            userId = userId == 0 ? currentUser.Id : userId;

            //first get the education
            var educationEntities = await _educationService.Get(x => x.UserId == userId, earlyLoad: x => x.School).ToListAsync();
            if (currentUser.Id == userId)
            {
                return RespondSuccess(new {
                    Educations = educationEntities.Select(x => x.ToEntityModel())
                });
            }

            return RespondSuccess(new {
                Educations = educationEntities.Select(x => x.ToModel(_mediaService))
            });
        }


        [Route("get/{id:int}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            //first get the education
            var education = _educationService.Get(id, x => x.School);
            return education == null ? NotFound() : GetResponseModel(education);
        }

        [Route("post")]
        [HttpPost]
        [Authorize]
        [ScopeAuthorize(Scope = OAuthScopes.EducationsRW)]
        public IHttpActionResult Post(EducationEntityModel entityModel)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            //first check the school, it should exist
            var school = _schoolService.Get(entityModel.SchoolId);
            if (school == null)
                return RespondFailure("The school must be provided to save education", "post_education");

            var education = new Education() {
                Name = entityModel.Name,
                Description = entityModel.Description,
                EducationType = entityModel.EducationType,
                FromDate = entityModel.FromDate,
                ToDate = entityModel.ToDate,
                SchoolId = entityModel.SchoolId,
                UserId = ApplicationContext.Current.CurrentUser.Id
            };

            _educationService.Insert(education);
            return GetResponseModel(education);
        }

        [Route("put")]
        [HttpPut]
        [Authorize]
        [ScopeAuthorize(Scope = OAuthScopes.EducationsRW)]
        public IHttpActionResult Put(EducationEntityModel entityModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //get the education
            var education = _educationService.Get(entityModel.Id);
            if (education == null || !ApplicationContext.Current.CurrentUser.CanEditResource(education))
                return Unauthorized();

            //first check the school, it should exist
            var school = _schoolService.Get(entityModel.SchoolId);
            if (school == null)
                return RespondFailure("The school must be provided to save education", "put_education");

            education.Name = entityModel.Name;
            education.Description = entityModel.Description;
            education.FromDate = entityModel.FromDate;
            education.ToDate = entityModel.ToDate;
            education.EducationType = entityModel.EducationType;
            education.SchoolId = entityModel.SchoolId;
            
            _educationService.Update(education);
            return GetResponseModel(education);
        }

        [Route("delete/{id:int}")]
        [HttpDelete]
        [Authorize]
        [ScopeAuthorize(Scope = OAuthScopes.EducationsRWD)]
        public IHttpActionResult Delete(int id)
        {
            //get the education
            var education = _educationService.Get(id);
            if (education == null || !ApplicationContext.Current.CurrentUser.CanEditResource(education))
                return Unauthorized();

            _educationService.Delete(education);
            return RespondSuccess();
        } 
        #endregion

        #region helpers
        private IHttpActionResult GetResponseModel(Education education)
        {
            //depending on the logged in user, we send either entity model or response model
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (currentUser.CanEditResource(education))
            {
                return RespondSuccess(new {
                    Education = education.ToEntityModel()
                });

            }
            return RespondSuccess(new {
                Education = education.ToModel(_mediaService)
            });
        }
        #endregion
    }
}