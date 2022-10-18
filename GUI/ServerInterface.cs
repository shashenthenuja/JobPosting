using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    [ServiceContract]
    public interface ServerInterface
    {
        [OperationContract]
        void SetJob(JobData jd);
        [OperationContract]
        JobData DownloadJob();
        [OperationContract]
        void UploadJob(JobData jd);
        [OperationContract]
        List<JobData> GetJobs();

    }
}
