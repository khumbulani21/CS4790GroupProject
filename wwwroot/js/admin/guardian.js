var dataTable;

$(document).ready(function () {
    $(".toggleChildren").click(function () {
        $(this).siblings(".guardianChildren").slideToggle();
    });
    loadList();
});

function loadList() {
    dataTable = $('#Guardians_DT').DataTable({
        "ajax": {
            "url": "/api/guardians",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "fullName", width: "40%" },
            {
                data: "familyGroups", width: "40%",
                "render": function (data) {
                    return `<div class="text-center">${buildChildDisplay(data)}</div>`
                }
            },
            {
                data: "guardianID", width: "20%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/Guardians/View/${data}"
                            class ="btn btn-success text-white" style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i>View Info</a>                            
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

function buildChildDisplay(data) {
    var display = "";
    for (let i = 0; i < data.length; i++) {
        display += `<div><a href='/Admin/Child/View/${data[i].child.childID}'>${data[i].child.fullName}</a></div>`
    }
    return display;
}