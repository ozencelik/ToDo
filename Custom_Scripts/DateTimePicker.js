$(document).ready(function () {

    var today = new Date();
    console.log(today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate() + '  ' + today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds());
    $('.datetimepicker').datepicker({
        changeMonth: true,
        changeYear: true,
        yearRange: "2019:2030",
        minDate: new Date(today.getFullYear(), today.getMonth(), today.getDate()),
        showOn: "both",
        buttonText: "<i class='fa fa-calendar'></i>"
    });

});


