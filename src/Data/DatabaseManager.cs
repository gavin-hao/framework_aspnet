using System;
using System.Collections.Generic;
using System.Text;
using Dongbo.Configuration;
using Dongbo.Configuration.Logging;

namespace Dongbo.Data
{
    internal class DatabaseManager
    {
        private static DatabaseManager instance = new DatabaseManager();

        private DatabaseCollection databases;
        string sharedDbInstance = string.Empty;
        string securityTrackingDbInstance = string.Empty;

        internal DatabaseManager()
        {
            databases = new DatabaseCollection();
            ConnectionStringCollection.RegisterConfigChangedNotification(OnConfigChanged);
        }

        void OnConfigChanged(object sender, EventArgs args)
        {
            ConnectionStringCollection collection = (ConnectionStringCollection)sender;
            foreach (string instanceName in collection)
            {
                string connString = collection[instanceName];
                if (databases.Contains(instanceName))
                {
                    Database db = databases[instanceName];
                    db.SetConnectionString(connString);
                }
            }
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        internal static DatabaseManager Instance
        {
            get { return instance; }
        }

        internal Database GetDatabase(string instanceName)
        {
            if (!databases.Contains(instanceName))
            {
                lock (databases)
                {
                    Database database = new Database(instanceName);

                    if (!databases.Contains(instanceName))
                    {
                        databases.Add(database);
                    }
                }
            }

            return databases[instanceName];
        }

       

        //internal Database GetSecurityTracking(int userId)
        //{
        //    if (securityTrackingDbInstance != string.Empty)
        //        return GetDatabase(securityTrackingDbInstance);

        //    securityTrackingDbInstance = DatabaseInstanceNameProvider.GetSecurityTrackingDb(userId);
        //    return GetDatabase(securityTrackingDbInstance);
        //}

        //internal Database GetSharedMaster()
        //{
        //    return GetDatabase(DatabaseInstance.SharedMaster);
        //}   

       

        internal DatabaseCollection Databases
        {
            get { return databases; }
        }

        internal void ResetShared()
        {
            sharedDbInstance = string.Empty;
        }

        internal void ResetSecurityTracking()
        {
            securityTrackingDbInstance = string.Empty;
        }
    }
}