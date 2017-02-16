using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities.SupplementaryModels {    
    public interface ITracingFieldsModel {
        int CreatedBy { get; set; }
        int? ModifiedBy { get; set; }
        UserTypes UserType { get; set; }
    }
}