using System.Collections.Generic;
using System.Web.Mvc;

namespace MovieConnections.Api.ViewModel {
    public class RegisterViewModel {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string ImagePath { get; set; }
        public string Password { get; set; }
        public IEnumerable<int> RoleIds { get; set; }
        public MultiSelectList RoleMultiSelectList { get; set; }
    }
}