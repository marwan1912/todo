using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApplication2.Controllers
{
    [RoutePrefix("api/Registeration")]
    public class RegisterationController : ApiController
    {
        [Route("register")]
        [HttpPost]
        public HttpResponseMessage Register([FromBody]User user)
        {
            try
            {
                string connectionString = null;
                string sql = null;
                connectionString = "Server = tcp:marawan.database.windows.net,1433; Initial Catalog = TodoDB; Persist Security Info = False; User ID = {user}; Password = {password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; "; 
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {

                    sql = "insert into users ([Email], [Password]) values(@email,@password)";
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {
                        using (MD5 md5Hash = MD5.Create())
                        {
                            string hash = GetMd5Hash(md5Hash, user.Password);

                            SqlParameter username1 = cmd.Parameters.AddWithValue("@email", user.Email);
                            if (user.Email == null)
                            {
                                username1.Value = DBNull.Value;
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter a valid email address.");
                            }
                            SqlParameter username2 = cmd.Parameters.AddWithValue("@password", hash);
                            if (user.Password == null)
                            {
                               username2.Value = DBNull.Value;
                               return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter a password.");
                            }

                            cnn.Open();
                            cmd.ExecuteNonQuery();
                            var message = Request.CreateResponse(HttpStatusCode.Created, user);
                            message.Headers.Location = new Uri(Request.RequestUri + user.ID.ToString());
                            return message;


                        }
                    }
                }
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This email is already being used by other user.");
            }
        }

        [Route("validate")]
        [HttpPost]
        public HttpResponseMessage validate([FromBody]User user)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server = tcp:marawan.database.windows.net,1433; Initial Catalog = TodoDB; Persist Security Info = False; User ID = {user}; Password = {password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; ";
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM users WHERE Email= @email", conn);

                    command.Parameters.Add(new SqlParameter("email", user.Email));


                    bool match = false;
                    using (MD5 md5Hash = MD5.Create())
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                String compare = Convert.ToString(reader[2]);
                                match = VerifyMd5Hash(md5Hash, user.Password, compare);



                            }
                        }


                    }
                    if (match == true)
                    {
                        string x = Guid.NewGuid().ToString();
                        return Request.CreateResponse(HttpStatusCode.OK, x);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incorrect Email or Password.");
                    }


                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
