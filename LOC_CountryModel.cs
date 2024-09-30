using DocumentFormat.OpenXml.Vml;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AddressBook_1182.Areas.LOC_Country.Models
{
    public class LOC_CountryModel
    {
        public int? CountryID { get; set; }
        [Required]
        [DisplayName("Country Name")]
        public string? CountryName { get; set; }
        [Required]
        [DisplayName("Country Code")]
        public string? CountryCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Photo { get; set; }
        public IFormFile ImageFile { get; set; }
    }
    public class LOC_CountryDropDownModel
    {
        public int? CountryID { get; set; }
        [Required]
        [DisplayName("Country Name")]
        public string? CountryName { get; set; }
    }
    public class SearchModel
    {
        public string? CountryName { get; set; }
    }
}
