
$(document).ready(function () {
    initializeDataTable('#PermissionTable');

    bindDeleteButtons();

    $('#PermissionTable').on('draw.dt', function () {
        bindDeleteButtons();
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
    $('.deletePermissionBtn').on('click', function (e) {
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