using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json.Linq;
using Sentry;
using Microsoft.AspNetCore.Http;
using AspNetCoreMVC.Controllers;
using Sentry.AspNetCore;

namespace AspNetCoreMVC.Controllers
{

    [Route("/")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) => _logger = logger;

        private void Checkout(List<Item> cart)
        {
            _logger.LogInformation("*********** BEFORE {inventory}", Store.inventory);
            var tempInventory = Store.inventory;
            foreach (var item in cart)
            {
                if (Store.inventory[item.id.ToString()] <= 0)
                {
                    throw new Exception("Not enough inventory for " + item.id);
                }
                else
                {
                    tempInventory[item.id.ToString()] = tempInventory[item.id.ToString()] - 1;
                }
            }
            Store.inventory = tempInventory;
            _logger.LogInformation("*********** AFTER {inventory}", Store.inventory);
        }

        [HttpPost("checkout")]
        public string Checkout([FromBody] Order order)
        {
            var email = order.email.ToString();
            var transactionId = Request.Headers["X-transaction-ID"];
            var sessionId = Request.Headers["X-session-ID"];
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new Sentry.Protocol.User
                {
                    Email = email
                };
                scope.SetTag("transaction_id", transactionId);
                scope.SetTag("session_id", sessionId);
                scope.SetExtra("inventory", Store.inventory);
            });

            Checkout(order.cart);
            return "SUCCESS: order has been placed";
        }

        [HttpGet("handled")]
        public string Handled()
        {
            try
            {
                throw null;
            }
            catch (Exception exception)
            {
                exception.Data.Add("detail",
                    new
                    {
                        Reason = "There's a 'throw null' hard-coded in the try block"
                    });

                _logger.LogError(exception, "handled error");
            }
            return "SUCCESS: back-end error handled gracefully";
        }


        [HttpGet("unhandled")]
        public string Unhandled()
        {
            int n1 = 1;
            int n2 = 0;
            int ans = n1 / n2;
            return "FAILURE: Server-side Error";
        }
    }

    public class Order
    {
        public string email { get; set; }
        public List<Item> cart { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string image { get; set; }
    }

    public static class Store
    {
        public static Dictionary<string, int> inventory
            = new Dictionary<string, int>
        {
        { "wrench", 1 },
        { "nails", 1 },
        { "hammer", 1 }
        };
    }

}
