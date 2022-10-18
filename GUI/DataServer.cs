using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    public class DataServer : ServerInterface
    {
        private static List<JobData> jobs = new List<JobData>();
        public void SetJob(JobData jd)
        {
            jobs.Add(jd);
        }

        public JobData DownloadJob()
        {
            foreach (JobData item in jobs)
            {
                if (item.status.Equals("OPEN"))
                {
                    return item;
                }
            }
            return null;
        }

        public void UploadJob(JobData jd)
        {
            foreach (JobData item in jobs)
            {
                if (item.JobId.Equals(jd.JobId))
                {
                    item.result = jd.result;
                }
            }
        }
        public List<JobData> GetJobs()
        {
            List<JobData> result = new List<JobData>();
            foreach (JobData item in jobs)
            {
                if (item.status.Equals("OPEN"))
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
