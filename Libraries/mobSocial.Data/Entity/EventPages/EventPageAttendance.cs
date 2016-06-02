using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.EventPages
{
    public class EventPageAttendance : BaseEntity
    {

        public int EventPageId { get; set; }
        public int CustomerId { get; set; }
        public int AttendanceStatusId { get; set; }


        public virtual EventPage EventPage { get; set; }

    }
}