using E_CommerceAR.Controllers;

namespace UnitTest_E_Commerce_AR
{

    public class ValidEmailTest
    {
        [Fact]
        public void IsValidEmail_ValidEmail_ReturnsTrue ()
        {
            // Arrange
            var accountsController = new AccountsController();
            string validEmail = "test@HomeSewet.com";

            // Act
            bool isValid = accountsController.IsValidEmail(validEmail);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmail_ReturnsFalse_1 ()
        {
            // Arrange
            var accountsController = new AccountsController();
            string invalidEmail = "testHomeSewet.com";

            // Act
            bool isValid = accountsController.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmail_ReturnsFalse_2 ()
        {
            // Arrange
            var accountsController = new AccountsController();
            string invalidEmail = "test.HomeSewet.com";

            // Act
            bool isValid = accountsController.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmptyEmail_ReturnsFalse ()
        {
            // Arrange
            var accountsController = new AccountsController();
            string invalidEmail = "";

            // Act
            bool isValid = accountsController.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidNullEmail_ReturnsFalse ()
        {
            // Arrange
            var accountsController = new AccountsController();
            string invalidEmail = null;

            // Act
            bool isValid = accountsController.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmail_ReturnsFalse_3 ()
        {
            // Arrange
            var accountsController = new AccountsController();
            string invalidEmail = "test@com";

            // Act
            bool isValid = accountsController.IsValidEmail(invalidEmail);

            // Assert
            Assert.False(isValid);
        }
    }
}
