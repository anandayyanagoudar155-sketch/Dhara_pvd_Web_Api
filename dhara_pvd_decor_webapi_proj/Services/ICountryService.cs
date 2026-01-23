
using System.Collections.Generic;
using System.Threading.Tasks;
using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface ICountryService
    {
        Task<bool> AddCountry(CountryController.AddCountryRequest request);
        Task<bool> DeleteCountry(long id);
        Task<bool> UpdateCountry(CountryController.UpdateCountryRequest request);
        Task<List<CountryController.country_list>> GetCountryList();
        Task<CountryController.Single_country_list?> GetCountryById(long id);
        Task<List<CountryController.drop_country_list>> GetDropCountryList();
    }
}

