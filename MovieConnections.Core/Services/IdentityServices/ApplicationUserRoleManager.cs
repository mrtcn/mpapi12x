using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieConnections.Core.Services.IdentityServices
{
    public class ApplicationUserRoleManager {
        private readonly ApplicationUserManager _applicationUserManager;
        private readonly ApplicationRoleManager _applicationRoleManager;

        public ApplicationUserRoleManager(ApplicationUserManager applicationUserManager
            , ApplicationRoleManager applicationRoleManager) {
            _applicationUserManager = applicationUserManager;
            _applicationRoleManager = applicationRoleManager;
        }



        public async Task RemoveExceptedRolesAsync(List<int> userRoleIds, List<int> roleIdList, int id)
        {
            var idsToBeDeleted = roleIdList == null ? userRoleIds : userRoleIds.Except(roleIdList);
            if (idsToBeDeleted == null)
                return;

            foreach (var idToBeDeleted in idsToBeDeleted)
            {
                var role = _applicationRoleManager.FindByIdAsync(idToBeDeleted).Result;
                if (role == null)
                    continue;
                await _applicationUserManager.RemoveFromRoleAsync(id, role.Name);
                
            }
        }

        public async Task CreateNotContainedRolesAsync(List<int> userRoleIds, List<int> roleIdList, int id)
        {
            var missingIds = userRoleIds == null ? roleIdList : roleIdList.Except(userRoleIds).ToList();
            if (missingIds == null || !missingIds.Any())
                return; ;
            foreach (var missingId in missingIds)
            {
                var role = _applicationRoleManager.FindByIdAsync(missingId).Result;
                if (role == null)
                    continue;
                await _applicationUserManager.AddToRoleAsync(id, role.Name);
            }
        }
    }
}
