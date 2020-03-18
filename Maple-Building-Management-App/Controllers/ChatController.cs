using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Maple_Building_Management_App.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            ViewBag.Message = "Maple Live Chat"; 
            if (Session["ChatName"] != null)
            {
                ViewBag.hdnFlag = Session["ChatName"];
            }
            else
            {
                ViewBag.hdnFlag = "";
            }
            return View();
        }
    }
}