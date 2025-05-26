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
    public class DSBasicInfoRepository
    {
        // 가장 큰 DS_CODE 값을 반환
        public string GetLatestDSCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(DS_CODE) FROM DS_BASICINFO WHERE DS_CODE LIKE 'DS%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public DSBasicInfo GetDSBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DS_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<DSBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public DSBasicInfo GetDSBasicInfoByCode(string dsCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DS_BASICINFO WHERE DS_CODE = @DS_Code";
                return dbHelper.Conn.QueryFirstOrDefault<DSBasicInfo>(query, new { DS_Code = dsCode });
            }
        }

        /// <summary>
        /// DS 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllDSBasicInfoRepo(out List<DSBasicInfo> dsBasicInfo)
        {
            Result res = new Result(true);
            dsBasicInfo = new List<DSBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, DS_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM DS_BASICINFO";
                    dsBasicInfo = dbHelper.Conn.Query<DSBasicInfo>(query).AsList();

                    LogHelper.WriteLog("dsBasicInfo Data", $"총 {dsBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllDSBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllDSBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
            }
            return res;
        }

        //DS Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllDSBasicInfoWithRiskMatrixRepo(out List<dynamic> dsInfoWithRisk)
        {
            Result res = new Result(true);
            dsInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.DS_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life,
                    r.HI
                FROM DS_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.DS_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    dsInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "DS 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateDSBasicInfoRepo(DSBasicInfo newDSBasicInfo)
        {
            Result res = new Result(true);
            using (DBHelper dbHelper = new DBHelper())
            {
                using (var conn = dbHelper.Conn)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // DS_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO DS_BASICINFO (DS_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, REMAIN_LIFE, WRITER) 
                VALUES (@DS_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newDSBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@DS_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    DS_Code = newDSBasicInfo.DS_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateDSBasicInfoRepo 성공: DS Serial_No: " + newDSBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("DS_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateDSBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(DS_BASICINFO)", "CreateDSBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateDSBasicInfoRepo(DSBasicInfo dsBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // DS_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE DS_BASICINFO
            SET 
                NAME = @Name, 
                INSTALL_DATE = @Install_Date, 
                OPERATING_DATE = @Operating_Date, 
                PRICE = @Price, 
                INSTALL_PLACE = @Install_Place, 
                CAPACITY = @Capacity, 
                RATED_V = @Rated_V, 
                RATED_A = @Rated_A, 
                MAKE_COMPANY = @Make_Company, 
                MAKE_NO = @Make_No, 
                PHOTO = @Photo, 
                IS_DIAGNOSTICS = @Is_Diagnostics, 
                IS_HEALTH = @Is_Health,
                REMAIN_LIFE = @Remain_Life,
                WRITER = @Writer
            WHERE DS_CODE = @DS_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, dsBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateDSBasicInfoRepo 성공. DS_CODE: " + dsBasicInfo.DS_Code;
                        LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateDSBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateDSBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteDSBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM DS_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteDSBasicInfoRepo 성공: DSBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteDSBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteDSBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(DS_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
