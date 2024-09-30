using AddressBook_1182.Areas.LOC_City.Models;
using AddressBook_1182.Areas.LOC_Country.Models;
using AddressBook_1182.Areas.LOC_State.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using SearchModel = AddressBook_1182.Areas.LOC_City.Models.SearchModel;

namespace AddressBook_1182.Areas.LOC_City.Controllers
{
    [Area("LOC_City")]
    [Route("LOC_City/[Controller]/[action]")]
    public class LOC_CityController : Controller
    {
        private IConfiguration Configuration;

        public LOC_CityController(IConfiguration _configuration)
        {

            Configuration = _configuration;

        }
       
        public IActionResult Index()
        {
            FillCountryDDL();
            FillStateDDL();
            DataTable dt = new DataTable();
            string str = this.Configuration.GetConnectionString("myConnection");
            SqlConnection conn1 = new SqlConnection(str);
            conn1.Open();
            SqlCommand objCmd = conn1.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_City_SelectAll";
            SqlDataReader objSDR = objCmd.ExecuteReader();
            if (objSDR.HasRows)
            {
                dt.Load(objSDR);
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
            ObjCmd.CommandText = "PR_City_Search";
            ObjCmd.Parameters.AddWithValue("CityName", model.CityName);
            SqlDataReader sqlDataReader = ObjCmd.ExecuteReader();
            dt.Load(sqlDataReader);
            return View("Index", dt);
        }
   
        public IActionResult AddEdit(int? CityID)
        {
            FillCountryDDL();
            FillStateDDL();
            if (CityID != null)
            {
                SqlConnection objConn = new
               SqlConnection(this.Configuration.GetConnectionString("myConnection"));
                objConn.Open();
                SqlCommand objCmd = objConn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = "PR_City_SelectByPK";
                objCmd.Parameters.AddWithValue("@CityID", CityID);
                SqlDataReader objSDR = objCmd.ExecuteReader();
                LOC_CityModel city = new LOC_CityModel();
                if (objSDR.HasRows)
                {
                    while (objSDR.Read())
                    {
                        city.CityName = objSDR["CityName"].ToString();
                        city.CountryID = Convert.ToInt32(objSDR["CountryID"]);
                        city.StateID = Convert.ToInt32(objSDR["StateID"]);
                        city.CityCode = objSDR["CityCode"].ToString();
                    }
                }
                return View("AddEdit", city);
            }

            return View("AddEdit");
        }
 
        [HttpPost]
        public IActionResult Save(LOC_CityModel model_LOC_City)
        {
            if (ModelState.IsValid)
            {
                SqlConnection objConn = new
               SqlConnection(this.Configuration.GetConnectionString("myConnection"));
                objConn.Open();
                SqlCommand objCmd = objConn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                if (model_LOC_City.CityID == null)
                {
                    objCmd.CommandText = "PR_City_Insert";
                }
                else
                {
                    objCmd.CommandText = "PR_City_UpdateByPK";
                    objCmd.Parameters.AddWithValue("@CityID", model_LOC_City.CityID);
                }
                objCmd.Parameters.AddWithValue("@CityName", model_LOC_City.CityName);
                objCmd.Parameters.AddWithValue("@CityCode", model_LOC_City.CityCode);
                objCmd.Parameters.AddWithValue("@CountryID", model_LOC_City.CountryID);
                objCmd.Parameters.AddWithValue("@StateID", model_LOC_City.StateID);
                if (Convert.ToBoolean(objCmd.ExecuteNonQuery()))
                {
                    if (model_LOC_City.CityID == null)
                        TempData["Message"] = "Record Inserted Successfully";
                    else
                    {
                        TempData["Message"] = "Record Updated Successfully";
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }
  
        public IActionResult Delete(int CityID)
        {
            SqlConnection objConn = new
           SqlConnection(this.Configuration.GetConnectionString("myConnection"));
            objConn.Open();
            SqlCommand objCmd = objConn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_City_DeleteByPK";
            objCmd.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;
            if (Convert.ToBoolean(objCmd.ExecuteNonQuery()))
            {
                TempData["Message"] = " ";
            }
            objConn.Close();
            return RedirectToAction("Index");
        }
       
        public void FillCountryDDL()
        {

            string str =
           this.Configuration.GetConnectionString("myConnection");
            List<LOC_CountryDropDownModel> loc_Country = new
           List<LOC_CountryDropDownModel>();
            SqlConnection objConn = new SqlConnection(str);
            objConn.Open();
            SqlCommand objCmd = objConn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_Country_SelectDropDownList";
            SqlDataReader objSDR = objCmd.ExecuteReader();
            if (objSDR.HasRows)
            {
                while (objSDR.Read())
                {
                    LOC_CountryDropDownModel country = new
                    LOC_CountryDropDownModel()
                    {
                        CountryID = Convert.ToInt32(objSDR["CountryID"]),
                        CountryName = objSDR["CountryName"].ToString()
                    };
                    loc_Country.Add(country);
                }
                objSDR.Close();
            }
            objConn.Close();
            ViewBag.CountryList = loc_Country;
        }
       
        public void FillStateDDL()
        {

            string str =
           this.Configuration.GetConnectionString("myConnection");
            List<LOC_StateDropDownModel> loc_State = new
           List<LOC_StateDropDownModel>();
            SqlConnection objConn = new SqlConnection(str);
            objConn.Open();
            SqlCommand objCmd = objConn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_State_SelectDropDownList";
            SqlDataReader objSDR = objCmd.ExecuteReader();
            if (objSDR.HasRows)
            {
                while (objSDR.Read())
                {
                    LOC_StateDropDownModel state = new
                    LOC_StateDropDownModel()
                    {
                        StateID = Convert.ToInt32(objSDR["StateID"]),
                        StateName = objSDR["StateName"].ToString()
                    };
                    loc_State.Add(state);
                }
                objSDR.Close();
            }
            objConn.Close();
            ViewBag.StateList = loc_State;
        }

    }
}
