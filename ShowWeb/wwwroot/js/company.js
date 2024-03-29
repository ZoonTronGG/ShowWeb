$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#tblData').DataTable({
        'ajax': { url: '/admin/company/getAll'},
        'columns': [
            {data: 'name', width: '15%'},
            {data: 'address', width: '15%'},
            {data: 'city', width: '15%'},
            {data: 'state', width: '15%'},
            {data: 'phoneNumber', width: '15%'},
            {
                data: 'id',
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary mx-2">
                    <i class="bi bi-pencil-square"></i> Edit</a>
                    <a onclick=Delete("/Admin/Company/Delete?id=${data}") class="btn btn-danger mx-2">
                    <i class="bi bi-trash"></i> Delete</a>
                    </div>`;
                },
                width: '20%'},
        ]
    });
}

function Delete(url){
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if(data.success){
                        Swal.fire(
                            'Deleted!',
                            data.message,
                            'success'
                        )
                        $('#tblData').DataTable().ajax.reload();
                    }
                    else{
                        Swal.fire(
                            'Error!',
                            data.message,
                            'error'
                        )
                    }
                }
            })
        }
    })
}