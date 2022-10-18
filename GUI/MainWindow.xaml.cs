using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebServer.Models;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string fileName = "";
        private ServerInterface foob;
        private JobData job;
        private string Id;
        private string cIp;
        private string cPort;
        public MainWindow()
        {
            InitializeComponent();
            Program ServerThread = new Program();

            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(DataServer));
            ServerThread.addClient(host, tcp);
            host.Open();
            Console.WriteLine("System Online");
            
            //ServerThread.Start();
            Id = ServerThread.id;
            cIp = ServerThread.ip;
            cPort = ServerThread.port;
            clientId.Text = clientId.Text + ServerThread.id;
            clientIp.Text = clientIp.Text + ServerThread.ip;
            clientPort.Text = clientPort.Text + ServerThread.port;
            NetworkThread();
            
        }

        private void postJob_Click(object sender, RoutedEventArgs e)
        {
            RestClient restClient = new RestClient("http://localhost:54662/");
            RestRequest request = new RestRequest("api/jobs/", Method.Get);
            RestResponse response = restClient.Execute(request);
            List<Job> jobList = JsonConvert.DeserializeObject<List<Job>>(response.Content);

            int index = jobList.Count + 1;

            // check if textblock isn't empty or doesn't contain placeholder text
            if (!codeBlock.Text.ToString().Equals("") && !codeBlock.Text.ToString().Equals("Write Python Code"))
            {
                string code = codeBlock.Text.ToString();
                JobData jb = new JobData();
                jb.code = code;
                jb.status = "OPEN";
                jb.JobId = index;
                AssignJob(jb);
            }
            else
            {
                if (!fileName.Equals(""))
                {
                    string code = File.ReadAllText(fileName);
                    JobData jb = new JobData();
                    jb.code = code;
                    jb.status = "OPEN";
                    jb.JobId = index;
                    AssignJob(jb);
                }
            }
        }

        private async void AssignJob(JobData jb)
        {
            job = jb;
            Task<Job> task = new Task<Job>(SetJob);
            task.Start();
            Job data = await task;
            if (data != null)
            {
                MessageBox.Show("Job Posted!");
            }
            else
            {
                MessageBox.Show("Failed to post job!");
            }
        }

        public Job SetJob()
        {
            ChannelFactory<ServerInterface> foobFactory1;
            ServerInterface foob1;
            NetTcpBinding tcp = new NetTcpBinding();

            Console.WriteLine("Connecting to " + cIp + ":" + cPort + " to assign job");

            string URL = "net.tcp://" + cIp + ":" + cPort;
            foobFactory1 = new ChannelFactory<ServerInterface>(tcp, URL);
            foob1 = foobFactory1.CreateChannel();

            Console.WriteLine("Connected");

            foob1.SetJob(job);

            Console.WriteLine("Job Set");

            Job jobdata = new Job();
            jobdata.Id = job.JobId;
            jobdata.Status = job.status;

            RestClient restClient = new RestClient("http://localhost:54662/");
            RestRequest restRequest = new RestRequest("api/jobs/", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(jobdata));
            RestResponse restResponse = restClient.Execute(restRequest);
            Job result = JsonConvert.DeserializeObject<Job>(restResponse.Content);
            return result;
        }

        private void attachFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op;
            op = new OpenFileDialog();
            op.Title = "Select Python Code";
            op.Filter = "Python Files (*.py)|*.py|Text Files (*.txt)|*.txt";
            if (op.ShowDialog() == true)
            {
                codeBlock.Text = "Write Python Code";
                fileName = op.FileName;
                currentFile.Visibility = Visibility.Visible;
                currentFile.Text = fileName;
            }
            if (!fileName.Equals(""))
            {
                codeBlock.IsReadOnly = true;
            }
        }

        private void viewJobs_Click(object sender, RoutedEventArgs e)
        {
            string currentJobs="";

            RestClient restClient = new RestClient("http://localhost:54662/");
            RestRequest request = new RestRequest("api/clients/", Method.Get);
            RestResponse response = restClient.Execute(request);
            List<Client> clienList = JsonConvert.DeserializeObject<List<Client>>(response.Content);

            
            RestRequest request2 = new RestRequest("api/jobs/", Method.Get);
            RestResponse response2 = restClient.Execute(request2);
            List<Job> list = JsonConvert.DeserializeObject<List<Job>>(response2.Content);

            foreach (Job item in list)
            {
                foreach (Client c in clienList)
                {
                    if (c.Id.Equals(Id))
                    {
                        if (item.Id.Equals(c.JobId)) {
                            currentJobs = currentJobs + "\n" + item.Id + " : " + item.Status;
                        }
                    }
                }
            }
            MessageBox.Show(currentJobs);
        }

        public async void NetworkThread()
        {
            Task<JobData> task = new Task<JobData>(CheckClients);
            task.Start();
            JobData result = await task;
            foob.UploadJob(result);
        }

        public JobData CheckClients()
        {
            do
            {
                RestClient restClient = new RestClient("http://localhost:54662/");
                RestRequest request = new RestRequest("api/clients/", Method.Get);
                RestResponse response = restClient.Execute(request);
                List<Client> clienList = JsonConvert.DeserializeObject<List<Client>>(response.Content);
                if (clienList.Count > 0)
                {
                    foreach (Client item in clienList)
                    {
                        if (item.Id.ToString() != Id)
                        {
                            ChannelFactory<ServerInterface> foobFactory;

                            NetTcpBinding tcp = new NetTcpBinding();

                            Console.WriteLine(Id + " trying to connect to " + item.Id);

                            string URL = "net.tcp://" + item.Ip + ":" + item.Port;
                            foobFactory = new ChannelFactory<ServerInterface>(tcp, URL);
                            foob = foobFactory.CreateChannel();

                            List<JobData> jobs = foob.GetJobs();

                            if (jobs.Count > 0 )
                            {
                                Console.WriteLine(Id + " downloading job");
                                JobData jd = foob.DownloadJob();
                                JobData result = DoTask(jd, item);
                                if (result != null)
                                {
                                    result.status = "DONE";
                                    Job resultData = new Job();
                                    resultData.Id = result.JobId;
                                    resultData.Status = result.status;
                                    RestRequest request2 = new RestRequest("api/jobs/?id=" + resultData.Id, Method.Put);
                                    request2.AddBody(JsonConvert.SerializeObject(resultData));
                                    restClient.Execute(request2);
                                    Console.WriteLine("JOB COMPLETE");
                                    return result;
                                }
                            }else
                            {
                                Console.WriteLine("No Jobs");
                            }
                        }
                    }
                }
            } while (true);
        }


        public JobData DoTask(JobData jd, Client c)
        {
            if (jd != null && jd.status.Equals("OPEN"))
            {
                try
                {
                    int var1, var2;
                    var1 = 1;
                    var2 = 1;
                    ScriptEngine engine = Python.CreateEngine();
                    ScriptScope scope = engine.CreateScope();
                    engine.Execute(jd.code, scope);
                    dynamic testFunction = scope.GetVariable("test_func");
                    int result = testFunction(var1, var2);
                    jd.result = result.ToString();
                    jd.status = "DONE";
                    return jd;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! " + e.Message);
                    return null;
                }
            }
            return null;
        }
    }
}
