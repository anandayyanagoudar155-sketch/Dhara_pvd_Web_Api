using dhara_pvd_decor_webapi_proj.Controllers;
using static dhara_pvd_decor_webapi_proj.Controllers.CompanyController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface ICompanyService
    {
        Task<bool> AddCompany(CompanyController.AddCompanyRequest request);

        Task<bool> DeleteCompany(long id);

        Task<bool> UpdateCompany(CompanyController.UpdateCompanyRequest request);

        Task<List<CompanyController.company_list>> GetCompanyList();

        Task<CompanyController.single_company_list?> GetCompanyById(long id);

        Task<List<CompanyController.drop_company_list>> GetDropCompanyList(long userId);

        Task<CompanyController.CompanyLogoResponse?> GetCompanyLogoById(long compId);
    }
}
