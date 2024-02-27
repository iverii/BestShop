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
    public class LoginModel : PageModel
    {
        [Required(ErrorMessage = "The Email is required!"), EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";



        //public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        //{
        //    base.OnPageHandlerExecuting(context);
        //    if (HttpContext.Session.GetString("role") != null)
        //    {
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
                errorMessage = "Data Validation failed";
                return;
            }

            // succesfully data validation


            //connect to database and check the user details
            try
            {
                string connectionstring = "data source=laptop-p7ngb1g4\\sqlexpress;initial catalog=bestshop;integrated security=true;";
                using(SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sql = "select * from users where email=@email";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email",Email);

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
                                string hashedPassword = reader.GetString(6);
                                string role = reader.GetString(7);
                                string created_at = reader.GetDateTime(8).ToString("MM//dd/yyyy");

                                // verify the password its encrypted so need to hash
                                var passwordHasher = new PasswordHasher<IdentityUser>();
                                var result = passwordHasher.VerifyHashedPassword(new IdentityUser(),hashedPassword,Password);


                                if(result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
                                {
                                    //add all of them on session data

                                    HttpContext.Session.SetInt32("id", id);
                                    HttpContext.Session.SetString("firstname", firstname);
                                    HttpContext.Session.SetString("lastname", lastname);
                                    HttpContext.Session.SetString("email", email);
                                    HttpContext.Session.SetString("phone", phone);
                                    HttpContext.Session.SetString("adress", adress);
                                    HttpContext.Session.SetString("role", role);
                                    HttpContext.Session.SetString("created_at", created_at);

                                    // user go home page its logined
                                    Response.Redirect("/");
                                }
                                    
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
            //Wrong email or password
            errorMessage = "Wrong Email or Password";
            
        }


    }
}
