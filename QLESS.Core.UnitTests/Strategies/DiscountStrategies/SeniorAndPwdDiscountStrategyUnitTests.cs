using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLESS.Core.Entities;
using QLESS.Core.Strategies.DiscountStrategies;
using System;
using System.Linq;

namespace QLESS.Core.UnitTests.Strategies.DiscountStrategies
{
    [TestClass]
    public class SeniorAndPwdDiscountStrategyUnitTests
    {
        // Fields
        private SeniorAndPwdDiscountStrategy sut;

        // Initialization
        [TestInitialize]
        public void TestInitialize()
        {
            sut = new SeniorAndPwdDiscountStrategy();
        }

        // Test Methods
        [TestMethod]
        [ExpectedException(typeof(DiscountStrategyException))]
        public void GetDiscount_WithNullCard_ShouldThrowDiscountStrategyException()
        {
            // Act
            sut.GetPrecentageDiscount(null);
        }

        [TestMethod]
        public void GetDiscount_WithNoPreviousSameDayTrips_DiscountShouldBeTwentyPrecent()
        {
            // Arrange
            var card = new Card() 
            {
                Trips = (new[] 
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddDays(-5),
                        Exit = DateTime.Today.AddDays(-5),
                    },
                    new Trip()
                    {
                        Entry = DateTime.Today.AddDays(-3),
                        Exit = DateTime.Today.AddDays(-3),
                    }
                }).ToList()
            };

            // Act
            var value = sut.GetPrecentageDiscount(card);

            // Assert
            value.Should().Be(0.2M);
        }

        [TestMethod]
        public void GetDiscount_WithOnePreviousSameDayTrip_DiscountShouldBeTwentyThreePercent()
        {
            // Arrange
            var card = new Card()
            {
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Now,
                        Exit = DateTime.Today.AddMinutes(30),
                    }
                }).ToList()
            };

            // Act
            var value = sut.GetPrecentageDiscount(card);

            // Assert
            value.Should().Be(0.23M);
        }

        [TestMethod]
        public void GetDiscount_WithThreePreviousSameDayTrip_DiscountShouldBeTwentyThreePercent()
        {
            // Arrange
            var card = new Card()
            {
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Now,
                        Exit = DateTime.Today.AddMinutes(30),
                    },
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        Exit = DateTime.Today.AddHours(1.5),
                    },
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(2),
                        Exit = DateTime.Today.AddHours(2.5),
                    }
                }).ToList()
            };

            // Act
            var value = sut.GetPrecentageDiscount(card);

            // Assert
            value.Should().Be(0.23M);
        }

        [TestMethod]
        public void GetDiscount_WithFourPreviousSameDayTrip_DiscountShouldBeTwentyPercent()
        {
            // Arrange
            var card = new Card()
            {
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Now,
                        Exit = DateTime.Today.AddMinutes(30),
                    },
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        Exit = DateTime.Today.AddHours(1.5),
                    },
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(2),
                        Exit = DateTime.Today.AddHours(2.5),
                    },
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(3),
                        Exit = DateTime.Today.AddHours(3.5),
                    }
                }).ToList()
            };

            // Act
            var value = sut.GetPrecentageDiscount(card);

            // Assert
            value.Should().Be(0.20M);
        }
    }
}
