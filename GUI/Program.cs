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
            //This is the actual host service system
            ServiceHost host;
            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(DataServer));
            addClient(host, tcp);
            //And open the host for business!
            host.Open();
            Console.WriteLine("System Online");
            Console.ReadLine();
            //Don't forget to close the host after you're done!
            host.Close();
        }

        private void addClient(ServiceHost host, NetTcpBinding tcp)
        {
            RestClient restClient = new RestClient("http://localhost:54662/");
            RestRequest request = new RestRequest("api/clients/", Method.Get);
            RestResponse response = restClient.Execute(request);
            List<Client> clientList = JsonConvert.DeserializeObject<List<Client>>(response.Content);

            Console.WriteLine(">>>>>>>" + clientList.Count);
            int index = 1;
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
                Console.WriteLine("YAY");
            }
        }
    }
}
