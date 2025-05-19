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
    public class ZIGZAGTRGojangRepository
    {
        /// <summary>
        /// 특정 ZIGZAGTR_Code에 대한 고장이력(ZIGZAGTR_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetZIGZAGTRFHByZIGZAGTRCode(string zigzagtrCode, out List<ZIGZAGTRFailureHistory> zigzagtrFHList)
        {
            Result res = new Result(true);
            zigzagtrFHList = new List<ZIGZAGTRFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM ZIGZAGTR_FAILURE_HISTORY
                        WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code";

                    zigzagtrFHList = dbHelper.Conn
                        .Query<ZIGZAGTRFailureHistory>(query, new { ZIGZAGTR_Code = zigzagtrCode })
                        .AsList();

                    res.Message = $"GetZIGZAGTRFHByZIGZAGTRCode 성공: ZIGZAGTR_CODE = {zigzagtrCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetZIGZAGTRFHByZIGZAGTRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ZIGZAGTR_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 ZIGZAGTR 고장이력 목록 조회
        /// </summary>
        public Result GetTotalZIGZAGTRGojang(out List<ZIGZAGTRFailureHistory> zigzagtrGojangList)
        {
            Result res = new Result(true);
            zigzagtrGojangList = new List<ZIGZAGTRFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ZIGZAGTR_FAILURE_HISTORY";
                    zigzagtrGojangList = dbHelper.Conn
                        .Query<ZIGZAGTRFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalZIGZAGTRGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalZIGZAGTRGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ZIGZAGTR_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// ZIGZAGTR_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetZIGZAGTRFHDetailByZIGZAGTRCode(string zigzagtrCode, string tblIdx, out List<ZIGZAGTRFailureHistory> zigzagtrFHList)
        {
            Result res = new Result(true);
            zigzagtrFHList = new List<ZIGZAGTRFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM ZIGZAGTR_FAILURE_HISTORY
                        WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    zigzagtrFHList = dbHelper.Conn
                        .Query<ZIGZAGTRFailureHistory>(query, new { ZIGZAGTR_Code = zigzagtrCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (zigzagtrFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetZIGZAGTRFHDetailByZIGZAGTRCode 성공: ZIGZAGTR_CODE = {zigzagtrCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetZIGZAGTRFHDetailByZIGZAGTRCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateZIGZAGTRFHRepo(ZIGZAGTRFailureHistory zigzagtrFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO ZIGZAGTR_FAILURE_HISTORY (
                            ZIGZAGTR_CODE,
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
                            @ZIGZAGTR_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, zigzagtrFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "ZIGZAGTR 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ZIGZAGTR 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateZIGZAGTRFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateZIGZAGTRFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateZIGZAGTRFHRepo(ZIGZAGTRFailureHistory zigzagtrFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE ZIGZAGTR_FAILURE_HISTORY
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
                        WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, zigzagtrFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "ZIGZAGTR 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ZIGZAGTR 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateZIGZAGTRFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteZIGZAGTRFHRepo(string zigzagtrCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM ZIGZAGTR_FAILURE_HISTORY
                        WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ZIGZAGTR_Code = zigzagtrCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "ZIGZAGTR 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ZIGZAGTR 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteZIGZAGTRFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
