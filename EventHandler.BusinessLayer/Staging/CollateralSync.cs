using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHandlerService.DataAccess.StagingDB;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading;

namespace EventHandler.BusinessLayer.Staging
{
    public class CollateralSync
    {
        public static bool IsNotified;
        public static void StartSyncCollateral()
        {
            var dependency = StagingDAL.RegistrationDependencyNotification();
            dependency.OnChange += OnMyNotification;
            while (IsNotified == false)
            {
                Thread.Sleep(3000);
            }

        }

        private static void OnMyNotification(object sender, OracleNotificationEventArgs eventArgs)
        {
            var operationType = eventArgs.Info.ToString().ToUpper();

            switch (operationType)
            {
                case "INSERT":
                    StagingDAL.InsertCollaterals();
                    break;
                case "UPDATE":
                    StagingDAL.UpdateCollaterals();
                    break;
                case "DELETE":
                    StagingDAL.DeleteCollaterals();
                    break;                    
            }
        }
    }
}
