using System.Collections.Generic;
using System.Threading.Tasks;
using static dhara_pvd_decor_webapi_proj.Controllers.UnitController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IUnitService
    {
        Task<int> AddUnit(AddUnitRequest request);
        Task<int> DeleteUnit(long id);
        Task<int> UpdateUnit(UpdateUnitRequest request);
        Task<List<Unit_list>> GetUnitList();
        Task<Single_Unit_list?> GetUnitById(long id);
        Task<List<drop_Unit_list>> GetDropUnitList();
    }
}