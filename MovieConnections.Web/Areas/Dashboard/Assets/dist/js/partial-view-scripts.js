﻿$(document).ready(function () {
    $("#IsAutoGenerated").on("ifChecked", function () {
        $("#Url").prop("readonly", true);
    });

    $("#IsAutoGenerated").on("ifUnchecked", function () {
        $("#Url").prop("readonly", false);
    });
});