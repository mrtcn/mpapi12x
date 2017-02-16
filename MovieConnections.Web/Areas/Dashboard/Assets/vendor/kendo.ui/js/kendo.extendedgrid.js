function resizeGrid(gridElement) {
    var dataArea = gridElement.find(".k-grid-content");
    var newHeight = $(window).height() - 230;
    var diff = gridElement.innerHeight() - dataArea.innerHeight();

    gridElement.height(newHeight);
    dataArea.height(newHeight - diff);
}

(function ($) {
    var kendo = window.kendo;
    var toolbar = [];

    var extendedGrid = kendo.ui.Grid.extend({
        initialLoad: true,
        options: {
            name: "ExtendedGrid",
            dataSource: {
                transport: {
                    read: {
                        url: "",
                        type: "post"
                    }
                },
                batch: false,
                schema: {
                    data: "Data",
                    total: "Total",
                    errors: "Errors"
                },
                pageSize: 25,
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                serverGrouping: true,
                serverAggregates: true,
                type: "aspnetmvc-ajax"
            },
            height: 530,
            filterable: {
                extra: false,
                operators: {
                    string: {
                        contains: "İçeriyor"
                    }
                }
            },
            ignoreCase: true,
            scrollable: true,
            groupable: true,
            sortable: true,
            pageable: {
                refresh: true,
                buttonCount: 5,
                pageSizes: [10, 25, 50, 100, 1000, 5000, 9999]
            },
            toolbar: toolbar,
            pdf: {
                fileName: "pdfExport.pdf"
            },
            excel: {
                fileName: "export.xlsx",
                filterable: true
            },
            dataBound: function() {
                var $element = this.element;
                var colCount = $element.find('.k-grid-header colgroup > col').length;

                if (this.dataSource.total() === 0) {
                    $element.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td colspan="' +
                        colCount + '">Gösterilecek kayıt yok...</td></tr>');
                }

                if ($element.parents('.tab-content').length === 0) {
                    resizeGrid($element);
                }
            }
        },
        init: function(element, options) {
            var that = this;
            this.options.dataSource.transport.read.url = $(element).attr('data-read-url');
            this.options.dataSource = $.extend({
                requestStart: function() {
                    if (!that.initialLoad) {
                        kendo.ui.progress($(element), true);
                    }
                },
                requestEnd: function() {
                    if (!that.initialLoad) {
                        kendo.ui.progress($(element), false);
                        that.initialLoad = true;
                    }
                }
            }, this.options.dataSource);
            options = $.extend(true, this.options, options);
            var modalCreatePermission = ($(element).attr('data-modal-create') && !$(element).attr('data-create')) || false;
            var createPermission = ($(element).attr('data-create') === "true" && !$(element).attr('data-show-createcolumn')) || false;
            var exportPermission = ($(element).data('export') === "true" && !$(element).data('show-exportcolumn')) || false;
            var editPermission = ($(element).attr('data-edit') === "true" && !$(element).attr('data-show-editcolumn')) || false;
            var deletePermission = ($(element).attr('data-delete') === "true" && !$(element).attr('data-show-deletecolumn')) || false;

            if(modalCreatePermission) {
                options.toolbar = $.merge([
                    {
                        name: "create",
                        template: kendo.format("<a class='k-button k-button-icontext k-grid-modal-add'" +
                            "data-toggle='modal' data-target='\\#modal' " +
                            "title='{0}'><i class='k-icon k-add'></i>{0}</a>", "Yeni Kayıt Ekle")
                    }
                ], options.toolbar || []);
            }
            if (createPermission) {
                options.toolbar = $.merge([
                    {
                        name: "create",
                        template: kendo.format("<a class='k-button k-button-icontext k-grid-add' title='{0}'>" +
                            "<i class='k-icon k-add'></i>{0}</a>", "Yeni Kayıt Ekle")
                    }
                ], options.toolbar || []);
            }

            if (exportPermission) {
                options.toolbar = $.merge([
                    {
                        name: "k-grid-excel",
                        template: kendo.format("<a class='k-button k-button-icontext k-grid-excel pull-right' " +
                            "title='{0}'><i class='k-icon k-add'></i>Excel'e Aktar</a>")
                    }
                ], options.toolbar || []);

                options.toolbar = $.merge([
                    {
                        name: "k-grid-pdf",
                        template: kendo.format("<a class='k-button k-button-icontext k-grid-pdf pull-right' " +
                            "title='{0}'><i class='k-icon k-add'></i>PDF'e Aktar</a>")
                    }
                ], options.toolbar || []);
            } else {
                $('.k-grid-pdf, .k-grid-excel').remove();
            }

            if (editPermission) {
                function editRecord(e) {
                    e.preventDefault();
                    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                    var url = fixOperationUrl($(element).attr('data-update-url'), dataItem.Id);

                    if (options.onEditButtonClicked) {
                        options.onEditButtonClicked(e, dataItem, url);
                    } else {
                        window.location = url;
                    }
                }

                options.columns = $.merge(options.columns || [], [
                    {
                        width: 86,
                        command: [{ text: options.editButtonText ? options.editButtonText : "Düzenle", click: editRecord }]
                    }
                ]);
            }

            if (deletePermission) {
                function deleteRecord(e) {
                    e.preventDefault();
                    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

                    var data = {};
                    data[dataItem.idField.toLowerCase()] = dataItem.id;

                    bootbox.confirm("İlgili kaydı silmek istediğinizden emin misiniz?", function(result) {
                        if (result === true) {
                            $.ajax({
                                type: "POST",
                                url: $(element).attr('data-remove-url'),
                                data: data,
                                async: true,
                                error: function() {
                                    $.notify("Silme işlemi yapılırken hata oluştu!", "error");
                                },
                                success: function (response) {
                                    if (response === "Success") {
                                        $.notify("Silme işlemi başarıyla gerçekleşti.", "success");
                                        that.dataSource.read();
                                    } else if (response === "HasRelatedEntities") {
                                        $.notify("Silmek istediğiniz içeriğe bağlı kayıtlar " +
                                            "olduğundan dolayı silme işlemi gerçekleştirilmedi.", "error");
                                    } else {
                                        $.notify("Silme işlemi yapılırken hata oluştu!", "error");
                                    }
                                }
                            });
                        }
                    });
                }

                options.columns = $.merge(options.columns || [], [
                    {
                        width: 86,
                        command: [{ text: "Sil", click: deleteRecord }]
                    }
                ]);


            }

            kendo.ui.Grid.fn.init.call(that, element, options);

            $(element).on("click", ".k-grid-add", { sender: that }, function() {
                window.location = $(element).attr('data-create-url');
            });

            $(element).on("change", ".k-grid-content input[type='checkbox']", function() {
                var row = $(this).closest("tr");
                var item = that.dataItem(row);
                var dataItem = that.dataSource.getByUid(item.uid);

                setTimeout(function(di, field, value) {
                    return function() {
                        di.set(field, value);
                    }
                }(dataItem, $(this).attr('name'), $(this).prop('checked')), 5);
            });
        },
        startLoading: function() {

        }
    });

    kendo.ui.plugin(extendedGrid);


})(jQuery);

function fixOperationUrl(url, id) {
    if (url.indexOf("?") !== -1) {
        var root = url.split('?')[0];
        var querystring = url.split('?')[1];

        return root + "/" + id + "?" + querystring;
    }

    return url + "/" + id;
}

kendo.pdf.defineFont({
    "DejaVu Sans": "http://cdn.kendostatic.com/2014.3.1314/styles/fonts/DejaVu/DejaVuSans.ttf",
    "DejaVu Sans|Bold": "http://cdn.kendostatic.com/2014.3.1314/styles/fonts/DejaVu/DejaVuSans-Bold.ttf",
    "DejaVu Sans|Bold|Italic": "http://cdn.kendostatic.com/2014.3.1314/styles/fonts/DejaVu/DejaVuSans-Oblique.ttf",
    "DejaVu Sans|Italic": "http://cdn.kendostatic.com/2014.3.1314/styles/fonts/DejaVu/DejaVuSans-Oblique.ttf"
});