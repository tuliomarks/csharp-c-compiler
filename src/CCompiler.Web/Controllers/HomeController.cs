﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCompiler.Common;

namespace AvalExpressoes.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(string source)
        {

            var destination = new Campo();          
            AvaliadorExpressoes.Inicializar(string.Format("{0}\0", source));
            if (AvaliadorExpressoes.Statement(destination))          
                return Json(new {destination = destination.Cod});

            return Json(new { destination = "SINTAXE INCORRETA!!!" });
            
        }

    }
}