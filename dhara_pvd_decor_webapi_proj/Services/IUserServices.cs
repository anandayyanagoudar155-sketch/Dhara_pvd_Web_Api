using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IUserServices
    {
        Task<int> AddUser(UserController.AddUserRequest request);

        Task<int> UpdateUser(UserController.UpdateUserRequest request);

        Task<int> DeleteUser(long id);

        Task<List<UserController.User_List>> GetUserList();

        Task<UserController.SingleUser?> GetUserById(long id);

        Task<List<UserController.Drop_User_List>> GetDropdownUserList();

        Task<int> AddUserDetails(UserController.AddUserDetailsRequest request);

        Task<int> AddMultipleUserDetails(UserController.AddUserDetailsRequest request);

        Task<int> UpdateUserDetails(UserController.UpdateUserDetailsRequest request);

        Task<int> DeleteUserDetails(long id);

        Task<List<UserController.UserDetails_List>> GetUserDetailsList();

        Task<List<UserController.Multiple_UserDetails_List>> GetMultipleUserDetailsByUserId(long userId);
    }
}

