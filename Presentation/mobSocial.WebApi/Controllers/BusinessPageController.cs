using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.BusinessPages;
using mobSocial.Data.Entity.EventPages;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Enum;
using mobSocial.Services.BusinessPages;
using mobSocial.Services.EventPages;
using mobSocial.Services.Extensions;
using mobSocial.Services.GeoServices;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.BusinessPages;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("businesspage")]
    public class BusinessPageController : RootApiController
    {
        private readonly IUserService _userService;
        private readonly MediaSettings _mediaSettings;
        private readonly IBusinessPageService _businessPageService;
        private readonly IEventPageAttendanceService _eventPageAttendanceService;
        private readonly IMediaService _mediaService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;

        public BusinessPageController(IUserService userService, MediaSettings mediaSettings, IBusinessPageService businessPageService,
            IEventPageAttendanceService eventPageAttendanceService, IMediaService mediaService, ICountryService countryService, IStateService stateService)
        {
            _userService = userService;
            _mediaSettings = mediaSettings;
            _businessPageService = businessPageService;
            _eventPageAttendanceService = eventPageAttendanceService;
            _mediaService = mediaService;
            _countryService = countryService;
            _stateService = stateService;
        }

        [Route("get/{eventId:int}")]
        [HttpGet]
        public IHttpActionResult Index(int eventId)
        {
            var entityId = eventId;

            var entity = _businessPageService.Get(entityId);
            if (entity == null)
            {
                return NotFound();
            }

            var model = entity.ToModel();


            foreach (var coupon in entity.Coupons)
            {
                model.Coupons.Add(new BusinessPageCouponModel {
                    Id = coupon.Id,
                    Name = coupon.Name,
                    Title = coupon.Title,
                    Disclaimer = coupon.Disclaimer,
                    DisplayOrder = coupon.DisplayOrder
                });
            }
            var media = _mediaService.GetEntityMedia<BusinessPage>(entityId, MediaType.Image);
            foreach (var picture in media)
            {
                model.Pictures.Add(picture.ToModel(_mediaService, _mediaSettings));
            }


            model.MainPictureUrl = _mediaService.GetPictureUrl(model.Pictures.FirstOrDefault()?.Id ?? 0, returnDefaultIfNotFound: true);
            model.FullSizeImageUrl = model.MainPictureUrl;


            // security
            //TODO: Trust community to update.
            model.CanEdit = ApplicationContext.Current.CurrentUser.IsAdministrator();

            return RespondSuccess(new {BusinessPage = model});

        }

        [HttpGet]
        [Route("get/{eventPageId:int}/{status}")]
        public IHttpActionResult Get(int eventPageId, AttendanceStatus status)
        {

            List<EventPageAttendance> attendees = null;
            switch (status)
            {
                case AttendanceStatus.Going:
                    attendees = _eventPageAttendanceService.GetAllGoing(eventPageId);
                    break;
                case AttendanceStatus.NotGoing:
                    attendees = _eventPageAttendanceService.GetAllNotGoing(eventPageId);
                    break;
                case AttendanceStatus.Maybe:
                    attendees = _eventPageAttendanceService.GetAllMaybies(eventPageId);
                    break;
                case AttendanceStatus.Invited:
                    attendees = _eventPageAttendanceService.GetAllInvited(eventPageId);
                    break;

            }
            var userIds = attendees.Select(x => x.CustomerId);

            var users = _userService.Get(x => userIds.Contains(x.Id)).ToArray();

            var models = new List<object>();

            foreach (var user in users)
            {
                models.Add(user.ToModel(_mediaService, _mediaSettings));
            }

            return RespondSuccess(new { Users = models });

        }


        [HttpPost]
        [Route("invitefriends")]
        public IHttpActionResult InviteFriends(int eventPageId, int[] customerIds)
        {

            if (ApplicationContext.Current.CurrentUser.IsVisitor())
                return Unauthorized();


            var invitedCustomers = _eventPageAttendanceService.InviteFriends(eventPageId, customerIds);

            var models = new List<object>();

            foreach (var customer in invitedCustomers)
            {
                models.Add(customer.ToModel(_mediaService, _mediaSettings));
            }

            return Json(models);
        }

        [HttpPost]
        public IHttpActionResult GetUninvitedFriends(int eventPageId, int index)
        {
           /* var customerId = ApplicationContext.Current.CurrentUser.Id;

            var uninvitedFriends = _eventPageAttendanceService.GetUninvitedFriends(eventPageId, customerId,
                index, 20);


            if (uninvitedFriends.Count == 0)
                return RespondSuccess();

            var uninvitedFriendsAsCustomers = _userService.Get(
                uninvitedFriends.Select(x => (x.ToCustomerId == customerId)
                    ? x.FromCustomerId
                    : x.ToCustomerId)
                    .ToArray());

            var models = new List<object>();

            foreach (var customer in uninvitedFriendsAsCustomers)
            {
                models.Add(new {
                    CustomerId = customer.Id,
                    FullName = customer.GetFullName(),
                    PictureUrl = _pictureService.GetPictureUrl(
                        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                        _mobSocialSettings.EventPageAttendanceThumbnailSize, _customerSettings.DefaultAvatarEnabled,
                        defaultPictureType: PictureType.Avatar),
                    ProfileUrl = Url.RouteUrl("CustomerProfileUrl", new { SeName = SeoExtensions.GetSeName(customer, 0) }),

                });
            }

            return Json(models);*/
            return null;

        }


        //todo: clean up unused methods

        [HttpPost]
        public IHttpActionResult GetAttendance(int start, int count, int attendanceStatusId)
        {

            //if (System.Enum.IsDefined(typeof(AttendanceStatus), attendanceStatusId))
            //    return Json(null);


            //var attendances = new List<EventPageAttendance>();
            //var attendanceStatusName = string.Empty;


            //switch (attendanceStatusId)
            //{
            //    case (int)AttendanceStatus.Invited:
            //        attendanceStatusName = AttendanceStatus.Invited.ToString();
            //        attendances = _eventPageAttendanceService.GetInvited(start, count);
            //        break;
            //    case (int)AttendanceStatus.Going:
            //        attendanceStatusName = AttendanceStatus.Going.ToString();
            //        attendances = _eventPageAttendanceService.GetGoing(start, count);
            //        break;
            //    case (int)AttendanceStatus.Maybe:
            //        attendanceStatusName = AttendanceStatus.Maybe.ToString();
            //        attendances = _eventPageAttendanceService.GetMaybies(start, count);
            //        break;
            //    case (int)AttendanceStatus.NotGoing:
            //        attendanceStatusName = AttendanceStatus.NotGoing.ToString();
            //        attendances = _eventPageAttendanceService.GetNotGoing(start, count);
            //        break;
            //}

            //var customerIds = attendances.Select(x => x.CustomerId).ToArray();
            //var customers = _customerService.GetCustomersByIds(customerIds);

            //var models = new List<object>();

            //foreach (var customer in customers)
            //{
            //    models.Add(new {

            //        FullName = customer.GetFullName(),
            //        PictureUrl = _pictureService.GetPictureUrl(
            //            customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
            //            _mobSocialSettings.EventPageAttendanceThumbnailSize, _customerSettings.DefaultAvatarEnabled,
            //            defaultPictureType: PictureType.Avatar),
            //        ProfileUrl = Url.RouteUrl("CustomerProfileUrl", new { SeName = SeoExtensions.GetSeName(customer, 0) }),

            //    });
            //}

            //return Json(new {

            //    AttendanceStatusName = attendanceStatusName,
            //    Customers = models
            //});
            return null;
        }


        [HttpPost]
        [Route("getattendancestatus")]
        public IHttpActionResult GetCustomerAttendanceStatus(int eventPageId)
        {

            var customerId = ApplicationContext.Current.CurrentUser.Id;

            var customerAttendanceStatus =
                _eventPageAttendanceService.GetCustomerAttendanceStatus(customerId, eventPageId);

            var attendanceStatusId = (customerAttendanceStatus == null)
                ? (int)AttendanceStatus.None
                : customerAttendanceStatus.AttendanceStatusId;

            return Json(new {
                CustomerId = customerId,
                EventPageId = eventPageId,
                AttendanceStatusId = attendanceStatusId
            });

        }


        [HttpPost]
        [Route("updateattendancestatus")]
        public IHttpActionResult UpdateAttendanceStatus(int eventPageId, int attendanceStatusId)
        {
            //if (_workContext.CurrentCustomer.IsGuest())
            //    return Json(new { redirect = Url.RouteUrl("Login") }, JsonRequestBehavior.AllowGet);



            //try
            //{
            //    if (!System.Enum.IsDefined(typeof(AttendanceStatus), attendanceStatusId))
            //        throw new ApplicationException("Invalid attendance status.");


            //    var customerId = _workContext.CurrentCustomer.Id;
            //    var customerAttendanceStatus =
            //        _eventPageAttendanceService.GetCustomerAttendanceStatus(eventPageId, customerId);
            //    var previousAttendanceStatusId = attendanceStatusId;

            //    if (customerAttendanceStatus == null) // new attendance
            //    {
            //        customerAttendanceStatus = new EventPageAttendance
            //        {
            //            EventPageId = eventPageId,
            //            CustomerId = customerId,
            //            AttendanceStatusId = attendanceStatusId,
            //            DateCreated = DateTime.Now,
            //            DateUpdated = DateTime.Now
            //        };
            //        _eventPageAttendanceService.Insert(customerAttendanceStatus);
            //    }
            //    else // update existing attendance
            //    {
            //        previousAttendanceStatusId = customerAttendanceStatus.AttendanceStatusId;
            //        customerAttendanceStatus.AttendanceStatusId = attendanceStatusId;
            //        customerAttendanceStatus.DateUpdated = DateTime.Now;
            //        _eventPageAttendanceService.Update(customerAttendanceStatus);
            //    }

            //    return Json(new {
            //        PreviousAttendanceStatusId = previousAttendanceStatusId,
            //        EventPageAttendanceId = customerAttendanceStatus.Id,
            //        EventPageId = eventPageId,
            //        CustomerId = customerId,
            //        AttendanceStatusId = attendanceStatusId,
            //        FullName = _workContext.CurrentCustomer.GetFullName(),
            //        PictureUrl = _pictureService.GetPictureUrl(
            //            _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
            //            _mobSocialSettings.EventPageAttendanceThumbnailSize, _customerSettings.DefaultAvatarEnabled,
            //            defaultPictureType: PictureType.Avatar),
            //        ProfileUrl =
            //            Url.RouteUrl("CustomerProfileUrl", new { SeName = SeoExtensions.GetSeName(_workContext.CurrentCustomer, 0) })
            //    });

            //}
            //catch
            //{
            //    return Json(false);
            //}
            return null;
        }

        [HttpGet]
        [Route("autocomplete/{term}")]
        public IHttpActionResult BusinessPageSearchAutoComplete(string term)
        {
            var items = _businessPageService.Get(x => x.Name.StartsWith(term));

            var models = new List<object>();
            foreach (var item in items)
            {
                models.Add(item.ToModel());
            }

            return RespondSuccess(new {BusinessPages = models});
        }

        [HttpGet]
        public IHttpActionResult Search()
        {



            //var states = _stateProvinceService.GetStateProvincesByCountryId(1).ToList();

            //var model = new BusinessPageSearchModel();

            //model.AvailableStates.Add(new SelectListItem {
            //    Text = _localizationService.GetResource("Admin.Address.SelectState"),
            //    Value = "0"
            //});

            //foreach (var state in states)
            //{
            //    model.AvailableStates.Add(new SelectListItem {
            //        Text = state.Name,
            //        Value = state.Id.ToString(),
            //        Selected = (state.Id == model.StateProvinceId)
            //    });
            //}


            //var countries = _countryService.GetAllCountries();

            //model.AvailableCountries.Add(new SelectListItem {
            //    Text = _localizationService.GetResource("Admin.Address.SelectCountry"),
            //    Value = "0"
            //});

            //foreach (var country in countries)
            //{
            //    model.AvailableCountries.Add(new SelectListItem {
            //        Text = country.Name,
            //        Value = country.Id.ToString(),
            //        Selected = (country.Id == model.StateProvinceId)
            //    });
            //}



            //return View(ControllerUtil.MobSocialViewsFolder + "/BusinessPage/Search.cshtml", model);

            return null;
        }


        [HttpGet]
        [Route("get/countries")]
        public IHttpActionResult GetAllCountries()
        {

            var countries = _countryService.Get(null, x => new {x.Name}).ToList();
            var model = new List<object>();
            countries.ForEach(x => model.Add(new {x.Id, x.Name }));

            return RespondSuccess(new { Countries = model });
        }


        [HttpGet]
        [Route("get/states")]
        public IHttpActionResult GetStateProvinces(int countryId)
        {
            var states = _stateService.Get(x => x.CountryId == countryId).ToList();
            var model = new List<object>();
            states.ForEach(x => model.Add(new {x.Id, x.Name }));
            return RespondSuccess(new {States = model});
        }




        [HttpGet]
        [Route("get/all")]
        public IHttpActionResult Search(string search = "", string city = "", int? stateProvinceId = null, int? countryId = null)
        {
            var businessResults = _businessPageService.Search(search, city, stateProvinceId, countryId);

            var results = new List<object>();
            foreach (var item in businessResults)
            {

                var state = _stateService.Get(x => x.Id == item.StateId).FirstOrDefault();
                var stateAbbreviation = (state != null) ? state.Abbreviation : string.Empty;

                var picture = item.GetPictures().FirstOrDefault();
                var pictureUrl = _mediaService.GetPictureUrl(picture, returnDefaultIfNotFound: true);

                results.Add(new {
                    Title = item.Name,
                    Subtitle = item.Address1 + " " + item.City + ", " + stateAbbreviation,
                    SeName = item.GetPermalink().Slug,
                    ThumbnailUrl = pictureUrl
                });
            }

            return RespondSuccess(new
            {
                BusinessPages = results
            });
        }


    }
}
