var jq = jQuery.noConflict();
jq(document).ready(function () {
    $("#accountNumberInput").on('input', function () {
        var accountNumber = $(this).val();

        console.log("Input changed, current value:", accountNumber);

        if (accountNumber.length = 7) {
            console.log("Making request for account number:", accountNumber);

            $.get("/Transactions/GetUserNameByAccountNumber", { accountNumber: accountNumber }, function (data) {
                console.log("Received response from server:", data);

                if (data.success) {
                    console.log("Setting userNameInput value to:", data.userName, data.accountNumber);
                    $("#userNameInput").val(data.userName);
                    $("#userIdInput").val(data.userId);
                    $("#accountIdInput").val(data.accountId);
                } else {
                    console.log("User not found for account number:", accountNumber);
                    $("#userNameInput").val("User not found");
                    // clear the hidden inputs
                    $("#userIdInput").val("");
                    $("#accountIdInput").val("");
                }
            });
        } else {
            console.log("Input length not yet at desired length.");
        }
    });
});
