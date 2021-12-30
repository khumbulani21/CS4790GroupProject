var dataTable;

$(document).ready(function () {
    loadList();
    //places column search below the table header
    $('#DT_load tfoot tr').insertAfter($('#DT_load thead tr'));

    //status filter 
    $("#DT_load_filter.dataTables_filter").append($("#statusFilter"));
   
    //column to be filtered
    var statusIndex = 4;
    $("#filterTable th").each(function (i) {
        if ($($(this)).html() == "Status") {
            statusIndex = i; return false;
        }
    });
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            var selectedItem = $('#statusFilter').val()
            var status = data[statusIndex];
            if (selectedItem === "" || status.includes(selectedItem)) {
                return true;
            }
            return false;
        }
    );

    $("#statusFilter").change(function (e) {
        dataTable.draw();
    });
   
    dataTable.draw();

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
            //hide View column filter
            document.getElementById("view").innerHTML = "";
            document.getElementById("date").innerHTML = "";
        },
        "searching": true, "order": [[4, "desc"]],
        "ajax": {
            "url": `/api/applicationStatus/`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [

            { data: "applicationId", width: "10%" },
            { data: "childFirst", width: "15%" },
            { data: "childLast", width: "15%" },
            { data: "programName", width: "25%" },
            { data: "applicationStatus", width: "15%" },
            { data: "modifiedDate", width: "10%" },
            { data: "applicationId", width: "10%",
                    "render": function (data)
                    {
                                    return `<div class="text-center">
                                           <a  href="/Admin/Applications/ApplicationInfo?id=${data}"
                                                        class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="fa fa-trash-o"></i>View </a>
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
function Approve(url) {
    swal({
        title: "Are you sure want to approve the request ?",
        text: "Request will be approved!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willPost) => {
        if (willPost) {
            $.ajax({
                type: 'Post',
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

function Decline(url) {
    swal({
        title: "Are you sure want to decline the request ?",
        text: "Request will be deny!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willPost) => {
        if (willPost) {
            $.ajax({
                type: 'Post',
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

function getEnrollmentCount(semesterID, programID) {
    $.ajax({
        type: 'GET',
        url: '/api/ApplicationStatus/EnrollmentCount/${semesterID}/${programID}',
        dataType: "json",
        success: function (data) {
            var newHtml = `<button class="button color-12" onclick="processCheckIn(this, ${data})">Check In</button>`;

        },
        error: function (data) {
            alert("Processing the request failed, please try again.");

        }
    });
    location.reload();

}
