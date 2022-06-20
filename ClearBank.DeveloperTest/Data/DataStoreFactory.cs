using ClearBank.DeveloperTest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.DeveloperTest.Data
{
    public class DataStoreFactory : IDataStoreFactory
    {
        private string DataStoreType { get; }

        public DataStoreFactory(ClearBankConfiguration clearBankConfiguration)
        {
            DataStoreType = clearBankConfiguration.DataStoreType;
        }


        public IDataStore CreateDataStore() => DataStoreType == "Backup" ? (IDataStore)new BackupAccountDataStore() : new AccountDataStore();
    }
}
