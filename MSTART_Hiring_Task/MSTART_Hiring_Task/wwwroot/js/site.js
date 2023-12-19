// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showUpdateProfileForm() {
    document.getElementById('updateProfileForm').style.display = 'block';
    document.getElementById('changePasswordForm').style.display = 'none';
}

function showChangePasswordForm() {
    document.getElementById('updateProfileForm').style.display = 'none';
    document.getElementById('changePasswordForm').style.display = 'block';
}