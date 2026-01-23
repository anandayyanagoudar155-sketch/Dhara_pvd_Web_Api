using System.Collections.Generic;
using System.Threading.Tasks;
using static dhara_pvd_decor_webapi_proj.Controllers.ColourController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IColourService
    {
        Task<int> AddColour(AddColourRequest request);
        Task<int> DeleteColour(long id);
        Task<int> UpdateColour(UpdateColourRequest request);
        Task<List<Colour_list>> GetColourList();
        Task<Single_Colour_list?> GetColourById(long id);
        Task<List<drop_Colour_list>> GetDropColourList();
    }
}
