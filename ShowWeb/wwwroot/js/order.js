$(document).ready(function () {
    const url = window.location.search
    const states = ['inprocess', 'completed', 'pending', 'approved', 'all'];
    let stateFound = false;
    
    for (const state of states) {
        if (url.includes(state)) {
            loadDataTable(state);
            stateFound = true;
            break;
        }
    }

    if (!stateFound) {
        loadDataTable('all');
    }
});

function loadDataTable(status) {
    $('#tblData').DataTable({
        'ajax': { url: '/admin/order/GetAll?status=' + status},
        'columns': [
            {data: 'id', width: '5%'},
            {data: 'name', width: '25%'},
            {data: 'phoneNumber', width: '20%'},
            {data: 'applicationUser.email', width: '20%'},
            {data: 'orderStatus', width: '10%'},
            {data: 'orderTotal', width: '10%'},
            {
                data: 'id', 
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a href="/Admin/Order/Details?id=${data}" class="btn btn-primary mx-2">
                    <i class="bi bi-pencil-square"></i></a>
                    </div>`;
                },
                width: '10%'},
        ]
    });
}