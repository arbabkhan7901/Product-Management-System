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
    public static class CommentDAO
    {
        public static int Save(CommentDTO dto)
        {
            String sqlQuery = "";
            sqlQuery = String.Format("INSERT INTO Comments(UserId,ProductId,CommentText,CommentOn) VALUES('{0}','{1}','{2}','{3}')",
                    dto.UserID, dto.ProductID, dto.CommentText, dto.CommentOn);
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


        public static CommentDTO GetCommentById(int pid)
        {
            var query = String.Format("Select * from Comments Where CommentId={0}", pid);
            CommentDTO dto = null;
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    using (MySqlDataReader reader = helper.ExecuteReader(query))
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

        public static List<CommentDTO> GetAllComments()
        {
            List<CommentDTO> list = new List<CommentDTO>();
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    var query = @"Select q.CommentId,q.UserId,q.ProductId, q.CommentText, q.CommentOn, u.Name,u.PictureName 
                            from Comments q, Users u 
                            where q.UserId = u.UserID";
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

        public static List<CommentDTO> GetTopComments(int topCount)
        {
            List<CommentDTO> list = new List<CommentDTO>();
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    var query = String.Format(@"SELECT q.CommentId,q.UserId,q.ProductId, q.CommentText, q.CommentOn, u.Name,u.PictureName FROM (
                                    SELECT * ,ROW_NUMBER ( ) OVER ( PARTITION BY ProductID ORDER BY CommentId DESC) r
                                    FROM Comments) q, Users u
                                    WHERE q.r <= {0} and q.UserId = u.UserID", topCount);
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
        private static CommentDTO FillDTO(MySqlDataReader reader)
        {
            var dto = new CommentDTO();
            dto.CommentID = reader.GetInt32(reader.GetOrdinal("CommentId"));
            dto.UserID = reader.GetInt32(reader.GetOrdinal("UserID"));
            dto.ProductID = reader.GetInt32(reader.GetOrdinal("ProductID"));
            dto.CommentText = reader.GetString(reader.GetOrdinal("CommentText"));
            dto.CommentOn = reader.GetDateTime(reader.GetOrdinal("CommentOn"));
            dto.UserName = reader.GetString(reader.GetOrdinal("Name"));
            dto.PictureName = reader.GetString(reader.GetOrdinal("PictureName"));

            return dto;
        }
    }
}
