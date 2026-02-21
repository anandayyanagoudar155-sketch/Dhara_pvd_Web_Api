using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface ICustomerService
    {
        Task<int> AddCustomer(CustomerController.AddCustomerRequest request);
        Task<int> UpdateCustomer(CustomerController.UpdateCustomerRequest request);
        Task<int> DeleteCustomer(long id);
        Task<List<CustomerController.Customer_List>> Get_Customer_List();
        Task<CustomerController.Single_Customer_List?> GetCustomerById(long id);
        Task<List<CustomerController.Drop_Customer_List>> Get_drop_customerlist();
        Task<int> Add_CustDetail_Request(CustomerController.Add_CustDetail_Request request);
        Task<int> UpdateCustDetail(CustomerController.Update_CustDetail_Request request);
        Task<int> DeleteCustDetail(long id);
        Task<List<CustomerController.CustDetail_List>> Get_CustDetail_list();
        Task<List<CustomerController.Single_CustDetail>> Get_CustDetail_by_id(long id);
        Task<List<CustomerController.Drop_CustDetail>> Get_drop_custdetail_list(long Comp_id, long Fin_year_id);

    }

}





