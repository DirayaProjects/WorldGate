
$(document).ready(function () {
    initializeDataTable('#RolesTable');

    bindDeleteButtons();

    $('#RolesTable').on('draw.dt', function () {
        bindDeleteButtons();
    });
    $('.editRoleBtn').on('click', function () {
        let roleId = $(this).data('id');
        let roleAlias = $(this).data('alias');
        let roleInfo = $(this).data('info');

        $('#roleId').val(roleId);
        $('#editAlias').val(roleAlias);
        $('#editInfo').val(roleInfo);
    });
    let successMessage = $('#tempSuccessMessage').val();
    let errorMessage = $('#tempErrorMessage').val();

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: successMessage
        });
    }

    if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage
        });
    }
});

function bindDeleteButtons() {
    $('.deleteRoleBtn').on('click', function (e) {
        e.preventDefault();
        let form = $(this).closest('form');

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
                form.submit();
            }
        });
    });
}
