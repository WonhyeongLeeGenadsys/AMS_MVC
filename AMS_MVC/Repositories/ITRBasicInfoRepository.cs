using AMS_MVC.Database;
using AMS_MVC.Models;
using AMS_MVC.Utlity;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Common.Log;

namespace AMS_MVC.Repositories
{
    public class ITRBasicInfoRepository
    {
        private readonly DBHelper mDb;

        public ITRBasicInfoRepository()
        {
            mDb = new DBHelper();
        }
        // 가장 큰 Interface_CODE 값을 반환
        public string GetLatestITRCode()
        {
            var query = "SELECT MAX(ITR_CODE) FROM INTERFACETR_BASICINFO WHERE ITR_CODE LIKE 'I%'";
            return mDb.Conn.QuerySingleOrDefault<string>(query);
        }

        public ITRBasicInfo GetITRBasicInfoByTblIdxRepo(string tblIdx)
        {
            var query = "SELECT * FROM INTERFACETR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

            return mDb.Conn.QueryFirstOrDefault<ITRBasicInfo>(query, new { Tbl_Idx = tblIdx });
        }

        public Result GetAllITRBasicInfoRepo(out List<ITRBasicInfo> interfaceTrBasicInfo)
        {
            Result res = new Result(true);
            interfaceTrBasicInfo = new List<ITRBasicInfo>();

            try
            {
                var query = "SELECT TBL_IDX, ITR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, INSTALL_PLACE, RATED_V, RATED_A, CONSTANT, MAKE_COMPANY, MAKE_NO, IS_DIAGNOSTICS, IS_HEALTH, WRITER FROM INTERFACETR_BASICINFO";
                interfaceTrBasicInfo = mDb.Conn.Query<ITRBasicInfo>(query).AsList();

                LogHelper.WriteLog("InterfaceTrBasicInfo Data", $"{interfaceTrBasicInfo}");
                res.Message = "GetAllInterfaceTrBasicInfoRepo 동작 성공";
                LogHelper.WriteLog("DB(INTERFACETR_BASICINFO", res.Message);
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllInterfaceTrBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;

                LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result CreateITRBasicInfoRepo(ITRBasicInfo newITRBasicInfo)
        {
            Result res = new Result(true);
            using (var conn = mDb.Conn)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var queryITRBasicInfo = @"
              INSERT INTO INTERFACETR_BASICINFO (ITR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
              INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
              IS_HEALTH, WRITER) 
              VALUES (@ITR_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
              @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
              @Is_Health, @Writer)";

                        int affectedRowsITRBasicInfo = mDb.Conn.Execute(queryITRBasicInfo, newITRBasicInfo, transaction);

                        if (affectedRowsITRBasicInfo > 0)
                        {
                            //RISKMATRIX 테이블에 데이터 삽입
                            var queryRiskMatrix = @"
                            INSERT INTO RISKMATRIX (CODE, COF, POF)
                            VALUES (@ITR_Code, @DefaultCof, @DefaultPof)";

                            var riskMatrixData = new
                            {
                                ITR_Code = newITRBasicInfo.ITR_Code,
                                DefaultCof = "0",
                                DefaultPof = "0"
                            };

                            int affetedFowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                            if (affetedFowsRiskMatrix > 0)
                            {
                                transaction.Commit();
                                res.Message = "CreateInterfaceTrBasicInfoRepo 성공: ITR Serial_No: " + newITRBasicInfo.Serial_No;
                                LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
                            }
                            else
                            {
                                throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                            }
                        }
                        else
                        {
                            throw new Exception("INTERFACETR_BASICINFO 테이블에 데이터 삽입 실패");

                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        res.IsSuccess = false;
                        res.Message = "CreateInterfaceTrBasicInfoRepo 실패: " + ex.Message;
                        LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", "CreateInterfaceTrBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                    }
                }
            }
            return res;
        }

        public Result UpdateITRBasicInfoRepo(ITRBasicInfo interfaceTrBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                var query = "UPDATE INTERFACETR_BASICINFO SET NAME = @Name, INSTALL_DATE = @Install_Date, OPERATING_DATE = @Operating_Date, PRICE=@Price, INSTALL_PLACE=@Install_Place, CAPACITY=@Capacity, RATED_V=@Rated_V, RATED_A=@Rated_A, MAKE_COMPANY=@Make_Company, MAKE_NO=@Make_No, PHOTO=@Photo, IS_DIAGNOSTICS=@Is_Diagnostics, IS_HEALTH=@Is_Health, WRITER=@Writer " +
                            "WHERE SERIAL_NO = @Serial_No";

                int affectedRows = mDb.Conn.Execute(query, interfaceTrBasicInfo);
                if (affectedRows > 0)
                {
                    res.Message = "UpdateInterfaceTrBasicInfoRepo 성공 SERIAL_NO: " + interfaceTrBasicInfo.Serial_No;
                    LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "UpdateInterfaceTrBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                    LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateInterfaceTrBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteITRBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                var query = "DELETE FROM INTERFACETR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

                int affectedRows = mDb.Conn.Execute(query, new { Tbl_Idx = tblIdx });
                if (affectedRows > 0)
                {
                    res.Message = "DeleteInterfaceTrBasicInfoRepo 성공: ITRBasicInfo Tbl_Idx: " + tblIdx;
                    LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "DeleteInterfaceTrBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                    LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteInterfaceTrBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(INTERFACETR_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}