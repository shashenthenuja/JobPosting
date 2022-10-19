using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class DataServer : ServerInterface
    {
        private static List<JobData> jobs = new List<JobData>();

        // Assign job to job board
        public void SetJob(JobData jd)
        {
            string decode = Base64Decode(jd.code);
            byte[] hashData = Hash(decode);
            if (hashData.SequenceEqual(jd.hash))
            {
                jd.code = decode;
                jobs.Add(jd);
                Console.WriteLine("Job Added [" + jobs.Count + "]");
            }
            else
            {
                Console.WriteLine("DATA IS WRONG");
            }
        }

        // send a job from board which is not done
        [MethodImpl(MethodImplOptions.Synchronized)]
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

        // Set result to the job
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UploadJob(JobData jd)
        {
            foreach (JobData item in jobs)
            {
                if (item.JobId.Equals(jd.JobId))
                {
                    item.result = jd.result;
                    item.status = jd.status;
                }
            }
        }

        // Get the count of jobs which are open
        [MethodImpl(MethodImplOptions.Synchronized)]
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

        // decode and hash verification
        public string Base64Decode(string text)
        {
            if (text != "")
            {
                var base64EncodedBytes = Convert.FromBase64String(text);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            return text;
        }

        public byte[] Hash(string data)
        {
            SHA256Managed sha256Hash = new SHA256Managed();
            byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            return hash;
        }
    }
}
