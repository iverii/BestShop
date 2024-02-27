using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BestShop.Pages.Admin.Messages
{
    [RequiredAuth(RequiredRole = "admin")]

    public class IndexModel : PageModel
    {
        public List<MessageInfo> listMessages = new List<MessageInfo>();
        public int page = 1;
        public int totalPages = 0;
        public int pageSize = 10;

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            if(requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }

            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlCount = "select count (*) from messages";
                    using(SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }


                    string sql = "select * from messages order by id desc";
                    sql += " offset @skip rows fetch next @pageSize rows only";

                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MessageInfo messageInfo = new MessageInfo();
                                messageInfo.Id = reader.GetInt32(0);
                                messageInfo.FirstName = reader.GetString(1);
                                messageInfo.LastName = reader.GetString(2);
                                messageInfo.Email = reader.GetString(3);
                                messageInfo.Phone = reader.GetString(4);
                                messageInfo.Subject = reader.GetString(5);
                                messageInfo.Message = reader.GetString(6);
                                messageInfo.CreatedAt = reader.GetDateTime(7).ToString("MM/dd/yyyy");

                                listMessages.Add(messageInfo);

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class MessageInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
