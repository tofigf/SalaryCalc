$(document).ready(function () {

    //checkbox checked button enabled
    $("#checkAll").click(function () {
        $('input:checkbox').not(this).prop('checked', this.checked);
    });
    //calculate
    $(".calcbutton").click(function () {
        var text = $(this).data('text');
        $("#Formula").val(function () {
            return this.value + text;
        });
    });
    //Role table
    $('.role-checkboxx').click(function () {
        if ($(this).hasClass("checked")) {
            $(this).removeClass('checked');
            $(this).next(".role-inputt").removeAttr('name');
        }
        else {

            $(this).addClass('checked');
            $(this).next(".role-inputt").attr('name', 'Roles[]');

        }
    }); 
    
   
    //date month and year
    $('.datepicker').datepicker({
        format: "mm-yyyy",
        viewMode: "months",
        minViewMode: "months"
    });
    //date
    $('.datepickerday').datepicker({
        format: 'dd/mm/yyyy'
       
    });
    //partial view
    function SetData(data) {
        $("#divPartialView").html(data); 
    }
    var currMonth = 0;
    var currYear = 0;
    //datepicker event
    $('.datepicker').datepicker().on('changeDate', function (e) {

         currMonth = new Date(e.date).getMonth() + 1;
         currYear = +String(e.date).split(" ")[3];
        //=============================================//
        $.ajax({
            url: "/Calculation/UsersForCalc/?currMonth=" + currMonth +"&currYear="+ currYear,
            cache: false,
            type: "get",
            dataType: "html",
            success: function (data) {
                SetData(data);
            }
        });
        
    });
    //Pagenate a click
    $(document).on("click", ".pagclick", function (e) {

        var page = $(this).data('page');
        $.ajax({
            url: "/Calculation/UsersForCalc/?currMonth=" + currMonth + "&currYear=" + currYear + "&page=" + page,
            cache: false,
            type: "get",
            dataType: "html",
            success: function (data) {
                SetData(data);
          
            }
        });

    });
    $(".positionId").val($(".tabclick").data("id"));
    $(document).on("click", ".tabclick", function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        $(".positionId").val(id);
    });
   
});