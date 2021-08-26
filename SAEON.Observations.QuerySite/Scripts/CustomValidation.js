(function ($) {
    $.validator.addMethod("isbeforedate", function (value, element, params) {
        if (!this.optional(element)) {
            var endDate = $("#" + params).val();
            console.log("isbeforedate: " + value + " " + endDate + " " + (new Date(value) <= new Date(endDate)));
            if (!/Invalid|NaN/.test(new Date(value)) && !/Invalid|NaN/.test(new Date(endDate))) {
                return new Date(value) <= new Date(endDate);
            }
        }
        return true;
    });
    $.validator.unobtrusive.adapters.addSingleVal("isbeforedate", "enddate");

    $.validator.addMethod("isafterdate", function (value, element, params) {
        if (!this.optional(element)) {
            var startDate = $("#" + params).val();
            console.log("isafterdate: " + value + " " + startDate + " " + (new Date(value) >= new Date(startDate)));
            if (!/Invalid|NaN/.test(new Date(value)) && !/Invalid|NaN/.test(new Date(startDate))) {
                return new Date(value) >= new Date(startDate);
            }
        }
        return true;
    });
    $.validator.unobtrusive.adapters.addSingleVal("isafterdate", "startdate");
}(jQuery));