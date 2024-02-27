using BestShop.MyHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages.Auth
{
    [RequiredNoAuth]
    [BindProperties]
    public class RegisterModel : PageModel
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



        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        public string Password { get; set; } = "";



        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm password do not match")]
        public string ConfirmPassword { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";


        //public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        //{
        //    base.OnPageHandlerExecuting(context);

        //    if(HttpContext.Session.GetString("role") != null)
        //    {
        //        //user alread authonticated => redirect to the home ppage
        //        context.Result = new RedirectResult("/");
        //    }
        //}
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
            if (Phone == null) Phone = "";

            // add the user details to the database
            string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "insert into users " +
                        "(firstname,lastname,email,phone,adress,password,role) values " +
                        "(@firstname,@lastname,@email,@phone,@adress,@password,'client');";
                    //Encrypte password
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", Firstname);
                        command.Parameters.AddWithValue("@lastname", Lastname);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@adress", Adress);
                        command.Parameters.AddWithValue("@password", hashedPassword);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Email))
                {
                    errorMessage = $"Email Adress: {Email} already used";
                }
                else
                {
                    errorMessage = ex.Message;
                }
                return;
            }

            //send confirmation email to the  user
            string username = Firstname + " " + Lastname;
            string subject = "Account Created Succesfully";
            string message = "Dear " + username + "\n\n" +
                "Your account has been created succesfully.\n\n" +
                "Best Regards";
            EmailSender.SendEmail(Email,username,subject,message);


            //initialize the authenticated session => add the  user to the session data
            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from users where email=@email";
                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);

                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string firstname = reader.GetString(1);
                                string lastname = reader.GetString(2);
                                string email = reader.GetString(3);
                                string phone = reader.GetString(4);
                                string adress = reader.GetString(5);
                                // string hashedPassword = reader.GetString(6);
                                string role = reader.GetString(7);
                                string created_at = reader.GetDateTime(8).ToString("MM//dd/yyyy");


                                //add all of them on session data

                                HttpContext.Session.SetInt32("id", id);
                                HttpContext.Session.SetString("firstname", firstname);
                                HttpContext.Session.SetString("lastname", lastname);
                                HttpContext.Session.SetString("email", email);
                                HttpContext.Session.SetString("phone", phone);
                                HttpContext.Session.SetString("adress", adress);
                                HttpContext.Session.SetString("role", role);
                                HttpContext.Session.SetString("created_at", created_at);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return;
            }


            successMessage = "Account created succesfully";

            //go to home page
            Response.Redirect("/");

        }
    }
}
