using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.PaymentSystems;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceShould
    {
        private readonly Mock<IDataStore> _mockDataStore;
        private readonly IPaymentService _paymentService;

        public PaymentServiceShould()
        {
            _mockDataStore = new Mock<IDataStore>();
            var mockDataStoreFactory = new Mock<IDataStoreFactory>();
            mockDataStoreFactory.Setup(x => x.CreateDataStore()).Returns(_mockDataStore.Object);

            var paymentSystems = new List<IPaymentSystem>()
            {
                new FasterPaymentsSystem(mockDataStoreFactory.Object),
                new BankersAutomatedClearingSystem(mockDataStoreFactory.Object),
                new ClearingHouseAutomatedPaymentSystemSystem(mockDataStoreFactory.Object)
            };
            IPaymentSystemFactory paymentSystemFactory = new PaymentSystemFactory(paymentSystems);
            _paymentService = new PaymentService(paymentSystemFactory);
        }

        public static IEnumerable<object[]> GetAccountInvalidRequest()
        {
            yield return new object[] { "", 100D, PaymentScheme.Bacs };
            yield return new object[] { "InValidAccount", 1000D, PaymentScheme.Bacs };
        }

        [Theory]
        [MemberData(nameof(GetAccountInvalidRequest))]
        public void ShouldReturnSuccessFalseWhenAccountIsInvalid(string debtorAccountNumber, decimal amount, PaymentScheme paymentScheme)
        {
            //Arrange
            _mockDataStore.Reset();
            _mockDataStore.Setup(x =>
                                        x.GetAccount(It.Is<string>(s => s.Equals(debtorAccountNumber))))
                                            .Returns((Account)null);
            var request = new MakePaymentRequest()
            {
                DebtorAccountNumber = debtorAccountNumber,
                Amount = amount,
                PaymentScheme = paymentScheme
            };
            var expectedResult = new MakePaymentResult() { Success = false };
            //Act
            var actual = _paymentService.MakePayment(request);
            //Assert
            Assert.Equal(expectedResult.Success, actual.Success);
        }


        public static IEnumerable<object[]> GetPaymentSystemValidationFailedRequest()
        {
            //BacsPaymentSchemeIsNotAllowedByAccount
            yield return new object[]
            {
                new MakePaymentRequest() {DebtorAccountNumber = "acc001", Amount = 100, PaymentScheme = PaymentScheme.Bacs},
                new Account() { AccountNumber = "acc001", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments }
            };
            //ChapsPaymentSchemeIsNotAllowedByAccount
            yield return new object[]
            {
                new MakePaymentRequest() {DebtorAccountNumber = "acc002", Amount = 100, PaymentScheme = PaymentScheme.Chaps},
                new Account() { AccountNumber = "acc002", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments }
            };
            //FasterPaymentSchemeIsNotAllowedByAccount
            yield return new object[]
            {
                new MakePaymentRequest() {DebtorAccountNumber = "acc003", Amount = 100, PaymentScheme = PaymentScheme.FasterPayments},
                new Account() { AccountNumber = "acc003", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps }
            };
            //FasterPaymentsShouldHaveAccountBalanceGreaterThanOrEqualToPaymentAmount (negative scenario)
            yield return new object[]
            {
                new MakePaymentRequest() {DebtorAccountNumber = "acc004", Amount = 201, PaymentScheme = PaymentScheme.FasterPayments},
                new Account() { AccountNumber = "acc004", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments }
            };
            //ChapsShouldHaveAccountStatusEqualToActive (negative scenario)
            yield return new object[]
            {
                new MakePaymentRequest() {DebtorAccountNumber = "acc005", Amount = 201, PaymentScheme = PaymentScheme.Chaps},
                new Account() { AccountNumber = "acc005", Balance = 201, Status = AccountStatus.Disabled,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments }
            };

        }

        [Theory]
        [MemberData(nameof(GetPaymentSystemValidationFailedRequest))]
        public void ShouldReturnSuccessFalseWhenPaymentSystemValidationFailed(MakePaymentRequest request, Account account)
        {
            var expectedResult = new MakePaymentResult() { Success = false };
            _mockDataStore.Reset();
            _mockDataStore.Setup(x =>
                    x.GetAccount(It.Is<string>(s => s.Equals(request.DebtorAccountNumber))))
                .Returns(account);
            //Act
            var actual = _paymentService.MakePayment(request);
            //Assert
            Assert.Equal(expectedResult.Success, actual.Success);
        }

        public static IEnumerable<object[]> GetPaymentSystemSuccessRequest()
        {
            //BacsPaymentScheme
            yield return new object[]
            {
                new MakePaymentRequest()
                    {DebtorAccountNumber = "acc001", Amount = 200, PaymentScheme = PaymentScheme.Bacs},
                new Account()
                {
                    AccountNumber = "acc001", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs
                }
            };

            yield return new object[]
            {
                new MakePaymentRequest()
                    {DebtorAccountNumber = "acc002", Amount = 100, PaymentScheme = PaymentScheme.Bacs},
                new Account()
                {
                    AccountNumber = "acc002", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs
                }
            };

            yield return new object[]
            {
                new MakePaymentRequest()
                    {DebtorAccountNumber = "acc003", Amount = 100, PaymentScheme = PaymentScheme.Bacs},
                new Account()
                {
                    AccountNumber = "acc003", Balance = 200, Status = AccountStatus.Disabled,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs
                }
            };

            //ChapsPaymentScheme
            yield return new object[]
            {
                new MakePaymentRequest()
                    {DebtorAccountNumber = "acc001", Amount = 200, PaymentScheme = PaymentScheme.Chaps},
                new Account()
                {
                    AccountNumber = "acc001", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs
                }
            };

            yield return new object[]
            {
                new MakePaymentRequest()
                    {DebtorAccountNumber = "acc002", Amount = 100, PaymentScheme = PaymentScheme.Chaps},
                new Account()
                {
                    AccountNumber = "acc002", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs
                }
            };

            //FasterPaymentScheme
            yield return new object[]
            {
                new MakePaymentRequest()
                    {DebtorAccountNumber = "acc001", Amount = 200, PaymentScheme = PaymentScheme.FasterPayments},
                new Account()
                {
                    AccountNumber = "acc001", Balance = 200, Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs
                }
            };

            yield return new object[]
            {
                new MakePaymentRequest()
                    {DebtorAccountNumber = "acc002", Amount = 100, PaymentScheme = PaymentScheme.FasterPayments},
                new Account()
                {
                    AccountNumber = "acc002", Balance = 101, Status = AccountStatus.Disabled,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetPaymentSystemSuccessRequest))]
        public void ShouldReturnSuccessTrueWhenPaymentSystemDebitsAmount(MakePaymentRequest request, Account account)
        {
            var expectedResult = new MakePaymentResult() { Success = true };
            _mockDataStore.Reset();
            _mockDataStore.Setup(x =>
                    x.GetAccount(It.Is<string>(s => s.Equals(request.DebtorAccountNumber))))
                .Returns(account);
            //Act
            var actual = _paymentService.MakePayment(request);
            //Assert
            Assert.Equal(expectedResult.Success, actual.Success);
        }

    }
}