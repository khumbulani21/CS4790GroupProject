$(document).ready(function () {
    $(".formRadio").click(function () {
        var changedTo = $(this).data('type');
        var parent = $(this).closest('.formRow');
        console.log(parent);
        if (changedTo === "Yes") {
            $(parent).find(".formText").show().attr('required', true);
            
        }
        else {
            $(parent).find(".formText").hide();
            $(parent).find(".formValue").val('No');
        }
    });

    $(".formText").change(function () {
        var parent = $(this).closest('.formRow');
        $(parent).find(".formValue").val($(this).val());
    });

});