using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Common.Interfaces.Repositories;

namespace TestApp.Data.Repositories
{
    public class DiskRepository : IDiskRepository
    {
        public string DiskSpace(string test)
        {
            return test.ToUpper();
        }
    }
}
