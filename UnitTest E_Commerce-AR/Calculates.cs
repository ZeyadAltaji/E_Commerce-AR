using E_CommerceAR.Areas.AdminDashboard.Controllers;

namespace UnitTest_E_Commerce_AR
{
    public class Calculates
    {

        [Fact]
        public void CalculateProfitRatio_WhenTotalRevenueIsZero_ShouldReturnZero ()
        {
            // Arrange
            decimal totalRevenue = 0;
            decimal totalCost = 100;
            var homeController = new HomeController();

            // Act
            decimal result = homeController.CalculateProfitRatio(totalRevenue , totalCost);

            // Assert
            Assert.Equal(0 , result);
        }

        [Fact]
        public void CalculateProfitRatio_WhenTotalRevenueGreaterThanZero_ShouldCalculateCorrectly ()
        {
            // Arrange
            decimal totalRevenue = 100;
            decimal totalCost = 50;
            var homeController = new HomeController();

            // Act
            decimal result = homeController.CalculateProfitRatio(totalRevenue , totalCost);

            // Assert
            Assert.Equal(50 , result);
        }

        [Fact]
        public void CalculateProfitRatio_WhenTotalCostIsZero_ShouldHandleCorrectly ()
        {
            // Arrange
            decimal totalRevenue = 100;
            decimal totalCost = 0;
            var homeController = new HomeController();

            // Act
            decimal result = homeController.CalculateProfitRatio(totalRevenue , totalCost);

            // Assert
            Assert.Equal(100 , result);
        }

        [Fact]
        public void CalculateProfitRatio_WhenBothTotalRevenueAndTotalCostAreZero_ShouldReturnZero ()
        {
            // Arrange
            decimal totalRevenue = 0;
            decimal totalCost = 0;
            var homeController = new HomeController();

            // Act
            decimal result = homeController.CalculateProfitRatio(totalRevenue , totalCost);

            // Assert
            Assert.Equal(0 , result);
        }

        [Fact]
        public void CalculateProfitRatio_WhenBothTotalRevenueAndTotalCostAreGreaterThanZero_ShouldCalculateCorrectly ()
        {
            // Arrange
            decimal totalRevenue = 200;
            decimal totalCost = 50;
            var homeController = new HomeController();

            // Act
            decimal result = homeController.CalculateProfitRatio(totalRevenue , totalCost);

            // Assert
            Assert.Equal(75 , result);
        }
    }

}