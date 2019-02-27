using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult changePwd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SavePwd()
        {
            UserDTO dto = new UserDTO();
            UserDTO obj = new UserDTO();
            obj = (UserDTO)Session["user"];
            String pwd = Request["newPwd"];
            String pwd1 = Request["newPwd1"];
            if(pwd != pwd1)
            {
                Response.Write("<script> alert('Incorrect Confirm Password!');</script>");
                return View("changePwd", obj);
            }
            else
            {
                dto.Password = pwd;
                dto.UserID = obj.UserID;
                int count = PMS.BAL.UserBO.UpdatePassword(dto);
                if (count > 0)
                    TempData["update"] = "Password Updated Sucessfully";
                return Redirect("~/Home/NormalUser");
            }
           
        }

        [HttpPost]
        public ActionResult SaveLogin()
        {
            String login = Request["login"];
            String password = Request["password"];
            var obj = PMS.BAL.UserBO.ValidateUser(login, password);
            if (obj != null)
            {
                Session["user"] = obj;
               
                if (obj.IsAdmin == true)
                    return Redirect("~/Home/Admin");
                else
                    return Redirect("~/Home/NormalUser");
            }
            else
            {
                Response.Write("<script> alert('Wrong Username/Password');</script>");
                ViewBag.Login = login;
                return View("Login");
            }
        }
       

        [HttpPost]
        public ActionResult Save(UserDTO dto)
        {
            UserDTO obj = (UserDTO)Session["user"];
            dto.Login = Request["login"];
            dto.Name = Request["Name"];
            dto.UserID = obj.UserID;
            var name = "";
            var ext = "";
            if (Request.Files["image"] != null)
            {
                var file = Request.Files["image"];
                if (file.FileName != "")
                {
                    ext = System.IO.Path.GetExtension(file.FileName);
                    name = Guid.NewGuid().ToString() + ext;
                    var rootPath = Server.MapPath("~/UploadedFiles");
                    var fileSavePath = System.IO.Path.Combine(rootPath, name);
                    file.SaveAs(fileSavePath);
                }
            }
            if (name == "")
                dto.PictureName = (String)Session["image"];
            else
                dto.PictureName = name;
            if (ext != ".jpg" && ext != "")
            {
                TempData["update"] = "Image extension should only be .jpg!";
                return RedirectToAction("NormalUser", "Home", null);
            }
            bool flag = PMS.BAL.UserBO.duplicateUser(dto);
            bool temp = (bool)Session["edit"];
            if (flag && !temp)
            {
                Response.Write("<script> alert('User already Exist');</script>");
                return View("Edit", dto);
            }
            else
            {
                int count = PMS.BAL.UserBO.Save(dto);
                if (count > 0)
                    TempData["update"] = "Data Updated Sucessfully";
                else
                    TempData["update"] = "Data does not Updated Sucessfully";
                return RedirectToAction("NormalUser", "Home", null);
            }
        }

        public ActionResult Email()
        {
            return View("ForgotPassword");
        }

        public ActionResult SaveEmail()
        {
            String email = Request["email"];
            bool flag1 = PMS.BAL.UserBO.validateEmail(email);
            if (flag1)
            {
                TempData["email"] = email;
                String subject = "Reset Password Code";
                Random rnd = new Random();
                int num = rnd.Next(12000, 90000);
                Session["Code"] = num.ToString();
                String body = num.ToString();
                bool flag = PMS.BAL.UserBO.Email(email, subject, body);
                return View("Email");
            }
            else
            {
                Response.Write("<script> alert('Wrong Email');</script>");
                return View("ForgotPassword");
            }

        }

        public ActionResult ResetPassword()
        {
            String pwd = Request["code"];
            String email = (String)TempData["email"];
            String login = PMS.BAL.UserBO.resetPassword(email, pwd);
            TempData["pwd"] = "Password Updated!";
            return Redirect("~/Home/NormalUser");
        }

        public ActionResult UpdatePassword()
        {
            String code = Request["code"];
            String temp = (String)Session["Code"];
            if (code == temp)
                return View();
            else
            {
                Response.Write("<script> alert('Wrong Code');</script>");
                return View("Email");
            }

        }
        public ActionResult Edit()
        {
            UserDTO dto = (UserDTO)Session["user"];
            int id = dto.UserID;
            UserDTO dto1 = PMS.BAL.UserBO.GetUserById(id);
            return View(dto1);
        }
        [HttpGet]
        public ActionResult Logout()
        {
            SessionManager.ClearSession();
            return RedirectToAction("Login");
        }

        
        public ActionResult SignUp()
        {
            UserDTO dto = new UserDTO();
            return View(dto);
        }

        [HttpPost]
        public ActionResult Register()
        {
            UserDTO dto = new UserDTO();
            var name = "";
            var ext = "";
            if (Request.Files["image"] != null)
            {
                var file = Request.Files["image"];
                if (file.FileName != "")
                {
                    ext = System.IO.Path.GetExtension(file.FileName);
                    name = Guid.NewGuid().ToString() + ext;
                    var rootPath = Server.MapPath("~/UploadedFiles");
                    var fileSavePath = System.IO.Path.Combine(rootPath, name);
                    file.SaveAs(fileSavePath);

                }
            }
            dto.PictureName = name;
            dto.Name = Request["Name"];
            dto.Login = Request["login"];
            dto.Password = Request["password"];
            dto.Email = Request["Email"];
            dto.IsAdmin = false;
            if (ext != ".jpg" && ext != "")
            {
                Response.Write("<script> alert('Image extension should only be .jpg!');</script>");
                return View("SignUp", dto);
            }
            bool flag = PMS.BAL.UserBO.duplicateUser(dto);
            if(flag)
                Response.Write("<script> alert('User already Exist');</script>");
            else
            {
                int count = PMS.BAL.UserBO.Save(dto);
                if (count > 0)

                    Response.Write("<script> alert('Data Inserted Sucessfully');</script>");
                else
                    Response.Write("<script> alert('Data does not Inserted Sucessfully');</script>");
                return View("Login");
            }
            return View("SignUp", dto);
        }

        [HttpGet]
        public ActionResult Login2()
        {
            return View("SignUp");
        }

        [HttpPost]
        public JsonResult ValidateUser(String login, String password)
        {
            Object data = null;

            try
            {
                var url = "";
                var flag = false;

                var obj = PMS.BAL.UserBO.ValidateUser(login, password);
                if (obj != null)
                {
                    flag = true;
                    SessionManager.User = obj;

                    if (obj.IsAdmin == true)
                        url = Url.Content("~/Home/Admin");
                    else
                        url = Url.Content("~/Home/NormalUser");
                }

                data = new
                {
                    valid = flag,
                    urlToRedirect = url
                };
            }
            catch (Exception)
            {
                data = new
                {
                    valid = false,
                    urlToRedirect = ""
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}