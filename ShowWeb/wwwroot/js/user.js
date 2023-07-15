$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#tblData').DataTable({
        'ajax': {url: '/admin/user/getAll'},
        'columns': [
            {data: 'name', width: '15%'},
            {data: 'email', width: '15%'},
            {data: 'phoneNumber', width: '15%'},
            {data: 'company.name', width: '15%'},
            {data: 'role', width: '15%'},
            {
                data: {id: 'id', lockoutEnd: 'lockoutEnd'},
                render: function (data) {
                    const today = new Date().getTime();
                    const lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `
                            <div class="text-center">
                                <a class="btn btn-warning text-white" style="cursor: pointer; width: 100px;" 
                                onclick=LockUnlock('${data.id}')>
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </a>
                                <a href="/Admin/User/RoleManagement?id=${data.id}" class="btn btn-success" style="cursor: pointer; width: 150px;">
                                    <i class="fas fa-edit"></i> Permission
                                </a>
                            </div>
                        `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a class="btn btn-danger text-white" style="cursor: pointer; width: 100px;"
                                onclick=LockUnlock('${data.id}')>
                                    <i class="bi bi-lock-fill"></i> Lock
                                </a>
                                <a href="/Admin/User/RoleManagement?id=${data.id}" class="btn btn-success" style="cursor: pointer; width: 150px;">
                                    <i class="fas fa-edit"></i> Permission
                                </a>
                            </div>
                        `;
                    }
                },
                width: '25%'
            },
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: 'POST',
        url: '/admin/user/LockUnlock',
        data: JSON.stringify(id),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                $('#message').html(data.message);
                $('#message').show();
                
                $('#tblData').DataTable().ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });
}