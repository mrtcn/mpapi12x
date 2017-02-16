using System;

namespace MovieConnections.Data.Models
{
    public enum PredefinedPage
    {
        None = 0,
        [PredefinedPageInfo(Controller = "Country", Action = "Index")]
        Country = 1,
        [PredefinedPageInfo(Controller = "City", Action = "Index")]
        City = 2,
        [PredefinedPageInfo(Controller = "District", Action = "Index")]
        District = 3
    }

    public class PredefinedPageInfoAttribute : Attribute {
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}