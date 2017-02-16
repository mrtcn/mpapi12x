using System.ComponentModel.DataAnnotations;

namespace MovieConnections.Web.Areas.Dashboard.Models {
    public enum DashboardControllerType {
        [Display(Name = "")]
        Null,
        [Display(Name = "Bağımsız")]
        Independent,
        [Display(Name = "Şehir Yönetimi")]
        City,
        [Display(Name = "Ülke Yönetimi")]
        Country,
        [Display(Name = "İlçe Yönetimi")]
        District,
        [Display(Name = "Dashboard Yönetimi")]
        DashboardManagement,
        [Display(Name = "Dashboard İzinleri")]
        DashboardPermission,
        [Display(Name = "Kullanıcı Yönetimi")]
        Register,
        [Display(Name = "Rol Yönetimi")]
        Role,
        [Display(Name = "Kullanıcı İzin")]
        UserModulePermission,
        [Display(Name = "Role İzin")]
        RoleModulePermission,
        [Display(Name = "Yabancı Dil")]
        Culture,
        [Display(Name = "Film Yönetimi")]
        Movie,
        [Display(Name = "Aktör Yönetimi")]
        Actor,
        [Display(Name = "Kategori Yönetimi")]
        Genre
    }
}