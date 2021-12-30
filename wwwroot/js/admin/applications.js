var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/applications",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            // { data: "aspNetUserId", width: "25%" },
            { data: "applicationId", width: "35%" },
            { data: "applicationStatus", width: "35%" },
            { data: "guardianName", width: "30%" },
            { data: "childName", width: "30%" },
            { data: "programName", width: "30%" }

        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
}

