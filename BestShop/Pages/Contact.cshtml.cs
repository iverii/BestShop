using BestShop.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BestShop.Pages
{
    public class ContactModel : PageModel
    {
        public void OnGet()
        {
        }

        public string Firstname { get; set; } = "";
        public string Lastname { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";

        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public void OnPost()
        {
            Firstname = Request.Form["firstname"];
            Lastname = Request.Form["lastname"];
            Email = Request.Form["email"];
            Phone = Request.Form["phone"];
            Subject = Request.Form["subject"];
            Message = Request.Form["message"];

            // Chek if any required field is empty
            if (Firstname.Length == 0 || Lastname.Length == 0 ||
                Email.Length == 0 || Subject.Length == 0 ||
                Message.Length == 0)
            {
                ErrorMessage = "Please fill all required fields";
                return;
            }

            // Add this message to the database
            try
            {
                string connectionString = "Data Source=LAPTOP-P7NGB1G4\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "insert into messages " +
                        "(firstname, lastname, email, phone, subject, message) values " +
                        "(@firstname, @lastname, @email, @phone, @subject, @message);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", Firstname);
                        command.Parameters.AddWithValue("@lastname", Lastname);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@subject", Subject);
                        command.Parameters.AddWithValue("@message", Message);

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }



            // Send Confirmation Email to the Client
            string username = Firstname + " " + Lastname;
            string emailSubject = "About your message: ";
            string emailMessage = "Dear " + username + ",\n" +
                "We received your message. Thank you for contaction us.\n" +
                "Best Regards \n\n" +
                "Your Message: \n\n" + Message;

            EmailSender.SendEmail(Email, username, emailSubject, emailMessage);

            SuccessMessage = "Your message has been recived correctly";

            Firstname = "";
            Lastname = "";
            Email = "";
            Phone = "";
            Subject = "";
            Message = "";


            

        }
    }
}
