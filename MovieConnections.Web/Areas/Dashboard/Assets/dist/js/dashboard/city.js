$.getScript('/areas/dashboard/assets/vendor/kendo.ui/js/kendo.aspnetmvc.min.js', function () {
    var grid = $('#grid').kendoExtendedGrid({
        dataSource: {            
            transport: {
                read: {
                    url: "/Dashboard/City/List",
                    type: "post"
                }
            },
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: {type: "number"},
                        Name: {type: "string"}
                    }
                }
            },
            sort:[{field: "Id", dir: "asc"}]
        },
        pdf: {
            fileName: "sehirler"
        },
        excel: {
            fileName: "sehirler"
        },        
        columns: [
            {
                field: "Id",
                title: "Id",
                width: 70
            }, {
                field: "Name",
                title: "Ad"
            }
        ]

    }).data("kendoExtendedGrid");
});
