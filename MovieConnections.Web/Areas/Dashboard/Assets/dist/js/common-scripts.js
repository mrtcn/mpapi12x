var app = {};

(function ($) {
    app.GetStatusValues = function () {
        return [
            {
                value: "Active",
                text: "Aktif"
            }, {
                value: "Inactive",
                text: "Pasif"
            }
        ];
    };

    app.GetMovieTypeValues = function () {
        return [
            {
                value: "Movie",
                text: "Film"
            }, {
                value: "Series",
                text: "Dizi"
            }, {
                value: "MiniSeries",
                text: "Mini Dizi"
            }
        ];
    };
}(jQuery));

$(document).ready(function () {
    
    $(":checkbox").each(function () {
        //if ($(this).attr("id") === $("#checkbox-label").text()) {
        //    var previousLabelText = $("#checkbox-label").closest("div").prev().text();
        //    $("#checkbox-label").text(previousLabelText);
        //}

        //var self = $(this),
        //  label = $("#checkbox-label"),
        //  label_text = label.text();

        //label.remove();
        $(this).iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue',
            //insert: '<div class="icheck_line-icon"></div>' + label_text
            increaseArea: '20%'
        });
    });

    $.fn.makeAllChecked = function (attribute)
    {
        $(this).on("change", function () {
            var $this = $(this);
            var targetClass = $this.attr(attribute);
            var $checkboxes = $(targetClass);
            var value = $this.prop('checked');
            if ($checkboxes.length) {
                $checkboxes.each(function (i, e) {
                    $(e).prop('checked', value).change();
                });                
            }
        });
        
    };

    $("#Language").kendoDropDownList({
        change: function () {
            window.location = this.value();
        }
    }).data("kendoDropDownList");

    $("#RelatedEntityId").kendoDropDownList({
        filter: "contains",
        change: function () {
            if (this.value()) {
                var url = window.location.pathname.split("?");
                window.location = url[0] + "?" + "relatedEntityId=" + this.value();
            }
        }
        
    }).data("kendoDropDownList");

});