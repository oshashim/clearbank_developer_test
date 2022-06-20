using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSystems
{
    public class PaymentSystemFactory : IPaymentSystemFactory
    {

        private readonly IList<IPaymentSystem> _paymentSystems;
        public PaymentSystemFactory(IList<IPaymentSystem> paymentSystems)
        {
            _paymentSystems = paymentSystems;
        }

        public IPaymentSystem CreatePaymentSystem(PaymentScheme paymentScheme)
        {
            return _paymentSystems.FirstOrDefault(x => x.PaymentScheme == paymentScheme);
        }
    }
}