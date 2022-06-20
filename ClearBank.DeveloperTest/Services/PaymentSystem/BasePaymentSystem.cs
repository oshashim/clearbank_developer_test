using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentSystems
{
    public abstract class BasePaymentSystem : IPaymentSystem
    {
        private readonly AllowedPaymentSchemes _allowedPaymentSchemes;
        private IDataStore DataStore { get; }
        protected BasePaymentSystem(PaymentScheme paymentScheme, AllowedPaymentSchemes allowedPaymentSchemes, IDataStoreFactory dataStoreFactory)
        {
            PaymentScheme = paymentScheme;
            _allowedPaymentSchemes = allowedPaymentSchemes;
            DataStore = dataStoreFactory.CreateDataStore();
        }

        public PaymentScheme PaymentScheme { get; }
        public virtual bool MakePayment(string debtorAccountNumber, decimal amount)
        {
            var debtorAccount = DataStore.GetAccount(debtorAccountNumber);
            if (!IsValidAccount(debtorAccount) ||
                 !IsThisPaymentSchemeAllowedByAccount(debtorAccount) ||
                 !HasSchemeSpecificConditionsMet(debtorAccount, amount))
                return false;
            debtorAccount.Balance -= amount;
            DataStore.UpdateAccount(debtorAccount);
            return true;
        }

        protected virtual bool HasSchemeSpecificConditionsMet(Account account, decimal amount) => true;

        private bool IsThisPaymentSchemeAllowedByAccount(Account account) => account.AllowedPaymentSchemes.HasFlag(_allowedPaymentSchemes);

        private bool IsValidAccount(Account account) => account != null;

    }
}