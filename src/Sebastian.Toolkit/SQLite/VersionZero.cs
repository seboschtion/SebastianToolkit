using System;

namespace Sebastian.Toolkit.SQLite
{
    public class VersionZero : DatabaseVersion
    {
        public override Version DbVersion => new Version(0, 0, 0, 0);

        protected override void Upgrade(SQLiteTransaction transaction)
        {
            transaction.Execute(@"CREATE TABLE IF NOT EXISTS ""DatabaseInfo"" (
                                 ""Id"" INTEGER PRIMARY KEY AUTOINCREMENT, 
                                 ""Version"" TEXT NOT NULL)");
        }
    }
}