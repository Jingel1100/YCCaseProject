using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebstoreMVC.Models;

namespace WebstoreMVC.Controllers
{
    public class HomeController : Controller
    {
                
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Contact()
        {
            ViewData["Message"] = "Here you can find us.";

            return View();
        }

        public IActionResult UMLDiagrams()
        {

            return View();
        }
        public IActionResult About()
        {

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
