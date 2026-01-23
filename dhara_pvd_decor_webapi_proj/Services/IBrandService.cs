using System.Collections.Generic;
using System.Threading.Tasks;
using static dhara_pvd_decor_webapi_proj.Controllers.BrandController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IBrandService
    {
        Task<int> AddBrand(AddBrandRequest request);
        Task<int> DeleteBrand(long id);
        Task<int> UpdateBrand(UpdateBrandRequest request);
        Task<List<Brand_list>> GetBrandList();
        Task<Single_Brand_list?> GetBrandById(long id);
        Task<List<drop_Brand_list>> GetDropBrandList();
    }
}