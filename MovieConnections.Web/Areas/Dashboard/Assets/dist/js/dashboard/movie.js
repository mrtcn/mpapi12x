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
                        Title: {type: "string"},
                        OriginalTitle: {type: "string"},
                        Director: {type: "string"},
                        Rating: {type: "number"},
                        Year: {type: "date"},
                        Country: {type: "string"},
                        NumberOfVotes: {type: "number"},
                        MovieType: { type: "string" },
                        Status: { type: "string" },
                        CreationDate: { type: "string" },
                        ModificationDate: { type: "string" }
                    }
                }
            },
            sort:[{field: "Id", dir: "asc"}]
        },
        pdf: {
            fileName: "filmler"
        },
        excel: {
            fileName: "filmler"
        },        
        columns: [
            {
                field: "Id",
                title: "Id",
                width: 70
            }, {
                field: "Title",
                title: "Film Adı"
            }, {
                field: "OriginalTitle",
                title: "Orj. Adı"
            }, {
                field: "Director",
                title: "Yönetmen"
            }, {
                field: "Rating",
                title: "Rating"
            }, {
                field: "Year",
                title: "Yıl",
                format: "{0:yyyy}"
            }, {
                field: "Country",
                title: "Ülke"
            }, {
                field: "NumberOfVotes",
                title: "Oy Sayısı"
            }, {
                field: "MovieType",
                title: "Tip",
                width: 100,
                values: app.GetMovieTypeValues()
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
