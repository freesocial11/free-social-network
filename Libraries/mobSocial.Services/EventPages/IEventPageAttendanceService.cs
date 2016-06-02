using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.EventPages;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.EventPages
{
    public interface IEventPageAttendanceService : IBaseEntityService<EventPageAttendance>
    {
       
       List<EventPageAttendance> GetGoing(int start, int count);
       List<EventPageAttendance> GetNotGoing(int start, int count);
       List<EventPageAttendance> GetInvited(int start, int count);
       List<EventPageAttendance> GetMaybies(int start, int count);
       
       List<EventPageAttendance> GetAllAttendances(int eventPageId);
       List<EventPageAttendance> GetAllInvited(int eventPageId);
       List<EventPageAttendance> GetAllGoing(int eventPageId);
       List<EventPageAttendance> GetAllMaybies(int eventPageId);
       List<EventPageAttendance> GetAllNotGoing(int eventPageId);
       

       int GetInvitedCount();

       
       List<CustomerFriend> GetUninvitedFriends(int eventPageId, int customerId, int index, int count);
       List<User> InviteFriends(int eventPageId, int[] invitedCustomerIds);

       EventPageAttendance GetCustomerAttendanceStatus(int customerId, int eventPageId);

       


       
    }
}
