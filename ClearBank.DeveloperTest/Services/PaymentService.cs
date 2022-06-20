using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;
using ClearBank.DeveloperTest.Infrastructure;
using ClearBank.DeveloperTest.Services.PaymentSystems;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentSystemFactory _paymentSystemFactory;

        public PaymentService()
        {
            var configuration = new ClearBankConfiguration()
            {
                DataStoreType = ConfigurationManager.AppSettings["DataStoreType"]
            };
            var dataStoreFactory = new DataStoreFactory(configuration);
            var paymentSystems = new List<IPaymentSystem>()
             {
                 new FasterPaymentsSystem(dataStoreFactory),
                 new BankersAutomatedClearingSystem(dataStoreFactory),
                 new ClearingHouseAutomatedPaymentSystemSystem(dataStoreFactory)
             };
            _paymentSystemFactory = new PaymentSystemFactory(paymentSystems);
        }

        public PaymentService(IPaymentSystemFactory paymentSystemFactory)
        {
            _paymentSystemFactory = paymentSystemFactory;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var paymentSystem = _paymentSystemFactory.CreatePaymentSystem(request.PaymentScheme);

            return new MakePaymentResult()
            {
                Success = paymentSystem.MakePayment(request.DebtorAccountNumber, request.Amount)
            };
        }
    }
}
