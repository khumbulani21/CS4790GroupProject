var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/employees",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "firstName", width: "20%" },
            { data: "lastName", width: "20%" },
            { data: "userName", width: "20%" },
            {
                data: "role", width: "10%",
                "render": function (data) {
                    if (data[1] == "Admin") {

                        return `<div class="text-center">
                                   <a onClick=PostAdminEmployee('/api/employees/postAdminEmployee/${data[0]}')
                                   class ="btn btn-outline-warning  style="cursor:pointer; width=100px;"> <i class="far fa-lock-open"></i>Admin</a>
                                </div>`;

                    }
                    else
                        return `<div class="text-center">
                                    <a onClick=PostAdminEmployee('/api/employees/postAdminEmployee/${data[0]}')
                                    class ="btn btn-outline-warning  style="cursor:pointer; width=100px;"> <i class="far fa-lock-close"></i>Employee</a>
                                </div>`;
                }
            },
            {data: "locked", width: "10%",
                "render": function (data) {
                    if (data[1] == "False") {

                        return `<div class="text-center">
                                   <a onClick=Post('/api/employees/postLockUnlock/${data[0]}')
                                   class ="btn btn-outline-warning  style="cursor:pointer; width=100px;"> <i class="far fa-lock-open"></i>Lock</a>
                                </div>`;

                    }
                    else
                        return `<div class="text-center">
                                    <a onClick=Post('/api/employees/postLockUnlock/${data[0]}')
                                    class ="btn btn-outline-warning  style="cursor:pointer; width=100px;"> <i class="far fa-lock-close"></i>Unlock</a>
                                </div>`;
            }
            },
            {
                data: "id", width: "10%",
                "render": function (data) {
                    return `<div class="text-center">
                                <a onClick=Delete('/api/employees/${data}')
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

//submits request to delete an account
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
//submits request to lock or unlock account
function Post(url) {
    swal({
        title: "Are you sure want to lock/unlock this account ?",
        text: "You can always lock/unlock!",
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
//submits post request to make an employee an admin
function PostAdminEmployee(url) {
    swal({
        title: "Are you sure want to change the role of employee ?",
        text: "You can always change the role!",
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
