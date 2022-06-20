using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.DeveloperTest.Data
{
    internal interface IDataStoreFactory
    {
        IDataStore CreateDataStore();
    }
}
