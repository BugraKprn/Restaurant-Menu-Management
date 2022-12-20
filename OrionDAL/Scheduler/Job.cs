using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrionDAL.Scheduler
{
    public delegate void JobHandler(SchedulerEngine engine);

    public enum JobType
    {
        Once,
        PeriodicMS,
        PeriodicMinute,
        PeriodicHour,
        TimeSpecific
    }

    public class Job
    {
        public JobType Type { get; set; }
        public DateTime ExecutionTime { get; set; }
        public double Interval { get; set; }
        public JobHandler Handler { get; set; }

        internal DateTime LastExecution { get; set; }
    }
}
