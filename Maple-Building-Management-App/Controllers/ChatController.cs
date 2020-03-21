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
                if ((bool)Session["Admin"])
                {
                    ViewBag.userTitle = "Administrator";
                }
                else if (Session["TenantID"] != null)
                {
                    ViewBag.userTitle = "Tenant";
                }
                else
                {
                    ViewBag.userTitle = "Property Manager";
                }
            }
            else
            {
                ViewBag.hdnFlag = "";
                ViewBag.userTitle = "Guest";
            }
            return View();
        }
    }
}