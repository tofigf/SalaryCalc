$(document).ready(function () {
    //validate inputs
    $.validate({
        modules: ' date, security, file'
        
    });
    
    //checkbox checked button enabled
    $(document).on("click", "#checkAll", function (e) {
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
    //Paginate a click
    $(document).on("click", ".pagclick", function (e) {
        e.preventDefault();
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
    //take all pages data  on click(pagination)
    $(document).on("click", "#all" , function (e) {
        e.preventDefault();
        if (!$('#allpages').val().length) {

            $("#allpages").val("true");
            $('.btn-default').removeClass('btn-default');
            $(this).addClass('btn-primary');
            $(this).text("Hamısı Seçildi (bütün səhifələr)");
    } 
    else {
            $("#allpages").val("");
            $('.btn-primary').removeClass('btn-primary');
            $(this).addClass('btn-default');
            $(this).text("Hamısını Seçin (bütün səhifələr)");

        }
     
       
      
       
    });
    //Bootstrap Tooltip
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
    $('input:checkbox').change(function () {
        if ($(this).is(":checked")) {
            $('.active_for_confirm').removeClass("disabled");
        } else {
            $('.active_for_confirm').addClass("disabled");
        }
    });
    //Confirm :checked ids
    $('#confirm').click(function (e) {
        var url = $(this).data('url');
        console.log(url);
        var ids = [];
        var flag = false;
        $("#confirmTable tbody tr .confirmchecked:checked").each(function (e) {
            ids.push($(this).data("id"));
        });
        $.ajax({
            url: url,
            type: 'Post',
            data: { ids: ids },
            dataType: "json",
            success: function (response) {

                if (response.status === 200 && response.isRedirect)
                {
                    if (!flag) {
                        
                        flag = true;
                        toastr.success("Təsdiq Olundu");
                    }
                   
                    window.location.href = response.redirectUrl;
                } else if (response.status === 409)
                {
                    if (!flag) {
                        flag = true;
                        toastr.error("Satış Seçilməyib");
                    }
                }
            },
            error: function ()
            {
                toastr.warning("Bir xəta yarandı");
            }
        });
    });
  
});