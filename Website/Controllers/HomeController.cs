using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        public IActionResult GetClients()
        {
            RestClient restClient = new RestClient("http://localhost:54662/");
            RestRequest request = new RestRequest("api/clients/", Method.Get);
            RestResponse restResponse = restClient.Execute(request);
            List<Client> data = JsonConvert.DeserializeObject<List<Client>>(restResponse.Content);
            return Ok(data);
        }

        public IActionResult GetJobs()
        {
            RestClient restClient = new RestClient("http://localhost:54662/");
            RestRequest request = new RestRequest("api/jobs/", Method.Get);
            RestResponse restResponse = restClient.Execute(request);
            List<Job> data = JsonConvert.DeserializeObject<List<Job>>(restResponse.Content);
            return Ok(data);
        }
    }
}
