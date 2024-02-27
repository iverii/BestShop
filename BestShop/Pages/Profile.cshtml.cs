using BestShop.MyHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages
{

    [RequiredAuth]
    [BindProperties]
    public class ProfileModel : PageModel
    {
        [Required(ErrorMessage = "The First Name is required")]
        public string Firstname { get; set; } = "";



        [Required(ErrorMessage = "The Last Name is required")]
        public string Lastname { get; set; } = "";



        [Required(ErrorMessage = "The Email is required"), EmailAddress]
        public string Email { get; set; } = "";
        public string? Phone { get; set; } = "";



        [Required(ErrorMessage = "The Adress is required")]
        public string Adress { get; set; } = "";



        //[Required(ErrorMessage = "Password is required")]
        //[StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        public string? Password { get; set; } = "";



        //[Required(ErrorMessage = "Confirm password is required")]
        //[Compare("Password", ErrorMessage = "Password and Confirm password do not match")]
        public string? ConfirmPassword { get; set; } = "";



        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            Firstname = HttpContext.Session.GetString("firstname") ?? "";
            Lastname = HttpContext.Session.GetString("lastname") ?? "";
            Email = HttpContext.Session.GetString("email") ?? "";
            Phone = HttpContext.Session.GetString("phone");
            Adress = HttpContext.Session.GetString("adress") ?? "";
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }
            //succesful data validation
            if (Phone == null)
            {
                Phone = "";
            }

            // Update the user profile or the password
            string submitButton = Request.Form["action"];
            string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";


            if (submitButton.Equals("profile"))
            {
                //update the user in database
                try
                {
                    using(SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "update users set firstname=@firstname,lastname=@lastname, " +
                            "email=@email,phone=@phone,adress=@adress where id=@id";

                        int? id = HttpContext.Session.GetInt32("id");

                        using(SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@firstname", Firstname);
                            command.Parameters.AddWithValue("@lastname", Lastname);
                            command.Parameters.AddWithValue("@email", Email);
                            command.Parameters.AddWithValue("@phone", Phone);
                            command.Parameters.AddWithValue("@adress", Adress);
                            command.Parameters.AddWithValue("@id", id);

                            command.ExecuteNonQuery();
                        }

                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }
                //update the session data

                HttpContext.Session.SetString("firstname", Firstname);
                HttpContext.Session.SetString("lastname", Lastname);
                HttpContext.Session.SetString("email", Email);
                HttpContext.Session.SetString("phone", Phone);
                HttpContext.Session.SetString("adress", Adress);

                successMessage = "Profile Updated Correctlye";


            }
            else if (submitButton.Equals("password"))
            {
                // Validate password and confirm password
                if (Password == null || Password.Length <5 || Password.Length > 50)
                {
                    errorMessage = "Password Length should be between 5 and 50 characters";
                    return;
                }
                if(ConfirmPassword == null || !ConfirmPassword.Equals(Password))
                {
                    errorMessage = "Password and confirm password do not match";
                    return;
                }

                // update the password in the database
                try
                {
                    using(SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "update users set password=@password where id=@id";

                        int? id = HttpContext.Session.GetInt32("id");

                        var passwordHasher = new PasswordHasher<IdentityUser>();
                        string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@password", hashedPassword);
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {

                    errorMessage = ex.Message;
                    return;
                }


                successMessage = "Password Updated Correctlye";

            }


        }
    }
}
