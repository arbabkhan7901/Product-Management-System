using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPrac.Security;

namespace WebPrac.Controllers
{
    public class ProductController : Controller
    {
        private ActionResult GetUrlToRedirect()
        {
            if (SessionManager.IsValidUser)
            {
                if (SessionManager.User.IsAdmin == false)
                {
                    TempData["Message"] = "Unauthorized Access";
                    return Redirect("~/Home/NormalUser");
                }
            }
            else
            {
                TempData["Message"] = "Unauthorized Access";
                return Redirect("~/User/Login");
            }

            return null;
        }
        public ActionResult ShowAll()
        {
            if (SessionManager.IsValidUser == false)
            {
                return Redirect("~/User/Login");
            }

            var products = PMS.BAL.ProductBO.GetAllProducts(true);
            String [] names = PMS.BAL.ProductBO.GetUserNames(products);
            for(int i = 0; i < products.Count(); i++)
            {
                products[i].userName = names[i];
            }
            UserDTO dto = (UserDTO)Session["user"];
            TempData["ID"] = dto.UserID;
            return View(products);
        }

        public ActionResult New()
        {
                var dto = new ProductDTO();
                return  View(dto);
        }

        public ActionResult Edit(int id)
        {

            var redVal = GetUrlToRedirect();
            if (redVal == null)
            {
                var prod = PMS.BAL.ProductBO.GetProductById(id);
                redVal= View("New", prod);
            }
            return redVal;
        }
        public ActionResult Edit2(int id)
        {
            var prod = PMS.BAL.ProductBO.GetProductById(id);
            return View("New", prod);
        }
        public ActionResult Delete(int id)
        {
            bool flag = (bool)TempData["edit"];
            if (!flag)
            {
                if (SessionManager.IsValidUser)
                {

                    if (SessionManager.User.IsAdmin == false)
                    {
                        TempData["Message"] = "Unauthorized Access";
                        return Redirect("~/Home/NormalUser");
                    }
                }
                else
                {
                    return Redirect("~/User/Login");
                }

            }
           // string strconfirm = "<script>if(!window.confirm('Do you want to delete the record?')){window.location.href='Default.aspx'}</script>";
            PMS.BAL.ProductBO.DeleteProduct(id);
            TempData["Msg"] = "Record is deleted!";
            return RedirectToAction("ShowAll");
        }

        [HttpPost]
        public ActionResult Save(ProductDTO dto)
        {
            bool flag = (bool)TempData["edit"];
            var uniqueName = "";
            var ext = "";
            if (Request.Files["Image"] != null)
            {
                var file = Request.Files["Image"];
                if (file.FileName != "")
                {
                     ext = System.IO.Path.GetExtension(file.FileName);
                    
                    //Generate a unique name using Guid
                    uniqueName = Guid.NewGuid().ToString() + ext;

                    //Get physical path of our folder where we want to save images
                    var rootPath = Server.MapPath("~/UploadedFiles");

                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);

                    // Save the uploaded file to "UploadedFiles" folder
                    file.SaveAs(fileSavePath);

                    dto.PictureName = uniqueName;
                }
            }



            if (dto.ProductID > 0)
            {
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = 1;
            }
            else
            {
                dto.CreatedOn = DateTime.Now;
                UserDTO obj = (UserDTO)Session["user"];
                dto.CreatedBy = obj.UserID;
            }
            if (ext != ".jpg" && ext != "")
            {
                TempData["Msg"] = "Image extension should only be .jpg!";
                return View("New", dto);
            }
            PMS.BAL.ProductBO.Save(dto);
            if(!flag)
                TempData["Msg"] = "Record is Saved!";
            else
                TempData["Msg"] = "Record is Updated!";

            return RedirectToAction("ShowAll");
        }

        public ActionResult SaveComment(int ProductID)
        {
            CommentDTO dto = new CommentDTO();
            dto.CommentText = Request["txtComment"];
            dto.CommentOn = DateTime.Now;
            dto.UserID = SessionManager.User.UserID;
            dto.ProductID = ProductID;
            PMS.BAL.CommentBO.Save(dto);
            var products = PMS.BAL.ProductBO.GetAllProducts(true);
            String[] names = PMS.BAL.ProductBO.GetUserNames(products);
            for (int i = 0; i < products.Count(); i++)
            {
                products[i].userName = names[i];
            }
            return View("ShowAll", products);
        }
    }
}