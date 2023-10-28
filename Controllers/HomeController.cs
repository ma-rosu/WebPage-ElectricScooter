using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebHaufe.Models;
using MySql.Data.MySqlClient;
using NuGet.Protocol.Plugins;

namespace WebHaufe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string connectionString = "Server=db4free.net; Database=projects; User ID=marosu; Password=3dc4a3e4;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Safe()
        {
            return View();
        }

        public IActionResult Rent()
        {
            return View();
        }

        public IActionResult RecordSaved()
        {
            return View();
        }

        public IActionResult Adm()
        {
            return View();
        }


        public IActionResult AdmCheckLogIn()
        {
            try
            {
                string id = Request.Form["id"];
                string pass = Request.Form["pass"];
                if (SecurityCheck(id, pass))
                {
                    return View("SuccesAdm");
                }
                else
                {
                    return View("ErrorAdm");
                }
            }
            catch
            {
                return View("SuccesAdm");
            }
            

        }

        public IActionResult DeleteRecordRequest()
        {
            string phoneToDelete = Request.Form["telefonStergere"];
            if(DeleteRecordInDB(phoneToDelete))
                return View("DeleteRecordRequestSucces");
            else
                return View("DeleteRecordRequestFailed");
        }

        private bool SecurityCheck(string id, string pass)
        {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT pass FROM admin where id=@id";

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string passwordFromDatabase = reader["pass"].ToString();

                            if (passwordFromDatabase == pass)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            
        }

        public IActionResult RecordRent()
        {
            string rentName = Request.Form["numeName"];
            string rentEmail = Request.Form["emailName"];
            string rentPhone = Request.Form["telefonName"];
            WriteRecordInDB(rentName, rentEmail, rentPhone);
            return View("RecordSaved");
        }

        private void WriteRecordInDB(string rentName, string rentEmail, string rentPhone)
        {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO haufe (nume, email, telefon) VALUES (@v1, @v2, @v3)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@v1", rentName);
                    cmd.Parameters.AddWithValue("@v2", rentEmail);
                    cmd.Parameters.AddWithValue("@v3", rentPhone);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool DeleteRecordInDB(string phoneToDelete)
        {
            bool deleted = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    string deleteQuery = "DELETE FROM haufe WHERE telefon = @phoneToDelete";

                    using (MySqlCommand cmd = new MySqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@phoneToDelete", phoneToDelete);
                        cmd.ExecuteNonQuery();
                    }
                    deleted = true;
                }
                catch
                {
                    return deleted;
                }
                
            }
            return deleted;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}