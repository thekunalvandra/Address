using AddressBook_1182.Areas.LOC_Country.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlClient;

namespace AddressBook_1182.Areas.LOC_Country.Controllers
{
    [Area("LOC_Country")]
    [Route("LOC_Country/[Controller]/[Action]")]
    public class LOC_CountryController : Controller
    {
        private IConfiguration Configuration;

        public LOC_CountryController(IConfiguration _configuration)
        {

            Configuration = _configuration;

        }

        public IActionResult Index()
        {
            string connectionstr = Configuration.GetConnectionString("myConnection");

            SqlDatabase sqlDB = new SqlDatabase(connectionstr);
            DbCommand dbCMD = sqlDB.GetStoredProcCommand("PR_Country_SelectAll");

            DataTable dt = new DataTable();
            using (IDataReader dr = sqlDB.ExecuteReader(dbCMD))
            {
                dt.Load(dr);
            }

            return View(dt);
        }
        public IActionResult Search(SearchModel model)
        {
            string connectionstr = this.Configuration.GetConnectionString("myConnection");
            DataTable dt = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(connectionstr);
            sqlConnection.Open();
            SqlCommand ObjCmd = sqlConnection.CreateCommand();
            ObjCmd.CommandType = CommandType.StoredProcedure;
            ObjCmd.CommandText = "PR_Country_Search";
            ObjCmd.Parameters.AddWithValue("CountryName", model.CountryName);
            SqlDataReader sqlDataReader = ObjCmd.ExecuteReader();
            dt.Load(sqlDataReader);
            return View("Index", dt);
        }
        public IActionResult AddEdit(int? CountryID)
        {


            if (CountryID != null)
            {
                SqlConnection Conn = new
               SqlConnection(Configuration.GetConnectionString("myConnection"));
                Conn.Open();
                SqlCommand Cmd = Conn.CreateCommand();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = "PR_Country_SelectByPK";
                Cmd.Parameters.AddWithValue("@CountryID", CountryID);

                SqlDataReader sdr = Cmd.ExecuteReader();
                LOC_CountryModel country = new LOC_CountryModel();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        country.CountryName = sdr["CountryName"].ToString();
                        country.CountryCode = sdr["CountryCode"].ToString();
                    }
                }
                return View("AddEdit", country);
            }

            return View("AddEdit");
        }
        [HttpPost]
        //public IActionResult Save(LOC_CountryModel CountryModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        SqlConnection Conn = new SqlConnection(this.Configuration.GetConnectionString("myConnection"));
        //        Conn.Open();
        //        SqlCommand Cmd = Conn.CreateCommand();

        //        Cmd.CommandType = CommandType.StoredProcedure;

        //        if (CountryModel.CountryID == null)
        //        {
        //            Cmd.CommandText = "PR_Country_Insert";
        //        }
        //        else
        //        {
        //            Cmd.CommandText = "PR_Branch_Update";
        //            Cmd.Parameters.AddWithValue("@CountryID", CountryModel.CountryID);
        //        }

        //        Cmd.Parameters.AddWithValue("@CountryName", CountryModel.CountryName);
        //        Cmd.Parameters.AddWithValue("@CountryCode", CountryModel.CountryCode);
        //        if (Convert.ToBoolean(Cmd.ExecuteNonQuery()))
        //        {
        //            if (CountryModel.CountryID == null)
        //                TempData["Message"] = "Record Inserted Successfully";
        //            else
        //            {
        //                TempData["Message"] = "Record Updated Successfully";
        //                return RedirectToAction("Index");
        //            }
        //        }
        //    }

        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public IActionResult Save(LOC_CountryModel CountryModel)
        {
            if (CountryModel.ImageFile != null && CountryModel.ImageFile.Length > 0 || ModelState.IsValid)
            {
                SqlConnection Conn = new SqlConnection(this.Configuration.GetConnectionString("myConnection"));
                Conn.Open();
                SqlCommand Cmd = Conn.CreateCommand();

                Cmd.CommandType = CommandType.StoredProcedure;

                if (CountryModel.CountryID == null)
                {
                    Cmd.CommandText = "PR_Country_Insert";
                }
                else
                {
                    Cmd.CommandText = "PR_Country_UpdateByPK";
                    Cmd.Parameters.AddWithValue("@CountryID", CountryModel.CountryID);
                }

                // Add the Country parameters
                Cmd.Parameters.AddWithValue("@CountryName", CountryModel.CountryName);
                Cmd.Parameters.AddWithValue("@CountryCode", CountryModel.CountryCode);

                // Image file upload logic
                if (CountryModel.ImageFile != null && CountryModel.ImageFile.Length > 0)
                {
                    // Define the path to save the image
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "countries");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Create unique file name to avoid overwriting
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + CountryModel.ImageFile.FileName;

                    // Combine the path and file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the image file to the path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        CountryModel.ImageFile.CopyTo(fileStream);
                    }

                    // Save the file path to the model
                    CountryModel.Photo = "/uploads/countries/" + uniqueFileName;

                    // Add the Photo path as a parameter to the stored procedure
                    Cmd.Parameters.AddWithValue("@Photo", CountryModel.Photo);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Photo", DBNull.Value);
                }

                if (Convert.ToBoolean(Cmd.ExecuteNonQuery()))
                {
                    if (CountryModel.CountryID == null)
                        TempData["Message"] = "Record Inserted Successfully";
                    else
                        TempData["Message"] = "Record Updated Successfully";

                    return RedirectToAction("Index");
                }
            }

            return View("AddEdit", CountryModel);
        }

        public IActionResult Delete(int CountryID)
        {
            SqlConnection Conn = new
            SqlConnection(Configuration.GetConnectionString("myConnection"));
            Conn.Open();
            SqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.CommandText = "PR_Country_DeleteByPK";
            Cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;
            if (Convert.ToBoolean(Cmd.ExecuteNonQuery()))
            {
                TempData["Message"] = " ";
            }
            Conn.Close();
            return RedirectToAction("Index");
        }
    }
}
