using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLESS.Core.Entities;
using QLESS.Core.Strategies.FareStrategies;

namespace QLESS.Core.UnitTests.Strategies.FareStrategies
{
    [TestClass]
    public class BaseFareStrategyUnitTests
    {
        // Fields
        private BaseFareStrategy sut;
        
        // Initialization
        [TestInitialize]
        public void TestInitialize()
        {
            sut = new BaseFareStrategy();
        }

        // Test Methods
        [TestMethod]
        [ExpectedException(typeof(FareStrategyException))]
        public void GetFare_WithNullCard_ShouldThrowFareStrategyException()
        {
            // Act
            sut.GetFare(null, 1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(FareStrategyException))]
        public void GetFare_WithNullCardType_ShouldThrowFareStrategyException()
        {
            // Arrange
            var card = new Card() { };

            // Act
            sut.GetFare(card, 1, 2);
        }

        [TestMethod]
        public void GetFare_WithValidCard_ShouldReturnFareEqualToBaseFare()
        {
            // Arrange
            var card = new Card()
            {
                Type = new CardType()
                {
                    BaseFare = 20M
                }
            };

            // Act
            var value = sut.GetFare(card, 1, 2);

            // Assert
            value.Should().Be(20M);
        }
    }
}
