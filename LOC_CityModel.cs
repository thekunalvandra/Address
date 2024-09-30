using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AddressBook_1182.Areas.LOC_City.Models
{
    public class LOC_CityModel
    {
        public int? CityID { get; set; }
        [Required(ErrorMessage = "City Name is Required")]
        [DisplayName("City Name")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "Please Select Country")]
        [DisplayName("Country Name")]
        public int? CountryID { get; set; }
        [Required(ErrorMessage = "Please Select State")]
        [DisplayName("State Name")]
        public int? StateID { get; set; }
        [Required(ErrorMessage = "City Code is Required")]
        [DisplayName("City Code")]
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        public string CityCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
    public class LOC_CityDropDownModel
    {
        public int? CityID { get; set; }
        [Required]
        [DisplayName("City Name")]
        public string? CityName { get; set; }
    }
    public class SearchModel
    {
        public string? CityName { get; set; }
    }
}
