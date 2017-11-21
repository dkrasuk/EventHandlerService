using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EventHandlerService.DataAccess.StagingDB
{
    public static class StagingDAL
    {
        private static OracleConnection connection = null;
        public static bool IsNotified;      

        public static OracleDependency RegistrationDependencyNotification()
        {
            try
            {
                const string query = @"select * from transit_collateral";
                connection = DAL.GetStagingConnection();
                OracleCommand cmd = new OracleCommand(query, connection);
                connection.Open();
                OracleDependency.Port = 1006;
                //   OracleDependency dependency = new OracleDependency(cmd, false, 300, false);
                var dependency = new OracleDependency(cmd);
                cmd.Notification.IsNotifiedOnce = false;
                cmd.Notification.Timeout = 300;

                cmd.ExecuteNonQuery();
                return dependency;
            }
            catch (Exception e)
            {
                var error = e.Message;
                return null;
            }
            finally
            {
                if (connection !=null)
                {
                    connection.Close();
                    connection.Dispose();
                }
              
            }           

        }

        //public static void InsertCollaterals()
        //{
        //    const string queryInsert = @"
        //                            begin
        //                            insert into target_transit_collateral
        //                            select *
        //                            from transit_collateral 
        //                            where  rownum <=5;
        //                            commit;
        //                            end;
        //                            ";
        //    if (connection!=null)
        //    {
        //        connection = DAL.GetStagingConnection();
        //        OracleCommand cmd = new OracleCommand(queryInsert, connection);
        //        connection.Open();
        //        cmd.ExecuteNonQuery();
        //    }

        //    connection.Close();
        //    connection.Dispose();
        //}

        public static bool InsertCollaterals()
        {
            const string queryInsert = @"
                                    begin
                                    insert into target_transit_collateral 
                                    select *
                                    from transit_collateral tc
                                    where not exists (
                                                  select * 
                                                  from target_transit_collateral ttc
                                                  where standard_hash(to_char(tc.rdate || tc.agreemid || tc.insuranceobjectid || tc.colltypeid || tc.cost || tc.currencyid || tc.collagreemid || tc.linked_amount || tc.linked_percent || tc.mastersystemid),'MD5' ) 
                                                  = 
                                                  standard_hash(to_char(ttc.rdate || ttc.agreemid || ttc.insuranceobjectid || ttc.colltypeid || ttc.cost || ttc.currencyid || ttc.collagreemid || ttc.linked_amount || ttc.linked_percent || ttc.mastersystemid),'MD5' )              
                                                  ); 
                                    commit;
                                    end;
                                    ";
            DAL.Execute(DAL.GetUnitOfWorkStaging(), queryInsert, null);
            return true;
        }

        public static bool UpdateCollaterals()
        {
            const string queryUpdate = @"
                                    begin
                                    declare
                                    cursor tmp IS
                                    select tc.* from transit_collateral tc
                                    where not exists (
                                                  select * 
                                                  from target_transit_collateral ttc
                                                  where standard_hash(to_char(tc.rdate || tc.agreemid || tc.insuranceobjectid || tc.colltypeid || tc.cost || tc.currencyid || tc.collagreemid || tc.linked_amount || tc.linked_percent || tc.mastersystemid),'MD5' ) = standard_hash(to_char(ttc.rdate || ttc.agreemid || ttc.insuranceobjectid || ttc.colltypeid || ttc.cost || ttc.currencyid || ttc.collagreemid || ttc.linked_amount || ttc.linked_percent || ttc.mastersystemid),'MD5' )              
                                                  );
                                    begin
                                      for agr in tmp loop
                                        update target_transit_collateral ttc
                                        set ttc.rdate = agr.rdate,
                                            ttc.agreemid = agr.agreemid,
                                            ttc.insuranceobjectid = agr.insuranceobjectid,
                                            ttc.colltypeid = agr.colltypeid,
                                            ttc.cost = agr.cost,
                                            ttc.currencyid = agr.currencyid,
                                            ttc.collagreemid = agr.collagreemid,
                                            ttc.linked_amount = agr.linked_amount,
                                            ttc.linked_percent = agr.linked_percent,
                                            ttc.mastersystemid = agr.mastersystemid        
                                        where ttc.agreemid = agr.agreemid
                                        and ttc.collagreemid = agr.collagreemid;       
                                        end loop;
                                        end;      
                                     commit;
                                     end;
                                        ";
            DAL.Execute(DAL.GetUnitOfWorkStaging(), queryUpdate, null);
            return true;
        }

        public static bool DeleteCollaterals()
        {
            const string queryDelete = @"
                                    begin
                                    delete  from target_transit_collateral ttc
                                    where ttc.collagreemid in (
                                                  select ttc.collagreemid from target_transit_collateral ttc
                                                  left join transit_collateral tc
                                                  on tc.agreemid = ttc.agreemid and
                                                   tc.collagreemid = ttc.collagreemid   
                                                   where tc.agreemid is null
                                                   and tc.collagreemid is null         
                                                  );   
                                        commit;
                                        end;

                                            ";
            DAL.Execute(DAL.GetUnitOfWorkStaging(), queryDelete, null);
            return true;
        }

    }
}
