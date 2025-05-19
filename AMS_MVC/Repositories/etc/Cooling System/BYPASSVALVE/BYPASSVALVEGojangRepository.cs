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
    public class BYPASSVALVEGojangRepository
    {
        /// <summary>
        /// 특정 BYPASSVALVE_Code에 대한 고장이력(BYPASSVALVE_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetBYPASSVALVEFHByBYPASSVALVECode(string bypassvalveCode, out List<BYPASSVALVEFailureHistory> bypassvalveFHList)
        {
            Result res = new Result(true);
            bypassvalveFHList = new List<BYPASSVALVEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM BYPASSVALVE_FAILURE_HISTORY
                        WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code";

                    bypassvalveFHList = dbHelper.Conn
                        .Query<BYPASSVALVEFailureHistory>(query, new { BYPASSVALVE_Code = bypassvalveCode })
                        .AsList();

                    res.Message = $"GetBYPASSVALVEFHByBYPASSVALVECode 성공: BYPASSVALVE_CODE = {bypassvalveCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetBYPASSVALVEFHByBYPASSVALVECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(BYPASSVALVE_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 BYPASSVALVE 고장이력 목록 조회
        /// </summary>
        public Result GetTotalBYPASSVALVEGojang(out List<BYPASSVALVEFailureHistory> bypassvalveGojangList)
        {
            Result res = new Result(true);
            bypassvalveGojangList = new List<BYPASSVALVEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM BYPASSVALVE_FAILURE_HISTORY";
                    bypassvalveGojangList = dbHelper.Conn
                        .Query<BYPASSVALVEFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalBYPASSVALVEGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalBYPASSVALVEGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(BYPASSVALVE_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// BYPASSVALVE_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetBYPASSVALVEFHDetailByBYPASSVALVECode(string bypassvalveCode, string tblIdx, out List<BYPASSVALVEFailureHistory> bypassvalveFHList)
        {
            Result res = new Result(true);
            bypassvalveFHList = new List<BYPASSVALVEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM BYPASSVALVE_FAILURE_HISTORY
                        WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code
                          AND TBL_IDX = @Tbl_Idx";

                    bypassvalveFHList = dbHelper.Conn
                        .Query<BYPASSVALVEFailureHistory>(query, new { BYPASSVALVE_Code = bypassvalveCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (bypassvalveFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetBYPASSVALVEFHDetailByBYPASSVALVECode 성공: BYPASSVALVE_CODE = {bypassvalveCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetBYPASSVALVEFHDetailByBYPASSVALVECode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateBYPASSVALVEFHRepo(BYPASSVALVEFailureHistory bypassvalveFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO BYPASSVALVE_FAILURE_HISTORY (
                            BYPASSVALVE_CODE,
                            FAIL_GOJANG_NAME,
                            FAIL_WEATHER,
                            FAIL_TEMP,
                            FAIL_HUM,
                            FAIL_CAUSE,
                            FAIL_REASON,
                            FAIL_STATUS,
                            FAIL_PART,
                            FAIL_PERIOD,
                            FAIL_FINDER,
                            FAIL_REPAIRER,
                            FAIL_SUPERVISOR,
                            FAIL_REPAIR_DATE,
                            FAIL_WRITER
                        ) VALUES (
                            @BYPASSVALVE_Code,
                            @Fail_Gojang_Name,
                            @Fail_Weather,
                            @Fail_Temp,
                            @Fail_Hum,
                            @Fail_Cause,
                            @Fail_Reason,
                            @Fail_Status,
                            @Fail_Part,
                            @Fail_Period,
                            @Fail_Finder,
                            @Fail_Repairer,
                            @Fail_Supervisor,
                            @Fail_Repair_Date,
                            @Fail_Writer
                        )";

                    int affectedRows = dbHelper.Conn.Execute(query, bypassvalveFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "BYPASSVALVE 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "BYPASSVALVE 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateBYPASSVALVEFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateBYPASSVALVEFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateBYPASSVALVEFHRepo(BYPASSVALVEFailureHistory bypassvalveFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE BYPASSVALVE_FAILURE_HISTORY
                        SET
                            FAIL_GOJANG_NAME = @Fail_Gojang_Name,
                            FAIL_WEATHER = @Fail_Weather,
                            FAIL_TEMP = @Fail_Temp,
                            FAIL_HUM = @Fail_Hum,
                            FAIL_REASON = @Fail_Reason,
                            FAIL_STATUS = @Fail_Status,
                            FAIL_PART = @Fail_Part,
                            FAIL_PERIOD = @Fail_Period,
                            FAIL_FINDER = @Fail_Finder,
                            FAIL_REPAIRER = @Fail_Repairer,
                            FAIL_SUPERVISOR = @Fail_Supervisor,
                            FAIL_REPAIR_DATE = @Fail_Repair_Date,
                            FAIL_CAUSE = @Fail_Cause,
                            FAIL_WRITER = @Fail_Writer
                        WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, bypassvalveFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "BYPASSVALVE 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "BYPASSVALVE 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateBYPASSVALVEFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteBYPASSVALVEFHRepo(string bypassvalveCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM BYPASSVALVE_FAILURE_HISTORY
                        WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { BYPASSVALVE_Code = bypassvalveCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "BYPASSVALVE 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "BYPASSVALVE 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteBYPASSVALVEFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
