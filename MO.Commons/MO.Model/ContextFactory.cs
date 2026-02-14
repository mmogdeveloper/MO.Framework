using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MO.Model.Context;

namespace MO.Model
{
    /// <summary>
    /// Add-Migration InitMOData -c MODataContext -o Migrations/MOData
    /// Update-Database -c MODataContext
    /// Remove-Migration -c MODataContext
    /// </summary>
    public class MODataContextFactory : IDesignTimeDbContextFactory<MODataContext>
    {
        public MODataContext CreateDbContext(string[] args)
        {
            var connStr = ConfigHelper.GetConnectionString("MOData");
            var optionsBuilder = new DbContextOptionsBuilder<MODataContext>();
            optionsBuilder.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
            return new MODataContext(optionsBuilder.Options);
        }
    }

    /// <summary>
    /// Add-Migration InitMORecord -c MORecordContext -o Migrations/MORecord
    /// Update-Database -c MORecordContext
    /// Remove-Migration -c MORecordContext
    /// </summary>
    public class MORecordContextFactory : IDesignTimeDbContextFactory<MORecordContext>
    {
        public MORecordContext CreateDbContext(string[] args)
        {
            var connStr = ConfigHelper.GetConnectionString("MORecord");
            var optionsBuilder = new DbContextOptionsBuilder<MORecordContext>();
            optionsBuilder.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
            return new MORecordContext(optionsBuilder.Options);
        }
    }
}
