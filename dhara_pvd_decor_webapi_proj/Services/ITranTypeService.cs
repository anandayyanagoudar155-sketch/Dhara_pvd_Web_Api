using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface ITranTypeService
    {
        Task<int> AddTransType(TranTypeController.AddTrans_typeRequest request);
        Task<int> UpdateTransType(TranTypeController.UpdateTrans_typeRequest request);
        Task<int> DeleteTransType(long id);
        Task<List<TranTypeController.trans_type_List>> Get_trans_type_list();
        Task<TranTypeController.Singletrans_type?> Get_trans_type_by_id(long id);
        Task<List<TranTypeController.Drop_trans_type_List>> Get_drop_trans_type_list();

    }

}


