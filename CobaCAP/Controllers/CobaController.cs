using CobaCAP.Models;
using Dapper;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CobaCAP.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CobaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICapPublisher _capBus;

        public CobaController(ICapPublisher capPublisher, IConfiguration configuration)
        {
            _capBus = capPublisher;
            _configuration = configuration;
        }

        [HttpPost(Name = "transaction")]
        public IActionResult AdonetWithTransaction()
        {
            var transConnString = _configuration.GetConnectionString("CapConnection");

            using (var connection = new SqlConnection(transConnString))
            {
                using (var transaction = connection.BeginTransaction(_capBus, false))
                {
                    //your business code
                    connection.Execute("insert into test(name) values('test')", transaction: transaction);

                    _capBus.Publish("sample.rabbitmq.sqlserver", new Person()
                    {
                        Id = 123,
                        Name = "Bar"
                    }, "sample.rabbitmq.callback");


                    transaction.Commit();
                }
            }

            return Ok();
        }

        [NonAction]
        [CapSubscribe("sample.rabbitmq.sqlserver")]
        public Person Subscriber(Person p)
        {
            //throw new InvalidOperationException("sengaja digagalkan (2)");
            Console.WriteLine($@"{DateTime.Now} Subscriber invoked, Info: {p}");
            return p;
        } 


        [NonAction]
        [CapSubscribe("sample.rabbitmq.callback")]
        public void Callback(Person p)
        {
            Console.WriteLine($@"{DateTime.Now} Callback invoked, Info: {p}");
        }


    }

}
