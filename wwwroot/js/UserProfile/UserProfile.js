$(document).ready(function () {
    const isWeakPassword = $('#isWeakPassword').val() === 'true';

    $('#changePasswordModal').modal({
        backdrop: isWeakPassword ? 'static' : true,
        keyboard: !isWeakPassword
    });

    $('#changePasswordModal').modal('show');

    if (isWeakPassword) {
        $('#closeModalButton').remove();

        Swal.fire({
            icon: 'warning',
            title: 'Password Change Required',
            text: 'Your current password is too weak. Please update it to a stronger one.',
            confirmButtonText: 'OK'
        });
    }

    $('#toggleOldPassword').on('click', function () {
        const oldPasswordInput = $('#OldPassword');
        const icon = $(this).find('i');
        if (oldPasswordInput.attr('type') === 'password') {
            oldPasswordInput.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            oldPasswordInput.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    $('#toggleNewPassword').on('click', function () {
        const newPasswordInput = $('#NewPassword');
        const icon = $(this).find('i');
        if (newPasswordInput.attr('type') === 'password') {
            newPasswordInput.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            newPasswordInput.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    $('#toggleConfirmPassword').on('click', function () {
        const confirmPasswordInput = $('#ConfirmPassword');
        const icon = $(this).find('i');
        if (confirmPasswordInput.attr('type') === 'password') {
            confirmPasswordInput.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            confirmPasswordInput.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    function generatePassword() {
        const passwordLength = 12;
        const upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const numbers = "0123456789";
        const specialChars = "!-$#%^&*()_+={ }[}:;,.<>?\u0040";

        let password = "";
        const allChars = upperCaseChars + lowerCaseChars + numbers + specialChars;

        password += upperCaseChars.charAt(Math.floor(Math.random() * upperCaseChars.length));
        password += numbers.charAt(Math.floor(Math.random() * numbers.length));
        password += specialChars.charAt(Math.floor(Math.random() * specialChars.length));

        for (let i = password.length; i < passwordLength; i++) {
            password += allChars.charAt(Math.floor(Math.random() * allChars.length));
        }

        password = password.split('').sort(() => 0.5 - Math.random()).join('');
        $('#SuggestedPassword').val(password);
    }

    $('#generatePassword').on('click', function () {
        generatePassword();
        const suggestedPassword = $('#SuggestedPassword').val();
        $('#NewPassword').val(suggestedPassword);
        $('#ConfirmPassword').val(suggestedPassword);
    });

    $('#regeneratePassword').on('click', function () {
        generatePassword();
    });

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
});
