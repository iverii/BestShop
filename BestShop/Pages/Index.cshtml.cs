using BestShop.Pages.Admin.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BestShop.Pages
{
    public class IndexModel : PageModel
    {
        public List<BookInfo> listNewestBooks = new List<BookInfo>();
        public List<BookInfo> listTopSales = new List<BookInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select top 4 * from books order by created_at desc";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
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

                                listNewestBooks.Add(bookInfo);
                            }
                        }
                    }
                    sql = "select top 4 books.*, (" +
                        "select sum(order_items.quantity) from order_items where books.id = order_items.book_id" +
                        ") as total_sales " +
                        "from books " +
                        "order by total_sales desc";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader())
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
                                listTopSales.Add(bookInfo);
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
}
