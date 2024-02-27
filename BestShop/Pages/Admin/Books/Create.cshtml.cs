using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages.Admin.Books
{
    [RequiredAuth(RequiredRole ="admin")]
    public class CreateModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The Title is required")]
        [MaxLength(100, ErrorMessage = "The Title cannot exceed 100 characters")]
        public string Title { get; set; } = "";


        [BindProperty]
        [Required(ErrorMessage = "The Authors is required")]
        [MaxLength(255, ErrorMessage = "The Authors cannot exceed 255 characters")]
        public string Authors { get; set; } = "";


        [BindProperty]
        [Required(ErrorMessage = "The ISBN is required")]
        [MaxLength(20, ErrorMessage = "The ISBN cannot exceed 20 characters")]
        public string ISBN { get; set; } = "";


        [BindProperty]
        [Required(ErrorMessage = "The Number of pages is required")]
        [Range(1, 10000, ErrorMessage = "The number of pages must be in the range from 1 to 10000")]
        public int NumPages { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "The Price is required")]
        public decimal Price { get; set; }


        [BindProperty, Required]
        public string Category { get; set; } = "";


        [BindProperty]
        [MaxLength(1000, ErrorMessage = "The description cannot exceed 1000 characters")]
        public string? Description { get; set; } = "";



        [BindProperty]
        [Required(ErrorMessage = "The Image File is required")]
        public IFormFile ImageFile { get; set; }


        public string errorMessage = "";
        public string successMessage = "";


        //save picture in global var
        private IWebHostEnvironment webHostEnvironment;

        public CreateModel(IWebHostEnvironment env)
        {
            webHostEnvironment = env;

        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            //succesfull data validation
            if(Description == null) { Description = ""; };


            //save te image on the server
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(ImageFile.FileName);

            string imageFolder = webHostEnvironment.WebRootPath + "/images/books";

            string imageFullPath = Path.Combine(imageFolder, newFileName);


            using (var stream = System.IO.File.Create(imageFullPath))
            {
                ImageFile.CopyTo(stream);
            }

            // save thennew book in the database
            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "insert into books " +
                        "(title, authors, isbn,num_pages, price, category, description, image_filename) values " +
                        "(@title, @authors, @isbn, @num_pages, @price, @category, @description, @image_filename);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", Title);
                        command.Parameters.AddWithValue("@authors", Authors);
                        command.Parameters.AddWithValue("@isbn", ISBN);
                        command.Parameters.AddWithValue("@num_pages", NumPages);
                        command.Parameters.AddWithValue("@price", Price);
                        command.Parameters.AddWithValue("@category", Category);
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@image_filename", newFileName);

                        command.ExecuteNonQuery();
                    }

                }

            }
            catch (Exception ex)
            {


                errorMessage = ex.Message;
                return;
            }



            successMessage = "Data saved correctly";
            Response.Redirect("/Admin/Books/Index");
        }

    }
}
