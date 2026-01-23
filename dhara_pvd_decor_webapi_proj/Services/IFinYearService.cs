using System.Collections.Generic;
using System.Threading.Tasks;
using static dhara_pvd_decor_webapi_proj.Controllers.FinYearController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IFinYearService
    {
        Task<int> AddFinYear(AddFinYearRequest request);
        Task<int> DeleteFinYear(long id);
        Task<int> UpdateFinYear(UpdateFinYearRequest request);
        Task<List<FinYearlist>> GetFinYearList();
        Task<Single_FinYear_list?> GetFinYearById(long id);
        Task<List<drop_FinYear_list>> GetDropFinYearList(long userId);
    }
}
