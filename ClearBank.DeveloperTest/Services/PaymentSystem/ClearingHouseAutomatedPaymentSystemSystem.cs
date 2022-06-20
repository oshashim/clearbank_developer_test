using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSystems
{
    public class ClearingHouseAutomatedPaymentSystemSystem : BasePaymentSystem
    {
        public ClearingHouseAutomatedPaymentSystemSystem(IDataStoreFactory dataStoreFactory) :
            base(PaymentScheme.Chaps,
                AllowedPaymentSchemes.Chaps, dataStoreFactory)
        { }

        protected override bool HasSchemeSpecificConditionsMet(Account account, decimal amount)
            => account.Status == AccountStatus.Live;
    }
}