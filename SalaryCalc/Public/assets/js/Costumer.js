﻿$(document).ready(function () {

    // Add custom validation rule
    // didnt use yet
    $.formUtils.addValidator({
        name: 'regextest',
        validatorFunction: function (value) {
            var test = RegExp('^[a-zA-Z0-9]+$');
            return test(value);
        },
        errorMessage: 'You have to answer with an even number',
        errorMessageKey: 'badEvenNumber'
    });

    //validate inputs
    $.validate({
        modules: ' date, security, file'
        
    });
    //checkbox checked button enabled
    $(document).on("click", "#checkAll", function (e) {
        $('input:checkbox').not(this).prop('checked', this.checked);
      
            $.ajax({
                url: "/Calculation/AllChecked/?allChecked=" + true,
                cache: false,
                type: "get",
                dataType: "json",
                success: function () {
                }
            });
        if (!$('#checkAll').is(':checked'))  {
            $.ajax({
                url: "/Calculation/AllChecked/?allChecked=" + false,
                cache: false,
                type: "get",
                dataType: "json",
                success: function () {
                }
            });
        }
        if (!$('#allpages').val().length) {

            $("#allpages").val("true");
        }
        else {
            $("#allpages").val("");

        }
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
    $("#DateYear").datepicker({
        format: "yyyy",
        viewMode: "years",
        minViewMode: "years"
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
        //=====================================
        //Checkbox:Checked
        //=====================================
        if ($('#checkAll').is(':checked')) {

            $.ajax({
                url: "/Calculation/AllChecked/?allChecked=" + true,
                cache: false,
                type: "get",
                dataType: "json",
                success: function () {
                }
            });
        }
        //else {
        //    $.ajax({
        //        url: "/Calculation/AllChecked/?allChecked=" + false,
        //        cache: false,
        //        type: "get",
        //        dataType: "json",
        //        success: function () {
        //        }
        //    });
        //}
    });
    //take all pages data  on click(pagination) didnt use
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
    //Calc Add User
    $(".calcbutton").click(function () {
        var text = $(this).data('text');
        $("#Formula").val(function () {
            return this.value + text;
        });
    });
    $("select[name='PinCod']").change(function (e) {
        //{ [123456B & ayliq] }* { [123456C & illik] } / 2
        if ($("select[name='CalcMethod'] option[value='ayliqgelir']").length === 0 && $("select[name='CalcMethod'] option[value='illikgelir']").length === 0 ) {

            $("select[name='CalcMethod']").append(`
              <option value="ayliq">Aylıq</option>
              <option value="illik">İllik</option>
                       `);
        }
         $("input[name ='HiddenPinCod']").val($(this).val());
    });
    $("select[name='CalcMethod']").change(function (e) {
         $("input[name ='HiddenCalcMethod']").val($(this).val());

        $("select[name='CalcMethod'] option").each(function () {
           
            if ($(this).val() !== '') {
                $(this).remove();
            }
        });
    });
    $("#calcAddUser").click(function () {

        if ($("input[name ='HiddenPinCod']").val() !== '' && $("input[name ='HiddenCalcMethod']").val() !== '') {
            $("#Formula").val(function () {
                return this.value + "{[" + $("input[name ='HiddenPinCod']").val() + "&" + $("input[name ='HiddenCalcMethod']").val() + "]}";
            });
        }
  

    });
    $(".allow_decimal").on("input", function (evt) {
        var self = $(this);
        self.val(self.val().replace(/[^0-9\.]/g, ''));
        if ((evt.which !== 46 || self.val().indexOf('.') !== -1) && (evt.which < 48 || evt.which > 57)) {
            evt.preventDefault();
        }
    });
    $("#NumberAdd").click(function () {
        thisValue = $("#startNumber").val();
        selectNumberdate = $("select[name ='NumberAddMonthOrYear']").val();
        //var lastChar = thisValue.substr(thisValue.length - 1);
        //if (lastChar.indexOf('%') > -1) {

        //}

        if (thisValue !== '' && selectNumberdate !== '') {
            $("#Formula").val(function () {
                return this.value + "$" + thisValue + "," + selectNumberdate + "$";
            });
        }
        $("#startNumber").val("");
        $(this).prop('disabled', true);

    });
    $('#Formula').on("change paste keyup", function ()  {

        if ($(this).val().indexOf('$') !== -1) {

            $("#NumberAdd").prop('disabled', false);
        }
      
    });
});