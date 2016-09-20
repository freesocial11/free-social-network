using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.ScheduledTasks
{
    public class ScheduledTask : BaseEntity
    {
        public string Name { get; set; }

        public int Seconds { get; set; }

        public string SystemName { get; set; }

        public bool Enabled { get; set; }

        public bool IsRunning { get; set; }

        public DateTime? LastStartDateTime { get; set; }

        public DateTime? LastEndDateTime { get; set; }

        public DateTime? LastSuccessDateTime { get; set; }

        public bool StopOnError { get; set; }
    }

    public class ScheduledTaskMap : BaseEntityConfiguration<ScheduledTask>
    {
    }
}
