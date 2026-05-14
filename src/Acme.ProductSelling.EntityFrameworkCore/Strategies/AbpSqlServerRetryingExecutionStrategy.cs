using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Strategies
{
    public class AbpSqlServerRetryingExecutionStrategy : SqlServerRetryingExecutionStrategy
    {
        public AbpSqlServerRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies)
            : base(dependencies, maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)
        {
        }
        
        // Disable retry when inside a transaction (ABP UoW uses transactions)
        public override bool RetriesOnFailure =>
            Dependencies.CurrentContext.Context.Database.CurrentTransaction == null;
    }
}
