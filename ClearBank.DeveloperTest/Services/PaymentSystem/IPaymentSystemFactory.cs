using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSystems
{
    public interface IPaymentSystemFactory
    {
        IPaymentSystem CreatePaymentSystem(PaymentScheme paymentScheme);
    }
}