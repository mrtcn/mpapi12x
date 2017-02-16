$.getScript('/areas/dashboard/assets/vendor/kendo.ui/js/kendo.aspnetmvc.min.js', function () {

    var editPermission = $("#grid").attr("data-edit") || false;

    var toolbar = [{
        name: "select-role",
        template: "<input id=\"RoleList\" class=\"form-control input-sm\" style=\"width:280px;\"/>"
    }];

    if (editPermission)
        toolbar = $.merge([
        {
            name: "save-changes",
            template: kendo.format("<a id=\"saveChanges\" type=\"button\" class=\"k-button k-button-text pull-right\" title={0} >{0}</a>", "Değişiklikleri Kaydet")
        }], toolbar);

    var grid = $('#grid').kendoExtendedGrid({
        dataSource: {            
            transport: {
                read: {
                    url: "/Dashboard/RoleModulePermission/List",
                    type: "post"
                }
            },
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { type: "number" },
                        ModuleId: { type: "number" },
                        ModuleName: { type: "string" },
                        Create: { type: "bool" },
                        Edit: { type: "bool" },
                        Delete: { type: "bool" },
                        Export: { type: "bool" },
                        View: { type: "bool" }
                    }
                }
            },            
            sort: [{ field: "Id", dir: "asc" }]
        },
        groupable: false,
        filterable: false,
        toolbar: toolbar,
        pdf: {
            fileName: "kullanici-modul-izin"
        },
        excel: {
            fileName: "kullanici-modul-izin"
        },        
        columns: [
            {
                field: "Id",
                title: "Id",
                width: 90
            },{
                field: "ModuleName",
                title: "Modül Adı"
            }, {
                field: "Create",
                title: "<input class='multicheck' type='checkbox' data-multicheck='.create-multi'> Oluşturma",
                width: 160,
                sortable: false,
                template: "<input type=\"checkbox\" class=\"create-multi\" name=\"Create\" #= Create ? checked=\"checked\" : \"\" #/>"
            }, {
                field: "Edit",
                title: "<input class='multicheck' type='checkbox' data-multicheck='.edit-multi'> Düzenleme",
                width: 160,
                sortable: false,
                template: "<input type=\"checkbox\" class=\"edit-multi\" name=\"Edit\" #= Edit ? 'checked=\"checked\"' : \"\" # />"
            }, {
                field: "Delete",
                title: "<input class='multicheck' type='checkbox' data-multicheck='.delete-multi'> Silme",
                width: 160,
                sortable: false,
                template: "<input type=\"checkbox\" class=\"delete-multi\" name=\"Delete\" #= Delete ? 'checked=\"checked\"' : \"\" # />"
            }, {
                field: "Export",
                title: "<input class='multicheck' type='checkbox' data-multicheck='.export-multi'> Dışarı Aktarma",
                width: 160,
                sortable: false,
                template: "<input type=\"checkbox\" class=\"export-multi\" name=\"Export\" #= Export ? 'checked=\"checked\"' : \"\" # />"
            }, {
                field: "View",
                title: "<input class='multicheck' type='checkbox' data-multicheck='.view-multi'> Görüntüleme",
                width: 160,
                sortable: false,
                template: "<input type=\"checkbox\" class=\"view-multi\" name=\"View\" #= View ? 'checked=\"checked\"' : \"\" # />"
            }
        ]

    }).data("kendoExtendedGrid");

    var roleList = $("#RoleList").kendoComboBox({
        placeholder: "Grup Seçiniz",
        dataTextField: "Text",
        dataValueField: "Value",
        filter: "contains",
        autoBind: true,
        dataSource: {
            transport: {
                read: {
                    url: "/Dashboard/RoleModulePermission/GetRoles",
                    type: "POST"
                }
            }            
        },
        change: function () {
            $.ajax({
                type: "POST",
                url: "/Dashboard/RoleModulePermission/List",
                data: { id: this.value() },
                async: true,
                success: function (data) {
                    grid.dataSource.data(data);
                    grid.refresh();
                }
            });
        }
    });

    $(".multicheck").makeAllChecked("data-multicheck");

    $("#saveChanges").on("click", function () {
        $.ajax({
            type: "POST",
            url: "/Dashboard/RoleModulePermission/CreateOrUpdate",
            data: {
                items: JSON.stringify(grid.dataSource.data()),
                roleId: $("#RoleList").data("kendoComboBox").value()
            },
            async: false,
            success: function () {
                $.notify("Yetkilendirme işlemi başarıyla gerçekleşti.", "success");
            },
            error: function () {
                $.notify("Yetkilendirme işlemi sırasında bir hata gerçekleşti!", "error");
            }
        });
    });
    
});
