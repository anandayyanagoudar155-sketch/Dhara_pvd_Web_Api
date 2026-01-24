using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IProdTypeService
    {
        Task<int> AddProdtype(ProdTypeController.AddProdtypeRequest request);
        Task<int> UpdateProdtype(ProdTypeController.UpdateProdtypeRequest request);
        Task<int> DeleteProdtype(long id);
        Task<List<ProdTypeController.Prodtype_list>> GetProdtypeList();
        Task<ProdTypeController.Single_Prodtype_list?> GetProdtypeById(long id);
        Task<List<ProdTypeController.drop_Prodtype_list>> GetDropProdtypeList();

    }

}



