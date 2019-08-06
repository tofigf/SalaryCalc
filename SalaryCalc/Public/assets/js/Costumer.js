//$(document).ready(function () {
//    $(".editposition").click(function () {

//        $('#myForm').attr('action', 'Positions/EditPosition');
//        var id = $(this).data("id");
//        $.ajax({
//            url: "/positions/edit/" + id,
//            type: "GET",
//            dataType: "json",
//            success: function (response) {
              
//                if (response.status === 200) {

//                    //$("#Id").val(response.Id);
//                    //$("#Name").val(response.Name);
//                }
             
//            },
//            error: function (errormessage) {
//                alert(errormessage.responseText);
//            }
//        });
//    });

//});