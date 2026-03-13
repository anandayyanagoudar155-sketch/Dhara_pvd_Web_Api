
using dhara_pvd_decor_webapi_proj.Controllers;
using static dhara_pvd_decor_webapi_proj.Controllers.InwardReturnController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IInwardReturnService
    {
        Task<int> Add_inwardreturn(InwardReturnController.AddInwardreturnRequest request);
        Task<int> UpdateInwardReturn(InwardReturnController.UpdateInwardreturnRequest request);
        Task<int> DeleteInwardReturn(long id);
        Task<List<InwardReturnController.Inwardreturn_List>> Get_inwardreturn_list();
        Task<List<InwardReturnController.SingleInwardreturn>> Get_InwardReturn_by_id(long id);
        Task<List<InwardReturnController.Drop_Ir_InwardDetail>> Get_inward_for_return(long customer_id, long comp_id, long fin_year_id);
        Task<List<InwardReturnController.Drop_Ir_ProductDetail>> Get_products_for_return(long inward_id);


    }
}
