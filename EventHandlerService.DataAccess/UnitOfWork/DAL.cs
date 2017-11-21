using AlfaBank.Logger;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using EventHandlerService.DataAccess.StagingDB;
using System.Data;

namespace EventHandlerService.DataAccess
{
    public static class DAL
    {
        public static T Execute<T>(IUnitOfWork unitOfWork, string query, string source, params object[] parameters)
        {

            try
            {
                using (unitOfWork)
                {
                    var result = unitOfWork.ExecuteQuery<T>(query,
                    parameters).First();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        public static void Execute(IUnitOfWork unitOfWork, string query, string source, params object[] parameters)
        {

            try
            {
                using (unitOfWork)
                {
                    unitOfWork.ExecuteQuery(query, parameters);
                }
            }
            catch (Exception ex)
            {
                //LogException(source, ex);

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unitOfWork"></param>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>                 

        public static IEnumerable<T> ExecuteArray<T>(IUnitOfWork unitOfWork, string query, string source, params object[] parameters)
        {
            try
            {
                using (unitOfWork)
                {
                    var result = unitOfWork.ExecuteQuery<T>(query,
                    parameters).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unitOfWork"></param>
        /// <param name="query"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteArray<T>(IUnitOfWork unitOfWork, string query, string source)
        {
            try
            {
                using (unitOfWork)
                {
                    var result = unitOfWork.ExecuteQuery<T>(query).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IUnitOfWork GetUnitOfWorkStaging()
        {
            OracleConnection db = new OracleConnection(ConfigurationManager.ConnectionStrings["Staging"].ConnectionString);
            var daf = new DataAccessFactory<StagingDbContext>(db, false, "Staging");            
            var uof = daf.CreateUnitOfWork();
            return uof;
        }
        /// <summary>
        /// GetStagingConnection
        /// </summary>
        /// <returns></returns>
        public static OracleConnection GetStagingConnection()
        {
            var conStr = ConfigurationManager.ConnectionStrings["Staging"].ConnectionString;
            OracleConnection db = new OracleConnection(conStr);
            return db;
        }
    }
}
