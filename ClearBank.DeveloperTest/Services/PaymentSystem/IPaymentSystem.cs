using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSystems
{
    public interface IPaymentSystem
    {
        PaymentScheme PaymentScheme { get; }
        bool MakePayment(string debtorAccountNumber, decimal amount);
    }
}