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
using Microsoft.AspNetCore.Http.Internal;
using AspNetCoreMVC.Controllers;
using Sentry.AspNetCore;


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

// public class User
// {
//     public string Email { get; set; }
// }
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

namespace AspNetCoreMVC.Controllers
{

    [Route("home/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // private readonly IGameService _gameService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            // _gameService = gameService;
            _logger = logger;
        }

        private void checkout(List<Item> cart)
        {
            _logger.LogInformation("\n*********** BEFORE " + Store.inventory);
            Dictionary<string, int> tempInventory = Store.inventory;
            foreach (Item item in cart)
            {
                if (Store.inventory[item.id.ToString()] <= 0)
                {
                    throw new Exception("Not enough inventory for " + item.id.ToString());
                }
                else
                {
                    tempInventory[item.id.ToString()] = tempInventory[item.id.ToString()] - 1;
                }
            }
            Store.inventory = tempInventory;
            _logger.LogInformation("\n*********** AFTER " + Store.inventory);
        }

        [HttpPost("checkout")]
        public ActionResult<IEnumerable<string>> checkout([FromBody] Order order)
        {
            checkout(order.cart);
            return new string[] { "SUCCESS: order happened properly" };
        }


        [HttpGet("handled")]
        public ActionResult<IEnumerable<string>> handled()
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

                var id = SentrySdk.CaptureException(exception);
            }
            return new string[] { "SUCCESS: back-end error handled gracefully" };
        }


        [HttpGet("unhandled")]
        public ActionResult<IEnumerable<string>> unhandled()
        {
            int n1 = 1;
            int n2 = 0;
            int ans = n1 / n2;
            return new string[] { "FAILURE: Server-side Error" };
        }

    }
}
