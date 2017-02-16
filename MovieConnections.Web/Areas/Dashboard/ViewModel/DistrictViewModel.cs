using System.Collections.Generic;
using System.Web.Mvc;
using MovieConnections.Core.EntityParams;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel {
    public class DistrictViewModel : DistrictParams {
        public IEnumerable<SelectListItem> CitySelectListItems { get; set; }
    }
}