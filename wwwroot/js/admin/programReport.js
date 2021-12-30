
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
            document.getElementById("programReport").innerHTML = "";
            document.getElementById("foodReport").innerHTML = "";
            
        },
        "ajax": {
            "url": "/api/programReport",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "programName", width: "30%" },
            { data: "semesterName", width: "30%" }
            ,
            {
                data: "enrollmentID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                                           <a  href="/Admin/Reports/ProgramReport?id=${data}"
                                                        class ="btn bg-color-5 text-white style="cursor:pointer; width=100px;"/> <i class="bi bi-info-circle"></i>program report</a>
                                            </div>`;
                }
            }
            ,
            {
                data: "enrollmentID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                                <a  href="#"
                                            class ="btn bg-color-2 text-white style="cursor:pointer; width=100px;"> <i class="bi bi-info-circle"></i>food report</a>
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
