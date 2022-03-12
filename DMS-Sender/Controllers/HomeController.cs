using DMS_Sender.Logics.Interface;
using DMS_Sender.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DMS_Sender.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExtractLogic _extractLogic;

        public HomeController(ILogger<HomeController> logger,
            IExtractLogic extractLogic)
        {
            _logger = logger;
            _extractLogic = extractLogic;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file, AuthModel authModel)
        {
            List<ProcessStatus> status = await _extractLogic.UnzipAndProcessFile(file, authModel);
            ViewBag.States = status;
            return View();
        }

       
    }
}
