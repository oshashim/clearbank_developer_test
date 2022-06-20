using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;
using ClearBank.DeveloperTest.Infrastructure;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
       
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            var configuration = new ClearBankConfiguration()
            {
                DataStoreType = ConfigurationManager.AppSettings["DataStoreType"]
            };
            var dataStoreFactory = new DataStoreFactory(configuration);
            Account account = null;

            if (dataStoreType == "Backup")
            {
                var accountDataStore = new BackupAccountDataStore();
                account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            }
            else
            {
                var accountDataStore = new AccountDataStore();
                account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            }

            var result = new MakePaymentResult();

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    
                   if(!hasValidAccount(account, AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    
                    if (!hasValidAccount(account, AllowedPaymentSchemes.FasterPayments))
                    {
                        result.Success = false;
                    }
                    else if (account.Balance < request.Amount)
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (!hasValidAccount(account, AllowedPaymentSchemes.Chaps))
                    {
                        result.Success = false;
                    }
                    else if (account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
                    }
                    break;
            }

            if (result.Success)
            {
                UpdateAccount(dataStoreType, account, request.Amount);
            }

            return result;
        }
        public bool hasValidAccount(Account account, AllowedPaymentSchemes allowedPaymentSchemes)
        {
            if (account == null)
            {
                return false; ;
            }
            else if (!account.AllowedPaymentSchemes.HasFlag(allowedPaymentSchemes))
            {
                return false;
            }
            return true;
        }
        public void  UpdateAccount(string dataStoreType ,Account account,decimal amount)
        {
            account.Balance -= amount;

            if (dataStoreType == "Backup")
            {
                var accountDataStore = new BackupAccountDataStore();
                accountDataStore.UpdateAccount(account);
            }
            else
            {
                var accountDataStore = new AccountDataStore();
                accountDataStore.UpdateAccount(account);
            }
        }
    }
}
