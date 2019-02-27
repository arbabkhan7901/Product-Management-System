using MySql.Data.MySqlClient;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DAL
{
    public static class UserDAO
    {
        public static int Save(UserDTO dto)
        {
            String sqlQuery = "";
            if (dto.UserID > 0)
            {
                sqlQuery = String.Format("Update Users Set Name='{0}', PictureName='{1}', Email = '{2}' Where UserID={3}",
                    dto.Name, dto.PictureName, dto.Email, dto.UserID);
            }
            else
            {
                sqlQuery = String.Format("INSERT INTO Users(Name, Login,Password, PictureName, IsAdmin,IsActive, Email) VALUES('{0}','{1}','{2}','{3}',{4},{5},'{6}')",
                    dto.Name, dto.Login, dto.Password, dto.PictureName, 0, 1, dto.Email);
            }
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    return helper.ExecuteQuery(sqlQuery);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }

        public static bool duplicateUser(UserDTO dto)
        {
            String sql = String.Format(@"Select * from users where login = '" + dto.Login + "' or email = '"+dto.Email+"'");
            DBHelper db = new DBHelper();
            using (MySqlDataReader reader = db.ExecuteReader(sql))
            {
                if (reader.Read())
                    return true;
                else
                    return false;
            } 
            
        }

        public static int UpdatePassword(UserDTO dto)
        {
            String sqlQuery = "";
            sqlQuery = String.Format("Update Users Set Password='{0}' Where UserID={1}", dto.Password, dto.UserID);
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    return helper.ExecuteQuery(sqlQuery);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }

        private static String getLogin(String email)
        {
            String login = "";
            String sql = String.Format(@"Select * from users where email = '" + email + "'");
            try
            {
                using (DBHelper db = new DBHelper())
                {
                    using (MySqlDataReader reader = db.ExecuteReader(sql))
                    {
                        if (reader.Read())
                        {
                            login = (String)reader.GetValue(1);
                            return login;
                        }
                    }    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return login;
        }


        public static String resetPassword(String email, String pwd)
        {
            int count = 0;
            String login = "";
            String query = String.Format(@"Update users set password = '{0}' where email = '" + email + "'", pwd);
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    count = helper.ExecuteQuery(query);
                    if (count > 0)
                    {
                        login = getLogin(email);
                        return login;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return login;
        }

        public static bool validateEmail(String email)
        {
            String query = String.Format(@"Select * from users where email = '" + email + "'");
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    using (MySqlDataReader reader = helper.ExecuteReader(query))
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                    }      
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
        public static UserDTO ValidateUser(String pLogin, String pPassword)
        {
            var query = String.Format("Select * from Users Where Login='{0}' and Password='{1}'", pLogin, pPassword);
            UserDTO dto = null;
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    using (var reader = helper.ExecuteReader(query))
                    {
                        if (reader.Read())
                        {
                            dto = FillDTO(reader);
                        }
                    }
                    return dto;
                }
            }
           catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dto;
        }

        public static UserDTO GetUserById(int pid)
        {

            var query = String.Format("Select * from Users Where UserId={0}", pid);
            UserDTO dto = null;
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    using (var reader = helper.ExecuteReader(query))
                    {
                        if (reader.Read())
                        {
                            dto = FillDTO(reader);
                        }
                    }
                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dto;
        }


        public static UserDTO GetUserByLogin(String login)
        {

            var query = String.Format("Select * from Users Where Name='{0}'", login);
            UserDTO dto = null;
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    using (var reader = helper.ExecuteReader(query))
                    {
                        if (reader.Read())
                        {
                            dto = FillDTO(reader);
                        }
                    }
                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dto;
        }

        public static List<UserDTO> GetAllUsers()
        {
            List<UserDTO> list = new List<UserDTO>();
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    var query = "Select * from Users Where IsActive = 1;";
                    using (var reader = helper.ExecuteReader(query))
                    {
                        while (reader.Read())
                        {
                            var dto = FillDTO(reader);
                            if (dto != null)
                            {
                                list.Add(dto);
                            }
                        }
                    }
                    return list;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }

        public static int DeleteUser(int pid)
        {
            String sqlQuery = String.Format("Update Users Set IsActive=0 Where UserID={0}", pid);

            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        private static UserDTO FillDTO(MySqlDataReader reader)
        {
            var dto = new UserDTO();
            dto.UserID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.Login = reader.GetString(2);
            dto.Password = reader.GetString(3);
            dto.PictureName = reader.GetString(4);
            dto.IsAdmin = reader.GetBoolean(5);
            dto.IsActive = reader.GetBoolean(6);
            dto.Email = reader.GetString(7);
            return dto;
        }

        public static UserDTO GetUserByPictuer(String pic)
        {

            var query = String.Format("Select * from Users Where PictureName='{0}'", pic);
            UserDTO dto = null;
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    using (var reader = helper.ExecuteReader(query))
                    {
                        if (reader.Read())
                        {
                            dto = FillDTO(reader);
                        }
                    }
                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dto;
        }
    }
}
