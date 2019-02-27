﻿using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Test()
        {
            int c = 0;
            int b = 0;
            int a = 0;
            return null;
        }
        public ActionResult Admin()
        {
            if (SessionManager.IsValidUser)
            {

                if (SessionManager.User.IsAdmin)
                {
                    
                    return View();
                }
                else
                {
                    
                    return RedirectToAction("NormalUser");
                }
            }
            else
            {
                return Redirect("~/User/Login");
            }
        }

      
        public ActionResult Profile(String pic)
        {
            if (WebPrac.Security.SessionManager.User == null)
                return Redirect("~/User/Login");
            PMS.Entities.UserDTO dto = PMS.BAL.UserBO.GetUserByPictuer(pic);
            return View(dto);
        }
        public ActionResult NormalUser()
        {
            if (SessionManager.IsValidUser)
            {

                if (SessionManager.User.IsAdmin)
                {
                    return RedirectToAction("Admin");
                }
                else
                {
                   
                    return View();
                }
            }
            else
            {
                return Redirect("~/User/Login");
            }
        }
    }
}