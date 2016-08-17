using System;

namespace Sebastian.Toolkit.SQLite
{
    public abstract class DatabaseVersion
    {
        public abstract Version DbVersion { get; }
        protected abstract void Upgrade(SQLiteTransaction transaction);

        public void UpgradeAndUpdateVersion(SQLiteTransaction transaction)
        {
            Upgrade(transaction);
            UpdateDatabaseVersion(transaction);
        }

        private void UpdateDatabaseVersion(SQLiteTransaction transaction)
        {
            const string sql = @"INSERT OR REPLACE INTO ""DatabaseInfo"" (""Id"", ""Version"") VALUES (1, @version);";
            using (var statement = transaction.Prepare(sql))
            {
                statement.Bind("@version", DbVersion.ToString());

                bool success = transaction.Execute(statement);
                if (!success)
                {
                    throw new Exception(string.Format("Failed to update database version to {0}", DbVersion));
                }
            }
        }
    }
}