using System.Collections.Generic;
using System.Threading.Tasks;
using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services.Interfaces
{
    public interface ICityService
    {
        Task<bool> AddCity(CityController.AddCityRequest request);
        Task<bool> DeleteCity(long id);
        Task<bool> UpdateCity(CityController.UpdatecityRequest request);
        Task<IEnumerable<CityController.city_list>> GetCityList();
        Task<CityController.Single_city_list?> GetCityById(long id);
        Task<IEnumerable<CityController.drop_city_list>> GetDropdownCityList(long stateId);
    }
}
