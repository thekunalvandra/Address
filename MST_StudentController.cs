using AddressBook_1182.Areas.LOC_City.Models;
using AddressBook_1182.Areas.MST_Branch.Models;
using AddressBook_1182.Areas.MST_Student.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using SearchModel = AddressBook_1182.Areas.MST_Student.Models.SearchModel;

namespace addressbook_1182.areas.mst_student.controllers
{
    [Area("MST_Student")]
    [Route("MST_Student/[Controller]/[action]")]
    public class MST_StudentController : Controller
    {
        private IConfiguration Configuration;

        public MST_StudentController(IConfiguration _configuration)
        {

            Configuration = _configuration;

        }
        public IActionResult Index()
        {
            FillCityDDL();
            FillBranchDDL();
            string connectionstr = Configuration.GetConnectionString("myConnection");

            SqlDatabase sqlDB = new SqlDatabase(connectionstr);
            DbCommand dbCMD = sqlDB.GetStoredProcCommand("PR_Student_SelectAll");

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
            ObjCmd.CommandText = "PR_Student_Search";
            ObjCmd.Parameters.AddWithValue("StudentName", model.StudentName);
            SqlDataReader sqlDataReader = ObjCmd.ExecuteReader();
            dt.Load(sqlDataReader);
            return View("Index", dt);
        }
        public IActionResult AddEdit(int? StudentID)
        {

            FillCityDDL();
            FillBranchDDL();
            if (StudentID != null)
            {
                SqlConnection Conn = new
               SqlConnection(Configuration.GetConnectionString("myConnection"));
                Conn.Open();
                SqlCommand Cmd = Conn.CreateCommand();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = "PR_Student_SelectByPK";
                Cmd.Parameters.AddWithValue("@StudentID", StudentID);

                SqlDataReader sdr = Cmd.ExecuteReader();
                MST_StudentModel country = new MST_StudentModel();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        country.StudentName = sdr["StudentName"].ToString();
                        country.BranchID = Convert.ToInt32(sdr["BranchID"]);
                        country.CityID = Convert.ToInt32(sdr["CityID"]);
                        country.Age = Convert.ToInt32(sdr["Age"]);
                        country.MobileNoFather = sdr["MobileNoFather"].ToString();
                        country.MobileNoStudent = sdr["MobileNoStudent"].ToString();
                        country.Address = sdr["Address"].ToString();
                        country.BirthDate = Convert.ToDateTime(sdr["BirthDate"]);
                        country.IsActive = Convert.ToBoolean(sdr["IsActive"]);
                        country.Gender = sdr["Gender"].ToString();
                        country.Password = sdr["Password"].ToString();
                    }
                }
                return View("AddEdit", country);
            }

            return View("AddEdit");
        }
        [HttpPost]
        public IActionResult Save(MST_StudentModel MST_StudentModel)
        {

            SqlConnection Conn = new SqlConnection(this.Configuration.GetConnectionString("myConnection"));
            Conn.Open();
            SqlCommand Cmd = Conn.CreateCommand();

            Cmd.CommandType = CommandType.StoredProcedure;

            if (MST_StudentModel.StudentID == null)
            {
                Cmd.CommandText = "PR_Student_Insert";
            }
            else
            {
                Cmd.CommandText = "PR_Student_UpdateByPK";
                Cmd.Parameters.AddWithValue("@StudentID", MST_StudentModel.StudentID);
            }
            Cmd.Parameters.AddWithValue("@StudentName", MST_StudentModel.StudentName);
            Cmd.Parameters.AddWithValue("@BranchID", MST_StudentModel.BranchID);
            Cmd.Parameters.AddWithValue("@CityID", MST_StudentModel.CityID);
            Cmd.Parameters.AddWithValue("@Age", MST_StudentModel.Age);
            Cmd.Parameters.AddWithValue("@Email", MST_StudentModel.Email);
            Cmd.Parameters.AddWithValue("@MobileNoStudent", MST_StudentModel.MobileNoStudent);
            Cmd.Parameters.AddWithValue("@MobileNoFather", MST_StudentModel.MobileNoFather);
            Cmd.Parameters.AddWithValue("@Address", MST_StudentModel.Address);
            Cmd.Parameters.AddWithValue("@BirthDate", MST_StudentModel.BirthDate);
            Cmd.Parameters.AddWithValue("@IsActive", MST_StudentModel.IsActive);
            Cmd.Parameters.AddWithValue("@Gender", MST_StudentModel.Gender);
            Cmd.Parameters.AddWithValue("@Password", MST_StudentModel.Password);
            //Cmd.Parameters.AddWithValue("@Created", MST_StudentModel.Created);
            //Cmd.Parameters.AddWithValue("@Modified", MST_StudentModel.Modified);
            if (Convert.ToBoolean(Cmd.ExecuteNonQuery()))
            {
                if (MST_StudentModel.StudentID == null)
                {
                    TempData["Message"] = "Record Inserted Successfully";
                }
                else
                {
                    TempData["Message"] = "Record Updated Successfully";
                    return RedirectToAction("Index");
                }
            }


            return RedirectToAction("Index");
        }
        public IActionResult Delete(int StudentID)
        {
            SqlConnection Conn = new
            SqlConnection(Configuration.GetConnectionString("myConnection"));
            Conn.Open();
            SqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.CommandText = "PR_Student_DeleteByPK";
            Cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = StudentID;
            if (Convert.ToBoolean(Cmd.ExecuteNonQuery()))
            {
                TempData["Message"] = " ";
            }
            Conn.Close();
            return RedirectToAction("Index");
        }
        public void FillCityDDL()
        {

            string str =
           this.Configuration.GetConnectionString("myConnection");
            List<LOC_CityDropDownModel> loc_City = new
           List<LOC_CityDropDownModel>();
            SqlConnection objConn = new SqlConnection(str);
            objConn.Open();
            SqlCommand objCmd = objConn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_City_SelectDropDownList";
            SqlDataReader objSDR = objCmd.ExecuteReader();
            if (objSDR.HasRows)
            {
                while (objSDR.Read())
                {
                    LOC_CityDropDownModel city = new
                    LOC_CityDropDownModel()
                    {
                        CityID = Convert.ToInt32(objSDR["CityID"]),
                        CityName = objSDR["CityName"].ToString()
                    };
                    loc_City.Add(city);
                }
                objSDR.Close();
            }
            objConn.Close();
            ViewBag.CityList = loc_City;
        }
        public void FillBranchDDL()
        {

            string str =
           this.Configuration.GetConnectionString("myConnection");
            List<LOC_BranchDropDownModel> loc_Branch = new
           List<LOC_BranchDropDownModel>();
            SqlConnection objConn = new SqlConnection(str);
            objConn.Open();
            SqlCommand objCmd = objConn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_Branch_SelectDropDownList";
            SqlDataReader objSDR = objCmd.ExecuteReader();
            if (objSDR.HasRows)
            {
                while (objSDR.Read())
                {
                    LOC_BranchDropDownModel Branch = new
                    LOC_BranchDropDownModel()
                    {
                        BranchID = Convert.ToInt32(objSDR["BranchID"]),
                        BranchName = objSDR["BranchName"].ToString()
                    };
                    loc_Branch.Add(Branch);
                }
                objSDR.Close();
            }
            objConn.Close();
            ViewBag.BranchList = loc_Branch;
        }
    }
}
