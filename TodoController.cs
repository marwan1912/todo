using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Web.Services;
using System.Xml.Linq;
using WebApplication2;


namespace WebApplication2.Controllers
{
    [RoutePrefix("api/Todo")]
    public class TodoController : ApiController
    {

         [HttpPost]
         [Route("load")]
         public HttpResponseMessage LoadAllTasks(User user)
         {
             try
             {
                 using (SqlConnection conn = new SqlConnection())
                 {
                     conn.ConnectionString = "Server = tcp:marawan.database.windows.net,1433; Initial Catalog = TodoDB; Persist Security Info = False; User ID = {user}; Password = {password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; ";
                     conn.Open();

                    SqlCommand command = new SqlCommand("SELECT ID, Item, Percentage FROM Todo WHERE UserEmail=@user", conn);
                     command.Parameters.Add(new SqlParameter("user", user.Email));
                     List<TodoItem> list = new List<TodoItem>();
                     using (SqlDataReader reader = command.ExecuteReader())
                     {
                         while (reader.Read())
                         {
                             TodoItem to = new TodoItem();
                             to.ID = Convert.ToInt32(reader["ID"]);
                             to.Item = reader["Item"].ToString();
                             to.Percentage = Convert.ToInt32(reader["Percentage"]);

                             list.Add(to);

                         }
                     }


                     if (list.Count != 0)
                     {
                         return Request.CreateResponse(HttpStatusCode.OK, list);
                     }
                     else
                     {
                         return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no tasks in the list to display.");
                     }

                 }
             }
             catch (Exception ex)
             {
                 return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
             }
         }
        [HttpPost]
        public HttpResponseMessage InsertTask([FromBody] TodoItem todo)
        {
            try
            {
                string connectionString = null;
                string sql = null;
                connectionString = "Server = tcp:marawan.database.windows.net,1433; Initial Catalog = TodoDB; Persist Security Info = False; User ID = {user}; Password = {password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; ";
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();


                    sql = "insert into Todo values(@user,@item,@percentage)";
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {

                        SqlParameter User = cmd.Parameters.AddWithValue("@user", todo.UserEmail);
                        if (todo.UserEmail == null)
                        {
                            User.Value = DBNull.Value;
                        }
                        if (todo.Item == null)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter a task.");
                        }
                        else
                        {
                            SqlParameter Item = cmd.Parameters.AddWithValue("@item", todo.Item);
                        }
                        if (todo.Percentage == null)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter a completion percentage.");

                        }
                        else
                        {
                            SqlParameter Percentage = cmd.Parameters.AddWithValue("@percentage", todo.Percentage);
                        }

                        cmd.ExecuteNonQuery();
                        var message = Request.CreateResponse(HttpStatusCode.Created,todo);
                        message.Headers.Location = new Uri(Request.RequestUri +todo.ID.ToString());
                        return message;

                    }
                }
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        public HttpResponseMessage DeleteTask(int id, TodoItem todo)
        {
            try
            {
                string connectionString = null;
                string sql = null;
                connectionString = "Server = tcp:marawan.database.windows.net,1433; Initial Catalog = TodoDB; Persist Security Info = False; User ID = {user}; Password = {password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; ";
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    SqlCommand command = new SqlCommand("SELECT ID , Item, Percentage FROM Todo WHERE UserEmail=@user AND ID=@id", cnn);
                    command.Parameters.Add(new SqlParameter("user", todo.UserEmail));
                    command.Parameters.Add(new SqlParameter("id", id));

                    List<TodoItem> list = new List<TodoItem>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TodoItem to = new TodoItem();
                            to.ID = Convert.ToInt32(reader["ID"]);
                            to.Item = reader["Item"].ToString();
                            to.Percentage = Convert.ToInt32(reader["Percentage"]);

                            list.Add(to);

                        }
                    }
                    if (list.Count == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no task in your list with this ID.");

                    }


                    sql = "DELETE FROM Todo WHERE ID=@id AND UserEmail=@user";
                        using (SqlCommand cmd = new SqlCommand(sql, cnn))
                        {
                            if (todo.ID == null)
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter the ID of the task you want to delete.");

                            }
                            cmd.Parameters.Add(new SqlParameter("user", todo.UserEmail));
                            cmd.Parameters.Add(new SqlParameter("id", id));
                            cmd.ExecuteNonQuery();
                        }

                        return Request.CreateResponse(HttpStatusCode.OK);
                    
                }
                

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        [Route("{id:int}")]
        [HttpPost]
        public HttpResponseMessage UpdateTask(int id, [FromBody]TodoItem todo)
        {
            try
            {
                string connectionString = null;
                string sql = null;
                connectionString = "Server = tcp:marawan.database.windows.net,1433; Initial Catalog = TodoDB; Persist Security Info = False; User ID = {user}; Password = {password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; ";
                using (SqlConnection cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();
                    if (todo.ID == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter the ID of the task you want to update.");

                    }
                    //sql = "UPDATE Todo SET Percentage='" + todo.Percentage + "'WHERE ID='" + id + "'";
                    sql = "UPDATE Todo SET Percentage=@percentage WHERE ID=@id AND UserEmail=@user";
                    if (todo.Percentage == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please enter the completion percentage of the task you want to update.");

                    }
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.Add(new SqlParameter("percentage", todo.Percentage));
                        cmd.Parameters.Add(new SqlParameter("id", id));
                        cmd.Parameters.Add(new SqlParameter("user", todo.UserEmail));

                        cmd.ExecuteNonQuery();
                    }
                    SqlCommand command = new SqlCommand("SELECT ID , Item, Percentage FROM Todo WHERE UserEmail=@user AND ID=@id", cnn);
                    command.Parameters.Add(new SqlParameter("user", todo.UserEmail));
                    command.Parameters.Add(new SqlParameter("id", id));

                    List<TodoItem> list = new List<TodoItem>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TodoItem to = new TodoItem();
                            to.ID = Convert.ToInt32(reader["ID"]);
                            to.Item = reader["Item"].ToString();
                            to.Percentage = Convert.ToInt32(reader["Percentage"]);

                            list.Add(to);

                        }
                    }


                    if (list.Count != 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,todo);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no task in your list with this ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
