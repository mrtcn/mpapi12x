using System.Collections.Generic;
using System.Web.Mvc;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel {
    public class RegisterViewModel {
        public RegisterViewModel()
        { }
        public RegisterViewModel(dynamic model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
            PhoneNumber = model.PhoneNumber;
            UserName = model.UserName;
            ImagePath = model.ImagePath;
        }
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