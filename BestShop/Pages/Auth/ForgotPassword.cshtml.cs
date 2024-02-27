using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages.Auth
{
    [RequiredNoAuth]
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "The Email is required"), EmailAddress]
        public string Email { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";
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

            // Creat token// save token in database // send token via email to user
            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from users where email=@email";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstname = reader.GetString(1);
                                string lastname = reader.GetString(2);

                                // Create unic token to send user
                                string token = Guid.NewGuid().ToString();

                                //save this token in database
                                SaveToken(Email, token);

                                // send the token by email to the user
                                string resetUrl = Url.PageLink("/Auth/ResetPassword") + "?token=" + token;
                                string username = firstname + " " + lastname;
                                string subject = "Password Reset";
                                string message = "Dear " + username + ",\n\n" +
                                    "You can reset your password using the following link:\n\n" +
                                    resetUrl + "\n\n" +
                                    "Best Regards";

                                EmailSender.SendEmail(Email,username,subject,message);
                            }
                            else
                            {
                                errorMessage = "No user found with this email";
                                return;
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


            successMessage = "Please check your email and click on reset password link";
        }


        private void SaveToken(string email, string token)
        {
            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // delete any old token from this email from database
                    string sql = "Delete from password_resets where email=@email";
                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);


                        command.ExecuteNonQuery();


                    }

                    sql = "insert into password_resets (email,token) values (@email,@token)";
                    using(SqlCommand command = new SqlCommand(sql,connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@token", token);
                        command.ExecuteNonQuery();
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
