namespace UserManagementApi.Services
{
    public static class ErrorMsg
    {

        public const string None = "";
        public const string InvalidProperties = "Some Properties are not valid";
        public const string InvalidUser = "User not found";
        public const string EmailNotConfirm = "Email address has not been confirmed";
        public const string NoUserEmail = "No user associated with email";
        public const string ResetPasswordSuccess = "Reset password URL has been sent to the email successfully";
        public const string InvalidPassword = "Invalid password";
        public const string NullModel = "Register Model is null";
        public const string UserNotCreated = "User did not create";
        public const string GeneralErrorMsg = "Something went wrong";
        public const string UserRoleNotFound = "This Role Doesnot Exist.";
        public const string InvalidLoginAttempt = "Invalid Login attempts..please try again";

    }

    public static class Msg
    {
        public const string ResetPasswordSuccess = "Password has been reset successfully";
        public const string EmailConfirm = "Email confirmed successfully!";
        public const string ConfirmPasswordNotMatch = "Confirm password doesn't match the password";
        public const string ConfirmEmailMsg = "Confirm your email";
        public const string EmailMsgBody1 = "Welcome to Kendrick Timer User auth Database";
        public const string EmailMsgBody2 = "Please confirm your email by ";
        public const string EmailMsgBody3 = "Clicking here";
        public const string UserCreated = "User created successfully";
        public const string ResetPassword = "Reset Password";
        public const string ResetPasswordMsg1 = "Follow the instructions to reset your password";
        public const string ResetPasswordMsg2 = "To reset your password";
        public const string ResetPasswordMsg3 = "Click Here.";

    }
}
