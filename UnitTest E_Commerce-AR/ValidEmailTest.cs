using E_CommerceAR.Controllers;

namespace UnitTest_E_Commerce_AR
{

    public class ValidEmailTest
    {
        [Fact]
        public void IsValidEmail_ValidEmail_ReturnsTrue ()
        {
            var accountsController = new AccountsController();
            string validEmail = "test@HomeSewet.com";

            bool isValid = accountsController.IsValidEmail(validEmail);

            Assert.True(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmail_ReturnsFalse_1 ()
        {

            var accountsController = new AccountsController();
            string invalidEmail = "testHomeSewet.com";


            bool isValid = accountsController.IsValidEmail(invalidEmail);


            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmail_ReturnsFalse_2 ()
        {

            var accountsController = new AccountsController();
            string invalidEmail = "test.HomeSewet.com";


            bool isValid = accountsController.IsValidEmail(invalidEmail);


            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmptyEmail_ReturnsFalse ()
        {

            var accountsController = new AccountsController();
            string invalidEmail = "";


            bool isValid = accountsController.IsValidEmail(invalidEmail);


            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidNullEmail_ReturnsFalse ()
        {

            var accountsController = new AccountsController();
            string invalidEmail = null;


            bool isValid = accountsController.IsValidEmail(invalidEmail);


            Assert.False(isValid);
        }

        [Fact]
        public void IsValidEmail_InvalidEmail_ReturnsFalse_3 ()
        {

            var accountsController = new AccountsController();
            string invalidEmail = "test@com";


            bool isValid = accountsController.IsValidEmail(invalidEmail);


            Assert.False(isValid);
        }
    }
}
