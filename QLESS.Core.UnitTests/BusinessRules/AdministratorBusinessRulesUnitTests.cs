using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QLESS.Core.BusinessRules;
using QLESS.Core.Data;
using QLESS.Core.Entities;
using QLESS.Core.Models;
using QLESS.Core.Strategies;
using QLESS.Core.Strategies.DiscountStrategies;
using QLESS.Core.Strategies.FareStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QLESS.Core.UnitTests.BusinessRules
{
    [TestClass]
    public class AdministratorBusinessRulesUnitTests
    {
        // Fields
        private AdministratorBusinessRules sut;
        private Mock<IRepository> mockRepository;
        private Mock<IStrategyFactory> mockStrategyFactory;

        // Initialization
        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new Mock<IRepository>();
            mockStrategyFactory = new Mock<IStrategyFactory>();

            var mockFareStrategy = new Mock<IFareStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns(mockFareStrategy.Object);

            var mockDiscountStrategy = new Mock<IDiscountStrategy>();
            mockStrategyFactory
                .Setup(m => m.GetDiscountStrategy(It.IsAny<Guid>()))
                .Returns(mockDiscountStrategy.Object);

            sut = new AdministratorBusinessRules(
                mockRepository.Object,
                mockStrategyFactory.Object
            );
        }

        // Test Methods
        #region CreateCardType

        [TestMethod]
        public void CreateCardType_WithNullName_ShouldThrowsCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Name should not be null, empty, or whitespace.");
        }

        [TestMethod]
        public void CreateCardType_WithEmptyName_ShouldThrowsCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns(string.Empty);

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Name should not be null, empty, or whitespace.");
        }

        [TestMethod]
        public void CreateCardType_WithWhiteSpaceName_ShouldThrowsCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns(" ");

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Name should not be null, empty, or whitespace.");
        }

        [TestMethod]
        public void CreateCardType_WithDescriptionExceedingCharacterLimit_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 256));

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Description should not exceed 255 characters.");
        }

        [TestMethod]
        public void CreateCardType_WithMinimumBalanceGreaterThenMaximumBalance_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(3000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Minimum balance should be greater than or equal to zero but less than maximum balance.");
        }

        [TestMethod]
        public void CreateCardType_WithMinimumBalanceLessThanZero_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(-5M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(3000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Minimum balance should be greater than or equal to zero but less than maximum balance.");
        }

        [TestMethod]
        public void CreateCardType_WithInitialBalanceAmountGreaterThanMaximumBalance_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.InitialBalance).Returns(12000M);
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(300M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Initial balance should be greater than or equal to zero but less than maximum balance.");
        }

        [TestMethod]
        public void CreateCardType_WithInitialBalanceLessThanZero_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.InitialBalance).Returns(-5M);
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(300M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Initial balance should be greater than or equal to zero but less than maximum balance.");
        }

        [TestMethod]
        public void CreateCardType_WithMinimumReloadAmountGreaterThanMaximumReloadAmount_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(300M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Minimum reload amount should be greater than or equal to zero but less than maximum reload amount.");
        }

        [TestMethod]
        public void CreateCardType_WithMinimumReloadAmountLessThanZero_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(-5M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Minimum reload amount should be greater than or equal to zero but less than maximum reload amount.");
        }

        [TestMethod]
        public void CreateCardType_WithBaseFareLessThanZero_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(10M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.BaseFare).Returns(-5M);

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Base fare should be greater than or equal to zero but less than initial balance.");
        }

        [TestMethod]
        public void CreateCardType_WithBaseFareGreaterThanInitialBalance_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(10M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.BaseFare).Returns(20M);
            mockModel.SetupGet(m => m.InitialBalance).Returns(10M);

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Base fare should be greater than or equal to zero but less than initial balance.");
        }

        [TestMethod]
        public void CreateCardType_WithNonExistingFareStrategyId_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(300M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });
            mockModel.SetupGet(m => m.FareStrategyId).Returns(Guid.NewGuid());

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            mockStrategyFactory
                .Setup(m => m.GetFareStrategy(It.IsAny<Guid>()))
                .Returns((IFareStrategy)null);

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Fare strategy not found.");
        }

        [TestMethod]
        public void CreateCardType_WithNonExistingDiscountStrategyId_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });
            mockModel.SetupGet(m => m.FareStrategyId).Returns(Guid.NewGuid());
            mockModel.SetupGet(m => m.DiscountStrategyId).Returns(Guid.NewGuid());

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            mockStrategyFactory
                .Setup(m => m.GetDiscountStrategy(It.IsAny<Guid>()))
                .Returns((IDiscountStrategy)null);

            // Act
            var ex = Assert.ThrowsException<CreateCardTypeException>(() => sut.CreateOrEditCardType(mockModel.Object));

            // Assert
            ex.Message.Should().Be("Discount strategy not found.");
        }

        [TestMethod]
        public void CreateCardType_WithNullDiscountStrategyId_ShouldInvokeCreateMethod()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(300M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });
            mockModel.SetupGet(m => m.DiscountStrategyId).Returns(Guid.Empty);

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => c);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.Create(It.IsAny<CardType>()), Times.Once);
        }

        [TestMethod]
        public void CreateCardType_WithEmptyDiscountStrategyId_ShouldNotInvokeGetDiscountStrategyMethod()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.MinimumBalance).Returns(5000M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(300M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new[] { Guid.NewGuid() });
            mockModel.SetupGet(m => m.DiscountStrategyId).Returns(Guid.Empty);

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(new Privilege());

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => c);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockStrategyFactory.Verify(m => m.GetDiscountStrategy(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void CreateCardType_WithNonEmptyCardTypeId_ShouldNotInvokeCreateMethod()
        {
            // Arrange
            var cardTypeId = Guid.NewGuid();
            var cardType = new CardType()
            {
                Id = cardTypeId
            };
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Id).Returns(cardTypeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.PrivilegeIds).Returns((ICollection<Guid>)null);

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardType);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.Create(It.IsAny<CardType>()), Times.Never());
        }

        [TestMethod]
        public void CreateCardType_WithNonEmptyCardTypeId_ShouldInvokeReadMethod()
        {
            // Arrange
            var cardTypeId = Guid.NewGuid();
            var cardType = new CardType()
            {
                Id = cardTypeId
            };
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Id).Returns(cardTypeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.PrivilegeIds).Returns((ICollection<Guid>)null);

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardType);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.Read<CardType, Guid>(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public void CreateCardType_WithExistingCardType_CardTypeShouldMapPropertiesFromModel()
        {
            // Arrange
            var cardTypeId = Guid.NewGuid();
            var cardType = new CardType()
            {
                Id = cardTypeId
            };
            var privilegedId1 = Guid.NewGuid();
            var privilegedId2 = Guid.NewGuid();
            var privilegeIds = (new[] { privilegedId1, privilegedId2 });
            var fareStrategyId = Guid.NewGuid();
            var discountStrategyId = Guid.NewGuid();

            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Id).Returns(cardTypeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.InitialBalance).Returns(100M);
            mockModel.SetupGet(m => m.MinimumBalance).Returns(10M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.BaseFare).Returns(20M);
            mockModel.SetupGet(m => m.Validity).Returns(TimeSpan.FromDays(100).Ticks);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(privilegeIds);
            mockModel.SetupGet(m => m.FareStrategyId).Returns(fareStrategyId);
            mockModel.SetupGet(m => m.DiscountStrategyId).Returns(discountStrategyId);

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns(cardType);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            cardType.Should().Match<CardType>(m =>
                m.Name == "Test Name" &&
                m.Description == new string('*', 255) &&
                m.InitialBalance == 100M &&
                m.MinimumBalance == 10M &&
                m.MaximumBalance == 10000M &&
                m.MinimumReloadAmount == 100M &&
                m.MaximumReloadAmount == 1000M &&
                m.BaseFare == 20M &&
                m.Validity == TimeSpan.FromDays(100).Ticks &&
                m.Privileges != null &&
                m.Privileges.All(p => privilegeIds.Contains(p.Id)) &&
                m.FareStrategyId == fareStrategyId &&
                m.DiscountStrategyId == discountStrategyId
            );
        }

        [TestMethod]
        [ExpectedException(typeof(CreateCardTypeException))]
        public void CreateCardType_WithNonExistingCardType_ShouldThrowCreateCardTypeException()
        {
            // Arrange
            var cardTypeId = Guid.NewGuid();
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Id).Returns(cardTypeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.PrivilegeIds).Returns((ICollection<Guid>)null);

            mockRepository
                .Setup(m => m.Read<CardType, Guid>(It.IsAny<Guid>()))
                .Returns((CardType)null);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);
        }

        [TestMethod]
        public void CreateCardType_WithValidParameters_ShouldInvokeCreateMethod()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.PrivilegeIds).Returns((ICollection<Guid>)null);

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => c);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockRepository
                .Verify(m => m.Create(It.IsAny<CardType>()), Times.Once());
        }

        [TestMethod]
        public void CreateCardType_WithValidParameters_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.PrivilegeIds).Returns((ICollection<Guid>)null);

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => c);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockRepository
                .Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void CreateCardType_WithValidParameters_NewCardTypeShouldHaveCorrectValues()
        {
            // Arrange
            var privilegedId1 = Guid.NewGuid();
            var privilegedId2 = Guid.NewGuid();
            var privilegeIds = (new[] { privilegedId1, privilegedId2 });
            var fareStrategyId = Guid.NewGuid();
            var discountStrategyId = Guid.NewGuid();

            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.InitialBalance).Returns(100M);
            mockModel.SetupGet(m => m.MinimumBalance).Returns(10M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.BaseFare).Returns(20M);
            mockModel.SetupGet(m => m.Validity).Returns(TimeSpan.FromDays(100).Ticks);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(privilegeIds);
            mockModel.SetupGet(m => m.FareStrategyId).Returns(fareStrategyId);
            mockModel.SetupGet(m => m.DiscountStrategyId).Returns(discountStrategyId);

            var newCardType = default(CardType);

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => newCardType = c);

            mockRepository
                .Setup(m => m.Read(It.IsAny<Expression<Func<Privilege, bool>>>()))
                .Returns(privilegeIds.Select(id => new Privilege() { Id = id }).AsQueryable());

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            newCardType.Should().Match<CardType>(c =>
                c.FareStrategyId == fareStrategyId &&
                c.DiscountStrategyId == discountStrategyId &&
                c.Name == "Test Name" &&
                c.Description == new string('*', 255) &&
                c.InitialBalance == 100M &&
                c.MinimumBalance == 10M &&
                c.MaximumBalance == 10000M &&
                c.MinimumReloadAmount == 100M &&
                c.MaximumReloadAmount == 1000M &&
                c.BaseFare == 20M &&
                c.Validity == TimeSpan.FromDays(100).Ticks &&
                c.Privileges != null &&
                c.Privileges.All(p => privilegeIds.Contains(p.Id))
            );
        }

        [TestMethod]
        public void CreateCardType_WithNullPrivilegeIds_ShouldNotInvokeReadMethod()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.PrivilegeIds).Returns((ICollection<Guid>)null);

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => c);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockRepository
                .Verify(m => m.Read(It.IsAny<Expression<Func<Privilege, bool>>>()), Times.Never());
        }

        [TestMethod]
        public void CreateCardType_WithEmptyPrivilegeIds_ShouldNotInvokeReadMethod()
        {
            // Arrange
            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(new Guid[] { });

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => c);

            // Act
            sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            mockRepository
                .Verify(m => m.Read(It.IsAny<Expression<Func<Privilege, bool>>>()), Times.Never());
        }

        [TestMethod]
        public void CreateCardType_WithValidParameters_ShouldReturnCardTypeModelWithCorrectValues()
        {
            // Arrange
            var privilegedId1 = Guid.NewGuid();
            var privilegedId2 = Guid.NewGuid();
            var privilegeIds = (new[] { privilegedId1, privilegedId2 });

            var fareStrategyId = Guid.NewGuid();
            var discountStrategyId = Guid.NewGuid();

            var mockModel = new Mock<ICardTypeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.InitialBalance).Returns(100M);
            mockModel.SetupGet(m => m.MinimumBalance).Returns(10M);
            mockModel.SetupGet(m => m.MaximumBalance).Returns(10000M);
            mockModel.SetupGet(m => m.MinimumReloadAmount).Returns(100M);
            mockModel.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            mockModel.SetupGet(m => m.BaseFare).Returns(20M);
            mockModel.SetupGet(m => m.Validity).Returns(TimeSpan.FromDays(100).Ticks);
            mockModel.SetupGet(m => m.PrivilegeIds).Returns(privilegeIds);
            mockModel.SetupGet(m => m.FareStrategyId).Returns(fareStrategyId);
            mockModel.SetupGet(m => m.DiscountStrategyId).Returns(discountStrategyId);

            mockRepository
                .Setup(m => m.Create(It.IsAny<CardType>()))
                .Returns((CardType c) => c);

            mockRepository
                .Setup(m => m.Read(It.IsAny<Expression<Func<Privilege, bool>>>()))
                .Returns(privilegeIds.Select(id => new Privilege() { Id = id }).AsQueryable());

            // Act
            var actual = sut.CreateOrEditCardType(mockModel.Object);

            // Assert
            var expected = new Mock<ICardTypeModel>();
            expected.SetupGet(m => m.Name).Returns("Test Name");
            expected.SetupGet(m => m.Description).Returns(new string('*', 255));
            expected.SetupGet(m => m.InitialBalance).Returns(100M);
            expected.SetupGet(m => m.MinimumBalance).Returns(10M);
            expected.SetupGet(m => m.MaximumBalance).Returns(10000M);
            expected.SetupGet(m => m.MinimumReloadAmount).Returns(100M);
            expected.SetupGet(m => m.MaximumReloadAmount).Returns(1000M);
            expected.SetupGet(m => m.BaseFare).Returns(20M);
            expected.SetupGet(m => m.Validity).Returns(TimeSpan.FromDays(100).Ticks);
            expected.SetupGet(m => m.PrivilegeIds).Returns(privilegeIds);
            expected.SetupGet(m => m.FareStrategyId).Returns(fareStrategyId);
            expected.SetupGet(m => m.DiscountStrategyId).Returns(discountStrategyId);

            actual.Should().BeEquivalentTo(expected.Object);
        }

        #endregion
        #region CreatePriviledge

        [TestMethod]
        [ExpectedException(typeof(CreatePriviledgeException))]
        public void CreatePriviledge_WithNameEqualToNull_ShouldThrowsCreatePriviledgeException()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(CreatePriviledgeException))]
        public void CreatePriviledge_WithNameEqualToEmptyString_ShouldThrowsCreatePriviledgeException()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns(string.Empty);

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(CreatePriviledgeException))]
        public void CreatePriviledge_WithNameEqualToWhiteSpace_ShouldhrowsCreatePriviledgeException()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns(" ");

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(CreatePriviledgeException))]
        public void CreatePriviledge_WithDescriptionExceedingCharacterLimit_ShouldThrowCreatePriviledgeException()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 256));

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(CreatePriviledgeException))]
        public void CreatePriviledge_WithInvalidIdentificationNumerPattern_ShouldThrowsCreatePriviledgeException()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("*?|");

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);
        }

        [TestMethod]
        public void CreatePriviledge_WithEmptyIdentificationNumberPattern_ShouldInvokeCreateMethod()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));

            mockRepository
                .Setup(m => m.Create(It.IsAny<Privilege>()))
                .Returns((Privilege p) => p);

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.Create(It.IsAny<Privilege>()), Times.Once);
        }

        [TestMethod]
        public void CreatePriviledge_WithEmptyPrivilegeId_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var privilegeId = Guid.NewGuid();
            var privilege = new Privilege()
            {
                Id = privilegeId
            };

            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Id).Returns(privilegeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(privilege);

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void CreatePriviledge_WithEmptyPrivilegeId_ShouldNotInvokeReadMethod()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            mockRepository
                .Setup(m => m.Create(It.IsAny<Privilege>()))
                .Returns((Privilege p) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void CreatePriviledge_WithExistingPrivilege_ShouldNotInvokeCreateMethod()
        {
            // Arrange
            var privilegeId = Guid.NewGuid();
            var privilege = new Privilege()
            {
                Id = privilegeId
            };

            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Id).Returns(privilegeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(privilege);

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.Create(It.IsAny<Privilege>()), Times.Never);
        }

        [TestMethod]
        public void CreatePriviledge_WithExistingPrivilege_PrivilegeShouldMapPropertiesFromModel()
        {
            // Arrange
            var privilegeId = Guid.NewGuid();
            var privilege = new Privilege()
            {
                Id = privilegeId
            };

            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Id).Returns(privilegeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns(privilege);

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            privilege.Should().Match<Privilege>(m =>
                m.Name == "Test Name" &&
                m.Description == new string('*', 255) &&
                m.IdentificationNumberPattern == "[a-z]"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(CreatePriviledgeException))]
        public void CreatePriviledge_WithNonExistingPrivilege_ShouldThrowCreatePriviledgeException()
        {
            // Arrange
            var privilegeId = Guid.NewGuid();
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Id).Returns(privilegeId);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            mockRepository
                .Setup(m => m.Read<Privilege, Guid>(It.IsAny<Guid>()))
                .Returns((Privilege)null);

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);
        }

        [TestMethod]
        public void CreatePriviledge_WithValidParameters_NewPrivilegeShouldHaveCorrectValues()
        {
            // Arrange
            var privilegeId = Guid.NewGuid();
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            var newPrivilege = default(Privilege);

            mockRepository
                .Setup(m => m.Create(It.IsAny<Privilege>()))
                .Returns((Privilege p) => 
                {
                    p.Id = privilegeId;
                    newPrivilege = p;
                    return newPrivilege;
                });

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            newPrivilege.Should().Match<Privilege>(m =>
                m.Id == privilegeId &&
                m.Name == "Test Name" &&
                m.Description == new string('*', 255) &&
                m.IdentificationNumberPattern == "[a-z]"
            );
        }

        [TestMethod]
        public void CreatePriviledge_WithValidParameters_ShouldInvokeSaveChangesMethod()
        {
            // Arrange
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            mockRepository
                .Setup(m => m.Create(It.IsAny<Privilege>()))
                .Returns((Privilege p) => p);

            // Act
            sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            mockRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void CreatePriviledge_WithValidParameters_ShouldReturnCreatePrivilegeModelWithCorrectValues()
        {
            // Arrange
            var privilegeId = Guid.NewGuid();
            var mockModel = new Mock<IPrivilegeModel>();
            mockModel.SetupProperty(m => m.Id);
            mockModel.SetupGet(m => m.Name).Returns("Test Name");
            mockModel.SetupGet(m => m.Description).Returns(new string('*', 255));
            mockModel.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            mockRepository
                .Setup(m => m.Create(It.IsAny<Privilege>()))
                .Returns((Privilege p) => 
                {
                    p.Id = privilegeId;
                    return p;
                });

            // Act
            var actual = sut.CreateOrEditPriviledge(mockModel.Object);

            // Assert
            var expected = new Mock<IPrivilegeModel>();
            expected.SetupGet(m => m.Id).Returns(privilegeId);
            expected.SetupGet(m => m.Name).Returns("Test Name");
            expected.SetupGet(m => m.Description).Returns(new string('*', 255));
            expected.SetupGet(m => m.IdentificationNumberPattern).Returns("[a-z]");

            actual.Should().BeEquivalentTo(expected.Object);
        }

        #endregion
    }
}
