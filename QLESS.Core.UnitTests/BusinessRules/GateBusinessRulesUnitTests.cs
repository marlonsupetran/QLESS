using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QLESS.Core.BusinessRules;
using QLESS.Core.Data;
using QLESS.Core.Entities;
using QLESS.Core.Strategies;
using QLESS.Core.Strategies.DiscountStrategies;
using QLESS.Core.Strategies.FareStrategies;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QLESS.Core.UnitTests.BusinessRules
{
    [TestClass]
    public class GateBusinessRulesUnitTests
    {
        // Fields
        private GateBusinessRules sut;
        private Mock<IRepository> mockRepository;
        private Mock<IStrategyFactory> mockStrategyFactory;

        // Initialization
        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new Mock<IRepository>();
            mockStrategyFactory = new Mock<IStrategyFactory>();

            sut = new GateBusinessRules(mockRepository.Object, mockStrategyFactory.Object);
        }

        // Test Methods
        #region Enter

        [TestMethod]
        [ExpectedException(typeof(CardNotFoundException))]
        public void Enter_WithEmptyCardId_ShouldThrowCardNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.Empty;

            // Act
            sut.Enter(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(CardNotFoundException))]
        public void Enter_WithNonExistingCardId_ShouldThrowCardNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();

            mockRepository
                .Setup(m => m.Read<Card, Guid>(It.IsAny<Guid>()))
                .Returns(default(Card));

            // Act
            sut.Enter(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(GateException))]
        public void Enter_WithPendingTrip_ShouldThrowGateException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card();
            card.Trips = (new[]
            {
                new Trip()
                {
                    Entry = DateTime.Today.AddHours(1),
                    EntryStationNumber = 1
                }
            }).ToList();

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Enter(cardNumber, 1);
        }

        [TestMethod]
        public void Enter_WithExpiredCard_ShouldThrowGateException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var expiredCard = new Card()
            {
                Expiry = DateTime.Today.AddDays(-1)
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { expiredCard }).AsQueryable());

            // Act
            var ex = Assert.ThrowsException<GateException>(() => sut.Enter(cardNumber, 1));

            // Assert
            ex.Message.Should().Be("Card has expired.");
        }

        [TestMethod]
        public void Enter_WithNonExpiringCard_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var expiredCard = new Card();

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { expiredCard }).AsQueryable());

            // Act
            sut.Enter(cardNumber, 1);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Enter_WithExistingCard_TripShouldBeAddedToCard()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card();
            card.Trips = (new[]
            {
                new Trip()
                {
                    Entry = DateTime.Today.AddHours(1),
                    EntryStationNumber = 1,
                    Exit = DateTime.Today.AddHours(1.4),
                    ExitStationNumber = 2
                }
            }).ToList();

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Enter(cardNumber, 1);

            // Assert
            card.Trips.Count.Should().Be(2);
        }

        [TestMethod]
        public void Enter_WithExistingCard_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card();
            card.Trips = (new[]
            {
                new Trip()
                {
                    Entry = DateTime.Today.AddHours(1),
                    EntryStationNumber = 1,
                    Exit = DateTime.Today.AddHours(1.4),
                    ExitStationNumber = 2
                }
            }).ToList();

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Enter(cardNumber, 1);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        #endregion
        #region Exit

        [TestMethod]
        [ExpectedException(typeof(CardNotFoundException))]
        public void Exit_WithEmptyCardId_ShouldThrowCardNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.Empty;

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(CardNotFoundException))]
        public void Exit_WithNonExistingCardId_ShouldThrowCardNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();

            mockRepository
                .Setup(m => m.Read<Card, Guid>(It.IsAny<Guid>()))
                .Returns(default(Card));

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(GateException))]
        public void Exit_WithNoPendingTrips_ShouldThrowGateExeption()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card
            {
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1,
                        Exit = DateTime.Today.AddHours(1.5),
                        ExitStationNumber = 2
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(GateException))]
        public void Exit_WithNoTrips_ShouldThrowGateExeption()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { new Card() }).AsQueryable());

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(GateException))]
        public void Exit_WithNonExistingFareStrategy_ShouldThrowCardNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid()
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(GateException))]
        public void Exit_WithNonExistingFareStrategy_ShouldThrowGateException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid()
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns((IFareStrategy)null);

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(GateException))]
        public void Exit_WithNonExistingDiscountStrategy_ShouldThrowGateException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid(),
                    DiscountStrategyId = Guid.NewGuid()
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            mockStrategyFactory
                .Setup(m => m.GetDiscountStrategy(It.IsAny<Guid>()))
                .Returns((IDiscountStrategy)null);

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(GateException))]
        public void Exit_WithFareGreaterThanCardBalance_ShouldThrowGateException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Balance = 10M,
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid()
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockFareStrategy
                .Setup(m => m.GetFare(It.IsAny<Card>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(20M);

            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            // Act
            sut.Exit(cardNumber, 1);
        }

        [TestMethod]
        public void Exit_WithExistingFareStrategy_ShouldSubtractFareFromCardBalance()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid()
                },
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockFareStrategy
                .Setup(m => m.GetFare(It.IsAny<Card>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(20M);

            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            // Act
            sut.Exit(cardNumber, 1);

            // Assert
            card.Balance.Should().Be(80M);
        }

        [TestMethod]
        public void Exit_WithExistingFareStrategy_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid()
                },
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            // Act
            sut.Exit(cardNumber, 1);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Exit_WithPendingTrip_TripShouldBeUpdated()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid()
                },
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            // Act
            sut.Exit(cardNumber, 2);

            // Assert
            card.Trips.FirstOrDefault().Should().Match<Trip>(t =>
                t.Entry == DateTime.Today.AddHours(1) &&
                t.EntryStationNumber == 1 &&
                t.Exit.HasValue &&
                t.Exit.Value.Date == DateTime.Today &&
                t.ExitStationNumber == 2
            );
        }

        [TestMethod]
        public void Exit_WithExistingDiscountStrategy_ShouldSubtractDiscountPercentateFromFare()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid(),
                    DiscountStrategyId = Guid.NewGuid()
                },
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockFareStrategy
                .Setup(m => m.GetFare(It.IsAny<Card>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(20M);

            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            var mockDiscountStrategy = new Mock<IDiscountStrategy>();
            mockDiscountStrategy
                .Setup(m => m.GetPrecentageDiscount(It.IsAny<Card>()))
                .Returns(0.2M);

            mockStrategyFactory
                .Setup(m => m.GetDiscountStrategy(It.IsAny<Guid>()))
                .Returns(mockDiscountStrategy.Object);

            // Act
            sut.Exit(cardNumber, 1);

            // Assert
            card.Balance.Should().Be(84M);
        }

        [TestMethod]
        public void Exit_WithExistingDiscountStrategy_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid(),
                    DiscountStrategyId = Guid.NewGuid()
                },
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            var mockDiscountStrategy = new Mock<IDiscountStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetDiscountStrategy(It.IsAny<Guid>()))
                .Returns(mockDiscountStrategy.Object);

            // Act
            sut.Exit(cardNumber, 1);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Exit_WithEmptyDiscountStrategyId_ShouldNotInvokeGetDiscountStrategyMethod()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid(),
                    DiscountStrategyId = Guid.Empty
                },
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            // Act
            sut.Exit(cardNumber, 1);

            // Assert
            mockStrategyFactory.Verify(m => m.GetDiscountStrategy(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Exit_WithEmptyDiscountStrategyId_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    FareStrategyId = Guid.NewGuid(),
                    DiscountStrategyId = Guid.Empty
                },
                Trips = (new[]
                {
                    new Trip()
                    {
                        Entry = DateTime.Today.AddHours(1),
                        EntryStationNumber = 1
                    }
                }).ToList()
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            // Act
            sut.Exit(cardNumber, 1);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        } 

        #endregion
    }
}
