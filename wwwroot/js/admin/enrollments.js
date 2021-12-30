//to do
//delete enrollment
//edit semester, program and hour block

var dataTable;

$(document).ready(function () {
    loadList();
    //places column search below the table header
    $('#DT_load tfoot tr').insertAfter($('#DT_load thead tr'));
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        //function for column filtering at the bottom of the data table
        //https://datatables.net/examples/api/multi_filter_select.html
        initComplete: function () {

            this.api().columns().every(function () {
                var column = this;
                var select = $('<select><option value=""></option></select>')
                    .appendTo($(column.footer()).empty())
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );

                        column
                            .search(val ? '^' + val + '$' : '', true, false)
                            .draw();
                    });

                column.data().unique().sort().each(function (d, j) {
                    select.append('<option value="' + d + '">' + d + '</option>')
                });
            });
            //hide Delete column filter
            document.getElementById("delete").innerHTML = "";
            document.getElementById("view").innerHTML = "";
        },
        "ajax": {
            "url": "/api/enrollments",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "childFirst", width: "10%" },
            { data: "childLast", width: "10%" },
            { data: "childGender", width: "5%" },
            { data: "childStatus", width: "5%" },
            { data: "programName", width: "20%" },
            { data: "hourBlock", width: "15%" },
            { data: "semesterName", width: "15%" },
            {
                data: "appIdEnrollID", width: "10%",
                "render": function (data) {
                    return `<div class="text-center">
                                 <a  href="/Admin/Enrollments/Upsert?applicationID=${data[0]}&&enrollmentID=${data[1]}"
                                 class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="bi bi-info-circle"></i>Update</a>
                            </div>`;
                }
            },
            {
                data: "enrollmentID", width: "10%",
                "render": function (data) {
                    return `<div class="text-center">
                                <a onClick=Delete('/api/enrollments/${data}')
                                class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="fa fa-trash-o"></i>Delete</a>
                            </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
}

function Delete(url) {

    swal({
        title: "Are you sure want to delete ?",
        text: "Once deleted, you will not be able to recover this  data!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }

            })
        }
    });
}

function ValidateInput() {

    //this one uses jquery to check if there is no id
    if ($('#ddlProgram option:selected').val() == 0) {
        swal('Error', 'Please select program', 'error')
        return false;
    }
    //this one uses jquery to check if there is no id
    if ($('#ddlSemester option:selected').val() == 0) {
        swal('Error', 'Please select semester', 'error')
        return false;
    }
    //this one uses jquery to check if there is no id
    if ($('#ddlHourBlock option:selected').val() == 0) {
        swal('Error', 'Please select Hours', 'error')
        return false;
    }

    return true;
}
//function ValidateUpdate() {
//    if (ValidateInput()) {
//        let currentSemester = document.getElementById("semesterID").value;
//        let programID = document.getElementById("programID").value;
//        let hourBlock = document.getElementById("hourBlock").value;
//        if ($('#ddlSemester option:selected').val() != currentSemester || $('#ddlProgram option:selected').val() != programID || $('#ddlHourBlock option:selected').val() != hourBlock) {

//            return true;
//        }
//        else {
//            swal('Error', 'Please make a change', 'error')
//            return false;
//        }

//    }
//    else {
//        return false;
//    }

//}
function ValidateEnrollment() {
    let progID = document.getElementById("ddlProgram").value;
    let semID = document.getElementById("semID").value;
   
    let hourBlock = document.getElementById("ddlHourBlock").value;
    let semesterID = document.getElementById("ddlSemester").value;
    let applicationID = document.getElementById("applicationID").value;
    if (applicationID==0) {
        applicationID =  document.getElementById("ddlApplication").value;
    }
    if (semesterID != 0 && progID != 0 && hourBlock != 0 && applicationID!=0) {
       

        $.ajax({
            type: 'GET',
            url: `/api/enrollments/${applicationID}/${semesterID}`,
            dataType: 'json',
            success: function (data) {
                if (data.data == 0) {
                    document.getElementById("btnCreate").disabled = false;
                }
                else {

                    document.getElementById("btnCreate").disabled = true;

                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal('Error', jqXHR.status, 'error');
            }
        });
    }
    else {

        document.getElementById("btnCreate").disabled = true;

    }
}
function ValidateUpdate() {
    let progID = document.getElementById("ddlProgram").value;
    let programID = document.getElementById("programID").value;
    
    let semesterID = document.getElementById("semesterID").value;
    let hBlock = document.getElementById("hourBlock").value;
    let hourBlock = document.getElementById("ddlHourBlock").value;
    let newSemester = document.getElementById("ddlSemester").value;
   
    let enrollmentID = document.getElementById("enrollmentID").value;
    if (newSemester != 0 && progID != 0 && hourBlock != 0) {
        if (programID == progID && hourBlock == hBlock && semesterID == newSemester) {
            document.getElementById("btnUpdate").disabled = true;
        }
        else {
            $.ajax({
                type: 'GET',
                url: `/api/enrollments/${enrollmentID}/${semesterID}/${newSemester}`,
                dataType: 'json',
                success: function (data) {
                    if (data.data == 0) {
                        document.getElementById("btnUpdate").disabled = false;
                    }
                    else {

                        document.getElementById("btnUpdate").disabled = true;

                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    swal('Error', jqXHR.status, 'error');
                }
            });
        }

       
    }
    else {

        document.getElementById("btnUpdate").disabled = true;

    }
}

