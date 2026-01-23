using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IPayTypeService
    {
        Task<int> AddPaytype(PayTypeController.AddPaytypeRequest request);
        Task<int> UpdatePaytype(PayTypeController.UpdatePaytypeRequest request);
        Task<int> DeletePaytype(long id);
        Task<List<PayTypeController.Paytype_list>> GetPaytypeList();
        Task<PayTypeController.Single_Paytype_list?> GetPaytypeById(long id);
        Task<List<PayTypeController.drop_Paytype_list>> GetDropdownPaytypeList();
    }
}