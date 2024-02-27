using BestShop.MyHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages.Auth
{
    [RequiredNoAuth]
    public class ResetPasswordModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "passowrd is required")]
        [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        public string Password { get; set; } = "";


        [BindProperty, Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm password do not match")]
        public string ConfirmPassword { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            string token = Request.Query["token"];

            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/");
                return;
            }
        }
        public void OnPost()
        {
            string token = Request.Query["token"];

            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/");
                return;
            }

            if(!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            // Connect to database and update password
            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //Check if token is valid, get username email from reset password
                    string email = "";
                    string sql = "select * from password_resets where token=@token";
                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@token", token);
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                email = reader.GetString(0);
                            }
                            else
                            {
                                errorMessage = "Wrong or Expired Tokekn";
                                return;
                            }
                        }
                    }

                    //encrypt new password and update user password
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                    sql = "update users set password=@password where email=@email";
                    using(SqlCommand command = new SqlCommand(sql,connection))
                    {
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }


                    // delete token from database
                    sql = "delete from password_resets where email=@email";
                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return;
            }

            successMessage = "Password reset succesfully";
        }
    }
}
