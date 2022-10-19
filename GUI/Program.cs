using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WebServer.Models;

namespace GUI
{
    public class Program
    {
        private static Random rnd = new Random();
        public string id { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public void Start()
        {
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(DataServer));
            addClient(host, tcp);
            host.Open();
            Console.WriteLine("System Online");
        }

        public void addClient(ServiceHost host, NetTcpBinding tcp)
        {
            RestClient restClient = new RestClient("http://localhost:54662/");
            RestRequest request = new RestRequest("api/clients/", Method.Get);
            RestResponse response = restClient.Execute(request);
            List<Client> clientList = JsonConvert.DeserializeObject<List<Client>>(response.Content);

            // assign a random port and ip to the client and add service
            int index = clientList.Count + 1;
            int portNum = rnd.Next(8000, 9000);
            string url = "net.tcp://0.0.0.0:" + portNum;
            id = index.ToString();
            ip = "localhost";
            port = portNum.ToString();
            Console.WriteLine(ip + ":" + port + " Added!");
            host.AddServiceEndpoint(typeof(ServerInterface), tcp, url);
            Client client = new Client();
            client.Id = index;
            client.Ip = "localhost";
            client.Port = port.ToString();
            client.Status = "OPEN";

            RestRequest restRequest = new RestRequest("api/clients/", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(client));
            RestResponse restResponse = restClient.Execute(restRequest);
            Client result = JsonConvert.DeserializeObject<Client>(restResponse.Content);
            if (result != null)
            {
                Console.WriteLine("Added client to database");
            }
        }
    }
}
