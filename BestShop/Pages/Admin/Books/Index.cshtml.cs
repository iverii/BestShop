using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BestShop.Pages.Admin.Books
{
    [RequiredAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<BookInfo> listBooks = new List<BookInfo>();
        public string search = "";

        public int page = 1; //Curent page
        public int totalPages = 0;
        private readonly int pageSize = 10; //books per page

        public string column = "id";
        public string order = "desc";

        public void OnGet()
        {
            search = Request.Query["search"];
            if (search == null) search = "";

            page = 1;
            string requestpage = Request.Query["page"];
            if (requestpage != null)
            {
                try
                {
                    page = int.Parse(requestpage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }

            string[] validColumnds = {"id", "title", "authors", "num_pages", "price", "category", "created_at" };
            column = Request.Query["column"];
            if(validColumnds == null || !validColumnds.Contains(column))
            {
                column = "id";
            }

            order = Request.Query["order"];
            if(order == null || !order.Equals("asc"))
            {
                order = "desc";
            }

            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlCount = "select count(*) from books";
                    if(search.Length > 0)
                    {
                        sqlCount += " where title like @search or authors like @search";
                    }

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");
                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }


                    string sql = "SELECT * FROM books";
                    if(search.Length > 0)
                    {
                        sql += " WHERE title LIKE @search OR authors LIKE @search";
                    }
                    sql += " order by " + column + " " + order; //" order by id desc";
                    sql += " offset @skip rows fetch next @pageSize rows only";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");
                        command.Parameters.AddWithValue("@skip", (page-1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                BookInfo bookInfo = new BookInfo();
                                bookInfo.Id = reader.GetInt32(0);
                                bookInfo.Title = reader.GetString(1);
                                bookInfo.Authors = reader.GetString(2);
                                bookInfo.Isbn = reader.GetString(3);
                                bookInfo.NumPages = reader.GetInt32(4);
                                bookInfo.Price = reader.GetDecimal(5);
                                bookInfo.Category = reader.GetString(6);
                                bookInfo.Description = reader.GetString(7);
                                bookInfo.ImageFileName = reader.GetString(8);
                                bookInfo.CreatedAt = reader.GetDateTime(9).ToString("MM/dd/yyyy");

                                listBooks.Add(bookInfo);
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
    public class BookInfo
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Authors { get; set; } = "";
        public string Isbn { get; set; } = "";
        public int NumPages { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageFileName { get; set; } = "";
        public string CreatedAt { get; set; } = "";

    }

}
