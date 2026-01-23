using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IStateService
    {
        Task<bool> AddState(StateController.AddStateRequest request);
        Task<bool> DeleteState(long id);
        Task<bool> UpdateState(StateController.UpdateStateRequest request);
        Task<List<StateController.state_list>> GetStateList();
        Task<StateController.Single_state_list?> GetStateById(long id);
        Task<List<StateController.drop_state_list>> GetDropStateList(long country_id);
    }
}
