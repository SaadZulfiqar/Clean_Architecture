using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoadTool.Core.Interfaces
{
    public interface IMigrationService
    {
        Task RemoveAsync();
        Task MigrateAsync();
    }
}
