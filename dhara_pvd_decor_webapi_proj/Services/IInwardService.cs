
using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IInwardService
    {
        Task<int> AddInward(InwardController.AddInwardRequest request);
        Task<int> Updateinward(InwardController.UpdateInwardRequest request);
        Task<int> DeleteInward(long id);
        Task<List<InwardController.Inward_List>> GetInwardList();
        Task<InwardController.SingleInwardList?> GetInwardById(long id);
        Task<List<InwardController.Drop_Inward_List>> Get_drop_inwardlist();
        Task<int> AddInwardDetails(InwardController.AddInwardDetailsRequest request);
        Task<int> UpdateInwardDetails(InwardController.UpdateInwardDetailsRequest request);
        Task<int> DeleteInwardDetails(long id);
        Task<List<InwardController.Inward_Details_List>> GetInwardDetailsList();
        Task<List<InwardController.SingleInwardDetailsList>> GetInwardDetailsByInwardId(long id);


    }
}
