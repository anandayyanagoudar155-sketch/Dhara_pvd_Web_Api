using System.Collections.Generic;
using System.Threading.Tasks;
using static dhara_pvd_decor_webapi_proj.Controllers.MonthController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IMonthService
    {
        Task<int> AddMonth(AddMonthRequest request);
        Task<int> DeleteMonth(long id);
        Task<int> UpdateMonth(UpdateMonthRequest request);
        Task<List<month_list>> GetMonthList();
        Task<Single_month_list?> GetMonthById(long id);
        Task<List<drop_month_list>> GetDropMonthList();
    }
}
