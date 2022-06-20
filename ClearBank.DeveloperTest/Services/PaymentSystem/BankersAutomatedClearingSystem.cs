using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSystems
{
    public class BankersAutomatedClearingSystem : BasePaymentSystem
    {
        public BankersAutomatedClearingSystem(IDataStoreFactory dataStoreFactory) :
            base(PaymentScheme.Bacs,
                        AllowedPaymentSchemes.Bacs, dataStoreFactory)
        { }
    }
}