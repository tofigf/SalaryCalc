$(document).ready(function () {
    //validate inputs
    $.validate({
        modules: ' date, security, file'
        
    });
    //Datatable
    $('#datatable').DataTable({
        "responsive": true,
        "paging": true,
        "ordering": true,
        "lenghmenu": [20],
        "pageLength": 10,
        dom: '<"dt-buttons"Bf><"clear">lirtp',

        buttons: [
            'copy', 'excel', 'pdf', 'print'
        ],

        language: {
            'paginate': {
                'previous': '<span class="prev-icon"></span>',
                'next': '<span class="next-icon"></span>'
            }
        },
        oLanguage: {
            "oPaginate": {
                "sFirst": "", // This is the link to the first page
                "sPrevious": " Əvvəlki ", // This is the link to the previous page
                "sNext": " Növbəti ", // This is the link to the next page
                "sLast": "" // This is the link to the last page
            },
            "sLengthMenu": "Hər səhifədə _MENU_  nəticə",
            "sZeroRecords": "Heçbir nəticə yoxdur",
            "sInfo": " _START_ dan _END_ of _TOTAL_ qədər nəticə",
            "sSearch": "Axtar:",
            "sInfoFiltered": "(filtered from _MAX_ total records)"

        }

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
    //Pagenate a click
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
  
});