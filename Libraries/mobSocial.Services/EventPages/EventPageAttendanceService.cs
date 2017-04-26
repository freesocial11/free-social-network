using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.EventPages;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Emails;
using mobSocial.Services.Users;

namespace mobSocial.Services.EventPages
{
    public class EventPageAttendanceService : BaseEntityService<EventPageAttendance>,IEventPageAttendanceService
    {

        private readonly IDataRepository<Friend> _customerFriendRepository;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        public EventPageAttendanceService(IDataRepository<EventPageAttendance> entityRepository, IDataRepository<Friend> customerFriendRepository, IUserService userService, IEmailSender emailSender) : base(entityRepository)
        {
            _customerFriendRepository = customerFriendRepository;
            _userService = userService;
            _emailSender = emailSender;
        }


        public List<EventPageAttendance> GetAllInvited(int eventPageId)
        {
            return Repository.Get(x => x.EventPageId == eventPageId && x.AttendanceStatusId == (int)AttendanceStatus.Invited).ToList();
        }
        public List<EventPageAttendance> GetInvited(int start, int count)
        {
            return Repository.Get(x => x.AttendanceStatusId == (int)AttendanceStatus.Invited).Skip(start).Take(count).ToList();
        }


        public List<EventPageAttendance> GetAllGoing(int eventPageId)
        {
            return Repository.Get(x => x.EventPageId == eventPageId && x.AttendanceStatusId == (int)AttendanceStatus.Going).ToList();
        }
        public List<EventPageAttendance> GetGoing(int start, int count)
        {
            return Repository.Get(x => x.AttendanceStatusId == (int)AttendanceStatus.Going).Skip(start).Take(count).ToList();
        }


        public List<EventPageAttendance> GetAllMaybies(int eventPageId)
        {
            return Repository.Get(x => x.EventPageId == eventPageId && x.AttendanceStatusId == (int)AttendanceStatus.Maybe).ToList();
        }
        public List<EventPageAttendance> GetMaybies(int start, int count)
        {
            return Repository.Get(x => x.AttendanceStatusId == (int)AttendanceStatus.Maybe).Skip(start).Take(count).ToList();
        }


        public List<EventPageAttendance> GetAllNotGoing(int eventPageId)
        {
            return Repository.Get(x => x.EventPageId == eventPageId && x.AttendanceStatusId == (int)AttendanceStatus.NotGoing).ToList();
        }

        public List<EventPageAttendance> GetNotGoing(int start, int count)
        {
            return Repository.Get(x => x.AttendanceStatusId == (int)AttendanceStatus.NotGoing).Skip(start).Take(count).ToList();
        }

        public List<Friend> GetUninvitedFriends(int eventPageId, int customerId, int index, int count)
        {

            var attendance = Repository.Get(x=>x.EventPageId == eventPageId).Select(x=>x.CustomerId).ToList();
            var uninvitedFriends = _customerFriendRepository.Get(x => x.Confirmed)
                .Where(x => x.ToCustomerId == customerId || x.FromCustomerId == customerId);

            // only univited friends
            if(attendance.Count > 0)
                uninvitedFriends = uninvitedFriends.Where(x => !attendance.Contains((x.FromCustomerId != customerId) ? x.FromCustomerId : x.ToCustomerId));

            var uninvitedFriendsList = uninvitedFriends
                    .OrderBy(x=>x.DateConfirmed)
                    .Skip(index).Take(count)
                    .ToList();

            return uninvitedFriendsList;


        }


        public List<User> InviteFriends(int eventPageId, int[] invitedCustomerIds)
        {

            var invitedCustomers = new List<User>();

            foreach (var customerId in invitedCustomerIds)
            {
                var eventPageAttendance = new EventPageAttendance()
                {
                    CustomerId = customerId,
                    EventPageId = eventPageId,
                    AttendanceStatusId = (int)AttendanceStatus.Invited,
                };

                Repository.Insert(eventPageAttendance);

                var customer = _userService.Get(customerId);
                invitedCustomers.Add(customer);

                _emailSender.SendEventInvitationNotification(customer);
            }

            return invitedCustomers;

        }



       

       

        public EventPageAttendance GetCustomerAttendanceStatus(int customerId, int eventPageId)
        {
            return Repository.Get(x => x.CustomerId == customerId && x.EventPageId == eventPageId).FirstOrDefault();
        }

        public int GetInvitedCount()
        {
            return Repository.Count(x => x.AttendanceStatusId == (int)AttendanceStatus.Invited);
        }

        public List<EventPageAttendance> GetAllAttendances(int eventPageId)
        {
            return base.Repository.Get(x => x.EventPageId == eventPageId).ToList();
        }
        
    }

}
