$(document).ready(function () {
    initializeDataTable('#userTable');

    bindActionButtons();

    const successMessage = $('#tempSuccessMessage').val();
    const errorMessage = $('#tempErrorMessage').val();

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

    $('#userTable').on('draw.dt', function () {
        bindActionButtons();
    });
});

function bindActionButtons() {
    $("#userTable").on("click", ".deleteUserbtn", function (e) {
        e.preventDefault();
        const userId = $(this).data("action-id");

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
                $(`#deleteForm-${userId}`).submit();
            }
        });
    });

    $("#userTable").on("click", ".detailsUserBtn", function () {
        const alias = $(this).data('alias');
        const knownAs = $(this).data('knownas');
        const email = $(this).data('email');
        const phone = $(this).data('phone');
        const dob = $(this).data('dob');
        const insurance = $(this).data('insurance');
        const address = $(this).data('address');
        const maritalStatus = $(this).data('marstat');
        const gender = $(this).data('gender');
        const emergContact = $(this).data('emergcontact');
        const favoriteLanguage = $(this).data('favlanguage');
        const lastLogin = $(this).data('lastlogin');
        const role = $(this).data('role');
        const userType = $(this).data('usertype');
        const createdate = $(this).data('dateofcreation');

        $('#detailAlias').text(alias);
        $('#detailKnownAs').text(knownAs);
        $('#detailEmail').text(email);
        $('#detailPhone').text(phone);
        $('#detailDOB').text(dob);
        $('#detailInsurance').text(insurance);
        $('#detailAddress').text(address);
        $('#detailMaritalStatus').text(maritalStatus);
        $('#detailGender').text(gender);
        $('#detailEmergencyContact').text(emergContact);
        $('#detailFavoriteLanguage').text(favoriteLanguage);
        $('#detailLastLogin').text(lastLogin ? new Date(lastLogin).toLocaleDateString() : 'Never');
        $('#detailRole').text(role);
        $('#detailUserType').text(userType);
        $('#detailCreatedat').text(createdate);
    });
}
