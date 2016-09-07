using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Data.Entity.Education;
using mobSocial.Services.Education;
using mobSocial.Services.MediaServices;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Education;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("schools")]
    public class SchoolController : RootApiController
    {
        #region fields
        private readonly ISchoolService _schoolService;
        private readonly IMediaService _mediaService;
        #endregion

        #region ctor
        public SchoolController(ISchoolService schoolService, IMediaService mediaService)
        {
            _schoolService = schoolService;
            _mediaService = mediaService;
        }
        #endregion

        #region actions
        [Route("get/{id:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            var school = await _schoolService.GetAsync(id);
            if (school == null)
                return NotFound();

            return RespondSuccess(new {
                School = school.ToModel(_mediaService)
            });
        }
        [HttpGet]
        [Route("get")]
        public async Task<IHttpActionResult> Get([FromUri] SchoolSearchModel searchModel)
        {
            if (searchModel.Count <= 0 || searchModel.Count > 30)
                searchModel.Count = 30;
            if (searchModel.Page <= 0)
                searchModel.Page = 1;

            //todo: show results nearest to user's location
            var schools = await _schoolService.Get(x => x.Name.StartsWith(searchModel.SearchText), null, true, searchModel.Page, searchModel.Count).ToListAsync();
            var model = schools.Select(x => x.ToModel(_mediaService));
            return RespondSuccess(new
            {
                Schools = model
            });
        }

        [Route("post")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult Post(SchoolEntityModel entityModel)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var school = new School() {
                City = entityModel.City,
                Name = entityModel.Name,
                LogoId = entityModel.LogoId,
                UserId = ApplicationContext.Current.CurrentUser.Id
            };
            _schoolService.Insert(school);

            return RespondSuccess();
        } 
        #endregion
    }
}