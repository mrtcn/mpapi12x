using System.ComponentModel.DataAnnotations;

namespace MovieConnections.Framework.Models {
    public enum ActionResultType {
        [Display(Name = "İşleminiz başarıyla gerçekleşti!")]
        Success,
        [Display(Name = "Beklenmedik bir hata oluştu!")]
        Failure
    }
}