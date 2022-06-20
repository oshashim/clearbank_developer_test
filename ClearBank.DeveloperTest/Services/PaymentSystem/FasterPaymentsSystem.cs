using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSystems
{
    public class FasterPaymentsSystem : BasePaymentSystem
    {
        public FasterPaymentsSystem(IDataStoreFactory dataStoreFactory) :
            base(PaymentScheme.FasterPayments,
                AllowedPaymentSchemes.FasterPayments, dataStoreFactory)
        { }

        protected override bool HasSchemeSpecificConditionsMet(Account account, decimal amount)
            => account.Balance >= amount;



    }
}