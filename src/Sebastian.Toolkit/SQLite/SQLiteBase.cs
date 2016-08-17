using System;
using System.Collections.Generic;
using System.Linq;
using Sebastian.Toolkit.Logging;
using SQLitePCL;

namespace Sebastian.Toolkit.SQLite
{
    public abstract class SQLiteBase
    {
        private ISQLiteConnection _connection;
        private bool _disposed;
        private readonly object _connectionLock = new object();

        protected abstract string DatabaseFilename { get; }
        protected abstract IEnumerable<DatabaseVersion> DatabaseVersions { get; }

        protected SQLiteBase()
        {
            Initialize();
            UpgradeDatabase();
        }

        protected bool HasUpgraded { get; set; }
        protected Version CurrentDatabaseVersion { get; set; }

        protected ISQLiteConnection SharedConnection
        {
            get
            {
                if (_connection == null)
                {
                    lock (_connectionLock)
                    {
                        if (_connection == null)
                        {
                            _connection = new SQLiteConnection(DatabaseFilename);
                            var pragma = _connection.Prepare(@"PRAGMA foreign_keys = ON;");
                            pragma.Step();
                        }
                    }
                }

                return _connection;
            }
            set { _connection = value; }
        }

        private void Initialize()
        {
            using (var dbExistsStatement = SharedConnection.Prepare("SELECT 1 FROM sqlite_master WHERE type='table' AND name='DatabaseInfo'"))
            {
                if (dbExistsStatement.Step() == SQLiteResult.ROW)
                {
                    const string sql = @"SELECT ""Version"" FROM DatabaseInfo LIMIT 1;";
                    using (var statement = SharedConnection.Prepare(sql))
                    {
                        statement.Step();
                        CurrentDatabaseVersion = new Version(statement.GetText("Version"));
                    }
                }
                else
                {
                    var versionZero = new VersionZero();
                    using (var transaction = new SQLiteTransaction(SharedConnection))
                    {
                        versionZero.UpgradeAndUpdateVersion(transaction);
                        transaction.Commit();
                    }
                    CurrentDatabaseVersion = versionZero.DbVersion;
                }
            }

            HasUpgraded = UpgradeDatabase();
            if (HasUpgraded)
            {
                CloseConnection();
            }
        }

        public void CloseConnection()
        {
            if (_connection != null)
            {
                lock (_connectionLock)
                {
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
            }
        }

        protected bool UpgradeDatabase()
        {
            var versionBeforeUpgrades = CurrentDatabaseVersion;
            foreach (var version in DatabaseVersions.Where(version => CurrentDatabaseVersion < version.DbVersion).ToList())
            {
                try
                {
                    using (var transaction = new SQLiteTransaction(SharedConnection))
                    {
                        version.UpgradeAndUpdateVersion(transaction);
                        transaction.Commit();
                    }

                    this.Logger().Debug("Database upgrade from {0} to {1} succeeded.", CurrentDatabaseVersion, version.DbVersion);
                    CurrentDatabaseVersion = version.DbVersion;
                }
                catch (Exception exception)
                {
                    this.Logger().Error(string.Format("Database upgrade from {0} to {1} failed.", CurrentDatabaseVersion, version.DbVersion), exception);
                }
            }

            bool hasUpgraded = versionBeforeUpgrades != CurrentDatabaseVersion;
            return hasUpgraded;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                CloseConnection();
                _disposed = true;
            }
        }
    }
}