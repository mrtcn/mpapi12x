$.getScript('/areas/dashboard/assets/vendor/kendo.ui/js/kendo.aspnetmvc.min.js', function () {
    var grid = $('#grid').kendoExtendedGrid({
        dataSource: {            
            batch: true,
            transport: {
                prefix: ""
                //update: {
                //    url: "/AboutUs/UpdateSorts",
                //    type: "post"
                //}
            },
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: {type: "number"},
                        Name: {type: "string"},
                        Status: {type: "string"},
                        CreationDate: { type: "date" },
                        ModificationDate: { type: "date" }
                    }
                }
            },
            sort:[{field: "Id", dir: "asc"}]
        },
        pdf: {
            fileName: "aktor"
        },
        excel: {
            fileName: "aktor"
        },        
        columns: [
            {
                field: "Id",
                title: "Id",
                width: 70
            }, {
                field: "Name",
                title: "Aktör Adı"
            }, {
                field: "Status",
                title: "Durum",
                values: app.GetStatusValues()
            }, {
                field: "CreationDate",
                title: "Oluşturma",
                format: "{0:dd.MM.yyyy HH:mm}",
                width: 120
            }, {
                field: "ModificationDate",
                title: "Güncelleme",
                format: "{0:dd.MM.yyyy HH:mm}",
                width: 120
            }
        ]

    }).data("kendoExtendedGrid");
});
