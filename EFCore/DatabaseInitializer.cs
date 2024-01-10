
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace irm_wpf.EFCore
{
    public class DatabaseInitializer
    {
        public static string databasePath = "data/database.db";
        public static void InitializeDatabase()
        {
            if (!File.Exists(databasePath))
            {
                using (var dbContext = new MyDbContext())
                {
                    // 创建数据库和表（如果尚不存在）
                    dbContext.Database.EnsureCreated();
                    //执行数据迁移 暂时不需要
                    //dbContext.Database.Migrate();
                }
            }
        }
    }
}