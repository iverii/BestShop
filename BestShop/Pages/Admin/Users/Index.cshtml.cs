using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BestShop.Pages.Admin.Users
{

    [RequiredAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<UserInfo> listUsers = new List<UserInfo>();

        public int page = 1;
        public int totalPages = 0;
        private readonly int pageSize = 10;
        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
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

                    //find numbers of users
                    string sqlCount = "select count(*) from users";
                    using(SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count/pageSize);
                    }

                    string sql = "select * from users order by id desc";

                    sql += " offset @skip rows fetch next @pageSize rows only";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page-1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo userInfo = new UserInfo();
                                userInfo.id = reader.GetInt32(0);
                                userInfo.fristname = reader.GetString(1);
                                userInfo.lastname = reader.GetString(2);
                                userInfo.email = reader.GetString(3);
                                userInfo.phone = reader.GetString(4);
                                userInfo.address = reader.GetString(5);
                                userInfo.password = reader.GetString(6);
                                userInfo.role = reader.GetString(7);
                                userInfo.createdAt = reader.GetDateTime(8).ToString("MM/dd/yyyy");

                                listUsers.Add(userInfo);
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

    public class UserInfo
    {
        public int id;
        public string fristname;
        public string lastname;
        public string email;
        public string phone;
        public string address;
        public string password;
        public string role;
        public string createdAt;
    }
}
