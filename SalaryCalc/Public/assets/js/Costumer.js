$(document).ready(function () {


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

    $(".positionId").val($(".tabclick").data("id"));
    $(document).on("click", ".tabclick", function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        $(".positionId").val(id);
});
    $('#myForm').submit(function (e) {
        e.preventDefault();
     var url = $(this).data('url');

        var RoleIdsDto = {
            Ids: [],
            PostionId: $(".positionId").val()
        };
        console.log(RoleIdsDto);
        $("#customtable .checkedArray:checked").each(function (e) {
            RoleIdsDto.Ids.push($(this).data("id"));
        });
    $.ajax({
        url: url,
        type: 'Post',
        data: { RoleIdsDto: RoleIdsDto },
       
        dataType: "json",
        success: function (response) {
            if (response.status === 200 && response.isRedirect) {
                window.location.href = response.redirectUrl;
            }
        }
    });
});

});