using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QLESS.Core.BusinessRules;
using QLESS.Core.Data;
using QLESS.Core.Entities;
using QLESS.Core.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QLESS.Core.UnitTests.BusinessRules
{
    [TestClass]
    public class TicketingBusinessRulesUnitTest
    {
        // Fields
        private TicketingBusinessRules sut;
        private Mock<IRepository> mockRepository;

        // Initializations
        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new Mock<IRepository>();
            sut = new TicketingBusinessRules(mockRepository.Object);
        }

        // Test Methods
        #region Activate

        [TestMethod]
        [ExpectedException(typeof(InvalidIdException))]
        public void Activate_WithEmptyCardId_ShouldThrowInvalidCardException()
        {
            // Arrange
            var cardNumber = Guid.Empty;
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateCardException))]
        public void Activate_WithDuplicateCardId_ShouldThrowDuplicateCardException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { new Card() }).AsQueryable());

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidIdException))]
        public void Activate_WithEmptyCardTypeId_ShouldThrowInvalidCardTypeException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.Empty;
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(CardTypeNotFoundException))]
        public void Activate_WithNonExistingCardTypeId_ShouldThrowCardTypeNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(default(CardType));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);
        }

        [TestMethod]
        public void Activate_WithNonEmptyCardId_CardIdShouldBeAssignedCorrectly()
        {
            // Arrange
            var card = default(Card);
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(new CardType() 
                {
                    Privileges = new[] { new Privilege() { Id = privilegeId } }
                });

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (card = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            card.Number.Should().Be(cardNumber);
        }

        [TestMethod]
        public void Activate_WithNonEmptyCardId_CreatedShouldBeDateToday()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var createdCard = default(Card);
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(new CardType()
                {
                    Privileges = new[] { new Privilege() { Id = privilegeId } }
                });

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (createdCard = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            createdCard.Created.Should().Be(DateTime.Today);
        }

        [TestMethod]
        public void Activate_WithValidityOfFiveYears_ExpiryShouldBeFiveYearsFromCreated()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var createdCard = default(Card);
            var cardTypeWithFiveYearValidity = new CardType()
            {
                Validity = (DateTime.Today.AddYears(5) - DateTime.Today).Ticks,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveYearValidity);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (createdCard = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            createdCard.Expiry.Should().Be(DateTime.Today.AddYears(5));
        }

        [TestMethod]
        public void Activate_WithValidityOfThreeYears_ExpiryShouldBeThreeYearsFromCreated()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var createdCard = default(Card);
            var cardTypeWithFiveYearValidity = new CardType()
            {
                Validity = (DateTime.Today.AddYears(3) - DateTime.Today).Ticks,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveYearValidity);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (createdCard = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            createdCard.Expiry.Should().Be(DateTime.Today.AddYears(3));
        }

        [TestMethod]
        public void Activate_WithValidityOfFourYears_ExpiryShouldBeFourYearsFromCreated()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var createdCard = default(Card);
            var cardTypeWithFiveYearValidity = new CardType()
            {
                Validity = (DateTime.Today.AddYears(4) - DateTime.Today).Ticks, // 1 leap year
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveYearValidity);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (createdCard = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            createdCard.Expiry.Should().Be(DateTime.Today.AddYears(4));
        }

        [TestMethod]
        public void Activate_WithValidityOfOneWeek_ExpiryShouldBeSevenDaysFromCreated()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var createdCard = default(Card);
            var cardTypeWithFiveYearValidity = new CardType()
            {
                Validity = (DateTime.Today.AddDays(7) - DateTime.Today).Ticks,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveYearValidity);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (createdCard = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            createdCard.Expiry.Should().Be(DateTime.Today.AddDays(7));
        }

        [TestMethod]
        public void Activate_WithNoValidity_ExpiryShouldBeNull()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var createdCard = default(Card);
            var cardTypeWithFiveYearValidity = new CardType()
            {
                Validity = 0,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveYearValidity);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (createdCard = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            createdCard.Expiry.Should().BeNull();
        }

        [TestMethod]
        public void Activate_WithInitialBalanceOfOneHundred_BalanceShouldBeEqualToOneHundred()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var createdCard = default(Card);
            var cardTypeWithOneHundredInitialBalance = new CardType()
            {
                InitialBalance = 100,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithOneHundredInitialBalance);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (createdCard = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            createdCard.Balance.Should().Be(100);
        }

        [TestMethod]
        public void Activate_WithInitialBalanceOfFiveHundred_BalanceShouldBeFiveHundred()
        {
            // Arrange
            var card = default(Card);
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var cardTypeWithFiveHundredInitialBalance = new CardType()
            {
                InitialBalance = 500,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveHundredInitialBalance);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (card = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            card.Balance.Should().Be(500);
        }

        [TestMethod]
        public void Activate_WithRequiredPrivilegeButEmptyPrivilegeId_ShouldThrowNewCardActivationException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.Empty;
            var cardTypeWithRequirePrivilege = new CardType()
            {
                Privileges = new[]
                {
                    new Privilege() { Id = Guid.NewGuid() },
                    new Privilege() { Id = Guid.NewGuid() }
                }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithRequirePrivilege);

            // Act
            var ex = Assert.ThrowsException<CardActivationException>(() => sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber));

            // Assert
            ex.Message.Should().Be("Privilege id is invalid.");
        }

        [TestMethod]
        public void Activate_WithoutRequiredPrivilegeAndEmptyPrivilegeId_ShouldThrowNewCardActivationException()
        {
            // Arrange
            var card = default(Card);
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.Empty;
            var cardTypeWithRequirePrivilege = new CardType()
            {
                Privileges = new Privilege[] { }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithRequirePrivilege);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (card = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            mockRepository.Verify(m => m.Create(It.IsAny<Card>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(CardActivationException))]
        public void Activate_WithRequiredPrivilegeAndNonExistingPrivilegeId_ShouldThrowCardActivationException()
        {
            // Arrange
            var card = default(Card);
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var cardTypeWithRequirePrivilege = new CardType()
            {
                Privileges = new[]
                {
                    new Privilege() { Id = Guid.NewGuid() },
                    new Privilege() { Id = Guid.NewGuid() }
                }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithRequirePrivilege);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (card = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            card.PrivilegeCard.Should().NotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(CardActivationException))]
        public void Activate_WithRequiredPrivilegeAndInvalidPrivilegeCardNumber_ShouldThrowCardActivationException()
        {
            // Arrange
            var card = default(Card);
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var cardTypeWithRequirePrivilege = new CardType()
            {
                Privileges = new[]
                {
                    new Privilege() 
                    {
                        Id = privilegeId,  
                        IdentificationNumberPattern = @"^([a-zA-Z0-9]{2})-([a-zA-Z0-9]{4})-([a-zA-Z0-9]{4})$"
                    },
                }
            };
            var privilegeCardNumber = "ABC123";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithRequirePrivilege);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (card = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            card.PrivilegeCard.Should()
                .NotBeNull().And
                .Match<PrivilegeCard>(m =>
                    m.Type.Id == privilegeId &&
                    m.IdentificationNumber == "ABC123"
                );
        }

        [TestMethod]
        public void Activate_WithRequiredPrivilegeAndExistingPrivilegeId_PrivilegeCardShouldBeCreated()
        {
            // Arrange
            var card = default(Card);
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var cardTypeWithRequirePrivilege = new CardType()
            {
                Privileges = new[]
                {
                    new Privilege() { Id = privilegeId },
                    new Privilege() { Id = Guid.NewGuid() }
                }
            };
            var privilegeCardNumber = "ABC123";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithRequirePrivilege);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (card = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            card.PrivilegeCard.Should()
                .NotBeNull().And
                .Match<PrivilegeCard>(m =>
                    m.Type.Id == privilegeId &&
                    m.IdentificationNumber == "ABC123"
                );
        }

        [TestMethod]
        public void Activate_WithValidCardType_ShouldReturnCorrectCardId()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(new CardType()
                {
                    Privileges = new[] { new Privilege() { Id = privilegeId } }
                });

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => c);

            // Act
            var cardDataModel = sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            cardDataModel.Number.Should().Be(cardNumber);
        }

        [TestMethod]
        public void Activate_WithInitialBalanceOfFiveHundred_ShouldReturnBalanceEqualToFiveHundred()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var cardTypeWithFiveYearValidity = new CardType()
            {
                InitialBalance = 500,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveYearValidity);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => c);

            // Act
            var cardDataModel = sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            cardDataModel.Balance.Should().Be(500);
        }

        [TestMethod]
        public void Activate_WithValidityOfFiveYears_ShouldReturnExpiryEqualToFiveYearsFromToday()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var cardTypeWithFiveYearValidity = new CardType()
            {
                Validity = (DateTime.Today.AddYears(5) - DateTime.Today).Ticks,
                Privileges = new[] { new Privilege() { Id = privilegeId } }
            };
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardTypeWithFiveYearValidity);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => c);

            // Act
            var cardDataModel = sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            cardDataModel.Expiry.Should().Be(DateTime.Today.AddYears(5));
        }

        [TestMethod]
        public void Activate_WithValidCardIdAndCardTypeId_SaveChangesShouldBeInvoked()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(new CardType()
                {
                    Privileges = new[] { new Privilege() { Id = privilegeId } }
                });

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => c);

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void Activate_WithValidCardIdAndCardTypeId_CardShouldHaveCorrectValues()
        {
            // Arrange
            var card = default(Card);
            var cardNumber = Guid.NewGuid();
            var cardTypeId = Guid.NewGuid();
            var privilegeId = Guid.NewGuid();
            var privilegeCardNumber = "";

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(new CardType()
                {
                    Id = cardTypeId,
                    Privileges = new[] { new Privilege() { Id = privilegeId } },
                    Validity = (DateTime.Today.AddYears(5) - DateTime.Today).Ticks,
                    InitialBalance = 100M
                });

            mockRepository
                .Setup(m => m.Create(It.IsAny<Card>()))
                .Returns((Card c) => (card = c));

            // Act
            sut.Activate(cardNumber, cardTypeId, privilegeId, privilegeCardNumber);

            // Assert
            card.Should().Match<Card>(m =>
                m.Number == cardNumber &&
                m.Created == DateTime.Today &&
                m.Expiry == DateTime.Today.AddYears(5) &&
                m.Balance == 100M &&
                m.Type.Id == cardTypeId
            );
        }

        #endregion
        #region CheckBalance

        [TestMethod]
        [ExpectedException(typeof(InvalidIdException))]
        public void CheckBalance_WithEmptyCardId_ShouldThrowInvalidCardException()
        {
            // Arrange
            var cardNumber = Guid.Empty;

            // Act
            sut.CheckBalance(cardNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(CardNotFoundException))]
        public void CheckBalance_WithNonExistingCardId_ShouldThrowCardNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();

            // Act
            sut.CheckBalance(cardNumber);
        }

        [TestMethod]
        public void CheckBalance_WithExistingCardId_ShouldReturnCorrectCardId()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Number = cardNumber
            };

            mockRepository
                .Setup(m => m.Read(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardDataModel = sut.CheckBalance(cardNumber);

            // Assert
            cardDataModel.Number.Should().Be(cardNumber);
        }

        [TestMethod]
        public void CheckBalance_WithExistingCardId_ShouldReturnCorrectExpiry()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var expiry = DateTime.Today.AddYears(5);
            var card = new Card()
            {
                Expiry = DateTime.Today.AddYears(5)
            };

            mockRepository
                .Setup(m => m.Read(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardDataModel = sut.CheckBalance(cardNumber);

            // Assert
            cardDataModel.Expiry.Should().Be(expiry);
        }

        [TestMethod]
        public void CheckBalance_WithExistingCardId_ShouldReturnCorrectRemainingBalance()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Balance = 50
            };

            mockRepository
                .Setup(m => m.Read(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardDataModel = sut.CheckBalance(cardNumber);

            // Assert
            cardDataModel.Balance.Should().Be(50);
        }

        #endregion
        #region Reload

        [TestMethod]
        [ExpectedException(typeof(InvalidIdException))]
        public void Reload_WithEmptyCardId_ShouldThrowInvalidCardException()
        {
            // Arrange
            var cardNumber = Guid.Empty;

            // Act
            sut.Reload(cardNumber, 0.0M, 0.0M);
        }

        [TestMethod]
        [ExpectedException(typeof(CardNotFoundException))]
        public void Reload_WithNonExistingCardId_ShouldThrowCardNotFoundException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();

            // Act
            sut.Reload(cardNumber, 0.0M, 0.0M);
        }

        [TestMethod]
        [ExpectedException(typeof(ReloadAmountException))]
        public void Reload_WithAmountGreaterThanPayment_ShouldThrowReloadAmountException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Reload(cardNumber, 500M, 100M);
        }

        [TestMethod]
        [ExpectedException(typeof(ReloadAmountException))]
        public void Reload_WithAmountLessThanMinimumReloadAmount_ShouldThrowReloadAmountException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Reload(cardNumber, 99M, 100M);
        }

        [TestMethod]
        [ExpectedException(typeof(ReloadAmountException))]
        public void Reload_WithAmountGreaterThanMaximumReloadAmount_ShouldThrowReloadAmountException()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var card = new Card()
            {
                Type = new CardType()
                {
                    MaximumReloadAmount = 1000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card,bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Reload(cardNumber, 1001M, 0M);
        }

        [TestMethod]
        public void Reload_WithAmountWithinRange_AmountShouldBeAddedToBalance()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M;
            var payment = 1000M;
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Reload(cardNumber, amount, payment);

            // Assert
            card.Balance.Should().Be(600M);
        }

        [TestMethod]
        public void Reload_WithAmountWithinRange_ShouldReturnBalanceEqualToCardBalance()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M;
            var payment = 1000M;
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardReloadDataModel = sut.Reload(cardNumber, amount, payment);

            // Assert
            cardReloadDataModel.Balance.Should().Be(600M); // Old balance + amount
        }

        [TestMethod]
        public void Reload_WithAmountWithinRange_ShouldReturnAmount()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M;
            var payment = 1000M;
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardReloadDataModel = sut.Reload(cardNumber, amount, payment);

            // Assert
            cardReloadDataModel.Amount.Should().Be(amount);
        }

        [TestMethod]
        public void Reload_WithAmountWithinRange_ShouldReturnPayment()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M;
            var payment = 1000M;
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardReloadDataModel = sut.Reload(cardNumber, amount, payment);

            // Assert
            cardReloadDataModel.Payment.Should().Be(payment);
        }

        [TestMethod]
        public void Reload_WithPaymentGreaterThanAmount_ShouldReturnChangeEqualToPaymentLessAmount()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M;
            var payment = 1000M; // Will exceed amount by 500
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardReloadDataModel = sut.Reload(cardNumber, amount, payment);

            // Assert
            cardReloadDataModel.Change.Should().Be(500M);
        }

        [TestMethod]
        public void Reload_WithPaymentEqualToAmount_ShouldReturnChangeEqualToZero()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M;
            var payment = 500M; // Will be exactly the same as amount
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardReloadDataModel = sut.Reload(cardNumber, amount, payment);

            // Assert
            cardReloadDataModel.Change.Should().Be(0M);
        }

        [TestMethod]
        public void Reload_WithPaymentEqualToAmountAndExceedsMaximumBalance_ShouldReturnChangeEqualToExcessOfMaximumBalance()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 1000M; // Will exceed maximum balance by 500
            var payment = 1000M; // Will be exactly the same as amount
            var card = new Card()
            {
                Balance = 9500M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardReloadDataModel = sut.Reload(cardNumber, amount, payment);

            // Assert
            cardReloadDataModel.Change.Should().Be(500M); // Excess of maximum balance
        }

        [TestMethod]
        public void Reload_WithPaymentGreaterThenAmountAndExceedsMaximumBalance_ShouldReturnChangeEqualToExcessOfAmountAndMaximumBalance()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M; // Will exceed maximum balance by 300
            var payment = 1000M; // Will exceed amount by 500
            var card = new Card()
            {
                Balance = 9800M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            var cardReloadDataModel = sut.Reload(cardNumber, amount, payment);

            // Assert
            cardReloadDataModel.Change.Should().Be(800M); // Excess of amount + excess of maximum balance (500 + 500)
        }

        [TestMethod]
        public void Reload_WithAmountWithinRange_SaveChangesShouldBeInvoked()
        {
            // Arrange
            var cardNumber = Guid.NewGuid();
            var amount = 500M;
            var payment = 1000M;
            var card = new Card()
            {
                Balance = 100M,
                Type = new CardType()
                {
                    MinimumReloadAmount = 100M,
                    MaximumReloadAmount = 1000M,
                    MinimumBalance = 0M,
                    MaximumBalance = 10000M
                }
            };

            mockRepository
                .Setup(m => m.Read<Card>(It.IsAny<Expression<Func<Card, bool>>>()))
                .Returns((new[] { card }).AsQueryable());

            // Act
            sut.Reload(cardNumber, amount, payment);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        #endregion
    }
}
