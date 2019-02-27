using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public class UserBO
    {
        public static int Save(UserDTO dto)
        {
            return PMS.DAL.UserDAO.Save(dto);
        }

        public static bool duplicateUser(UserDTO dto)
        {
            return PMS.DAL.UserDAO.duplicateUser(dto);
        }

        public static bool validateEmail(String email)
        {
            return PMS.DAL.UserDAO.validateEmail(email);
        }

        public static String resetPassword(String email, String pwd)
        {
            return PMS.DAL.UserDAO.resetPassword(email, pwd);
        }

        public static bool Email(String toEmailAddress, String subject, String body)
        {

            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                MailAddress to = new MailAddress(toEmailAddress);
                mail.To.Add(to);
                MailAddress from = new MailAddress("arbabkhan7901@gmail.com", "Admin");
                mail.From = from;
                mail.Subject = subject;
                mail.Body = body;
                var sc = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("arbabkhan7901@gmail.com", "Lovefamily1997"),
                    EnableSsl = true
                };
                sc.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static UserDTO GetUserByPictuer(String pic)
        {
            return PMS.DAL.UserDAO.GetUserByPictuer(pic);
        }

        public static int UpdatePassword(UserDTO dto)
        {
            return PMS.DAL.UserDAO.UpdatePassword(dto);
        }

        public static UserDTO ValidateUser(String pLogin, String pPassword)
        {
            return PMS.DAL.UserDAO.ValidateUser(pLogin, pPassword);
        }
        public static UserDTO GetUserById(int pid)
        {
            return PMS.DAL.UserDAO.GetUserById(pid);
        }

        public static UserDTO GetUserByLogin(String Login)
        {
            return PMS.DAL.UserDAO.GetUserByLogin(Login);
        }
        public static List<UserDTO> GetAllUsers()
        {
            return PMS.DAL.UserDAO.GetAllUsers();
        }

        public static int DeleteUser(int pid)
        {
            return PMS.DAL.UserDAO.DeleteUser(pid);
        }

    }
}
