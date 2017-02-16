$.getScript('/areas/dashboard/assets/vendor/kendo.ui/js/kendo.aspnetmvc.min.js', function () {
    var grid = $('#grid').kendoExtendedGrid({
        dataSource: {            
            transport: {
                read: {
                    url: "/Register/List",
                    type: "post"
                }
            },
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { type: "string" },
                        FirstName: { type: "string" },
                        LastName: { type: "string" },
                        UserName: { type: "string" }
                    }
                }
            },
            sort:[{field: "Id", dir: "asc"}]
        },
        pdf: {
            fileName: "kullanicilar"
        },
        excel: {
            fileName: "kullanicilar"
        },        
        columns: [
            {
                field: "Id",
                title: "Id",
                width: 70
            },{
                field: "FirstName",
                title: "Ad"
            },{
                field: "LastName",
                title: "Soyad"
            }, {
                field: "UserName",
                title: "Kullanıcı Adı"
            }
        ]

    }).data("kendoExtendedGrid");
});
