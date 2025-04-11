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
    public class DCCABLEGojangRepository
    {
        public Result GetDCCABLEFHByDCCABLECode(string dccableCode, out List<DCCABLEFailureHistory> dccableFHList)
        {
            Result res = new Result(true);
            dccableFHList = new List<DCCABLEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM DCCABLE_FAILURE_HISTORY 
                WHERE DCCABLE_CODE = @DCCABLE_Code";

                    dccableFHList = dbHelper.Conn.Query<DCCABLEFailureHistory>(query, new { DCCABLE_Code = dccableCode }).AsList();
                    res.Message = $"GetDCCABLEChkByDCCABLECode 성공: DCCABLE_CODE = {dccableCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCABLEFHByDCCABLECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCABLE_FAILURE_HISTORY)", res.Message);
            }
            return res;
        }
        // 전체 DCCABLE 고장이력 데이터 조회
        public Result GetTotalDCCABLEGojang(out List<DCCABLEFailureHistory> dccableGojangList)
        {
            Result res = new Result(true);
            dccableGojangList = new List<DCCABLEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCABLE_FAILURE_HISTORY";
                    dccableGojangList = dbHelper.Conn.Query<DCCABLEFailureHistory>(query).AsList();
                }
                res.Message = $"GetTotalDCCABLEGojang 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCABLEGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCABLE_FAILURE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetDCCABLEFHDetailByDCCABLECode(string dccableCode, string gojangName, out List<DCCABLEFailureHistory> dccableFHList)
        {
            Result res = new Result(true);
            dccableFHList = new List<DCCABLEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DCCABLE_FAILURE_HISTORY 
                WHERE DCCABLE_CODE = @DCCABLE_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    dccableFHList = dbHelper.Conn.Query<DCCABLEFailureHistory>(query, new { DCCABLE_Code = dccableCode, Fail_Gojang_Name = gojangName }).AsList();
                    if (dccableFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCABLEFHDetailByDCCABLECode 성공: DCCABLE_CODE = {dccableCode}, FAIL_GOJANG_NAME = {gojangName}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCABLEFHDetailByDCCABLECode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateDCCABLEFHRepo(DCCABLEFailureHistory dccableFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
        INSERT INTO DCCABLE_FAILURE_HISTORY (
        DCCABLE_CODE, FAIL_GOJANG_NAME, FAIL_WEATHER, FAIL_TEMP, FAIL_HUM, FAIL_CAUSE, FAIL_REASON, FAIL_STATUS, FAIL_PART, FAIL_PERIOD, FAIL_FINDER, FAIL_REPAIRER, FAIL_SUPERVISOR, FAIL_REPAIR_DATE, FAIL_WRITER 
        ) VALUES (@DCCABLE_Code, @Fail_Gojang_Name, @Fail_Weather, @Fail_Temp, @Fail_Hum, @Fail_Cause, @Fail_Reason, @Fail_Status, @Fail_Part, @Fail_Period, @Fail_Finder, @Fail_Repairer, @Fail_Supervisor, @Fail_Repair_Date, @Fail_Writer)";

                    int affectedRows = dbHelper.Conn.Execute(query, dccableFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCABLE 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCABLE 고장이력 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDCCABLEFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDCCABLEFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // DCCABLE 고장이력 데이터 업데이트
        public Result UpdateDCCABLEFHRepo(DCCABLEFailureHistory dccableFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DCCABLE_FAILURE_HISTORY
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
                WHERE DCCABLE_CODE = @DCCABLE_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, dccableFH);
                    res.Message = affectedRows > 0 ? "DCCABLE 고장이력 데이터 업데이트 성공" : "DCCABLE 고장이력 데이터 업데이트 실패";
                }
            }

            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCABLEFHRepo 실패: {ex.Message}";
            }
            return res;
        }

        // DCCABLE 고장이력 데이터 삭제
        public Result DeleteDCCABLEFHRepo(string dccableCode, string gojangName)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DCCABLE_FAILURE_HISTORY WHERE DCCABLE_CODE = @DCCABLE_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCABLE_Code = dccableCode, FAIL_Gojang_Name = gojangName });
                    res.Message = affectedRows > 0 ? "DCCABLE 고장이력 데이터 삭제 성공" : "DCCABLE 고장이력 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCABLEFHRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

