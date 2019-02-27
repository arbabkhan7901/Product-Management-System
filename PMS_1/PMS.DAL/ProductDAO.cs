using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMS.Entities;
using System.Data.SqlClient;

namespace PMS.DAL
{
    public static class ProductDAO
    {
        public static int Save(ProductDTO dto)
        {
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    String sqlQuery = "";
                    if (dto.ProductID > 0)
                    {
                        sqlQuery = String.Format("Update dbo.Products Set Name='{0}',Price='{1}',PictureName='{2}',ModifiedOn='{3}',ModifiedBy='{4}' Where ProductID={5}",
                            dto.Name, dto.Price, dto.PictureName, dto.ModifiedOn, dto.ModifiedBy, dto.ProductID);
                        helper.ExecuteQuery(sqlQuery);
                        return dto.ProductID;
                    }
                    else
                    {
                        sqlQuery = String.Format("INSERT INTO dbo.Products(Name, Price, PictureName, CreatedOn, CreatedBy,IsActive) VALUES('{0}','{1}','{2}','{3}','{4}',{5})",
                            dto.Name, dto.Price, dto.PictureName, dto.CreatedOn, dto.CreatedBy, 1);
                        var obj = helper.ExecuteScalar(sqlQuery);
                        return Convert.ToInt32(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }
        public static ProductDTO GetProductById(int pid)
        {
            var query = String.Format("Select * from dbo.Products Where ProductId={0}", pid);
            ProductDTO dto = null;
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);
                    if (reader.Read())
                    {
                        dto = FillDTO(reader);
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

        public static List<ProductDTO> GetAllProducts(Boolean pLoadComments = false)
        {
            var query = "Select * from dbo.Products Where IsActive = 1;";
            List<ProductDTO> list = new List<ProductDTO>();
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);
                    while (reader.Read())
                    {
                        var dto = FillDTO(reader);
                        if (dto != null)
                        {
                            list.Add(dto);
                        }
                    }
                    if (pLoadComments == true)
                    {
                        // var commentsList = CommentDAO.GetAllComments();

                        var commentsList = CommentDAO.GetTopComments(2);

                        foreach (var prod in list)
                        {
                            List<CommentDTO> prodComments = commentsList.Where(c => c.ProductID == prod.ProductID).ToList();
                            prod.Comments = prodComments;
                        }

                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }

        public static String[] GetUserNames(List<ProductDTO> pro)
        {
            string[] arr = new string[100];
            SqlDataReader reader;
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    for (int i = 0; i < pro.Count; i++)
                    {
                        String query = String.Format(@"Select Name from dbo.users where userID = {0}", pro[i].CreatedBy);

                        reader = helper.ExecuteReader(query);
                        while (reader.Read())
                        {
                            arr[i] = reader.GetString(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return arr;
        }

        public static int DeleteProduct(int pid)
        {
            String sqlQuery = String.Format("Update dbo.Products Set IsActive=0 Where ProductID={0}", pid);
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    return helper.ExecuteQuery(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }

        private static ProductDTO FillDTO(SqlDataReader reader)
        {
            var dto = new ProductDTO();
            dto.ProductID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.Price = reader.GetDouble(2);
            dto.PictureName = reader.GetString(3);
            dto.CreatedOn = reader.GetDateTime(4);
            dto.CreatedBy = reader.GetInt32(5);
            if (reader.GetValue(6) != DBNull.Value)
                dto.ModifiedOn = reader.GetDateTime(6);
            if (reader.GetValue(7) != DBNull.Value)
                dto.ModifiedBy = reader.GetInt32(7);
            dto.IsActive = reader.GetBoolean(8);
            return dto;
        }
    }
}
