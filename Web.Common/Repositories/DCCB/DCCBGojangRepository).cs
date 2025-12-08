
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCBGojangRepository
    {
        public Result GetDCCBFHByDCCBCode(string dccbCode, out List<DCCBFailureHistory> dccbFHList)
        {
            Result res = new Result(true);
            dccbFHList = new List<DCCBFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM DCCB_FAILURE_HISTORY 
                WHERE DCCB_CODE = @DCCB_Code";

                    dccbFHList = dbHelper.Conn.Query<DCCBFailureHistory>(query, new { DCCB_Code = dccbCode }).AsList();
                    res.Message = $"GetDCCBChkByDCCBCode 성공: DCCB_CODE = {dccbCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCBFHByDCCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCB_FAILURE_HISTORY)", res.Message);
            }
            return res;
        }
        // 전체 DCCB 고장이력 데이터 조회
        public Result GetTotalDCCBGojang(out List<DCCBFailureHistory> dccbGojangList)
        {
            Result res = new Result(true);
            dccbGojangList = new List<DCCBFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCB_FAILURE_HISTORY";
                    dccbGojangList = dbHelper.Conn.Query<DCCBFailureHistory>(query).AsList();
                }
                res.Message = $"GetTotalDCCBGojang 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCBGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCB_FAILURE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetDCCBFHDetailByDCCBCode(string dccbCode, string gojangName, out List<DCCBFailureHistory> dccbFHList)
        {
            Result res = new Result(true);
            dccbFHList = new List<DCCBFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DCCB_FAILURE_HISTORY 
                WHERE DCCB_CODE = @DCCB_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    dccbFHList = dbHelper.Conn.Query<DCCBFailureHistory>(query, new { DCCB_Code = dccbCode, Fail_Gojang_Name = gojangName }).AsList();
                    if (dccbFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCBFHDetailByDCCBCode 성공: DCCB_CODE = {dccbCode}, FAIL_GOJANG_NAME = {gojangName}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCBFHDetailByDCCBCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateDCCBFHRepo(DCCBFailureHistory dccbFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
        INSERT INTO DCCB_FAILURE_HISTORY (
        DCCB_CODE, FAIL_GOJANG_NAME, FAIL_WEATHER, FAIL_TEMP, FAIL_HUM, FAIL_CAUSE, FAIL_REASON, FAIL_STATUS, FAIL_PART, FAIL_PERIOD, FAIL_FINDER, FAIL_REPAIRER, FAIL_SUPERVISOR, FAIL_REPAIR_DATE, FAIL_WRITER 
        ) VALUES (@DCCB_Code, @Fail_Gojang_Name, @Fail_Weather, @Fail_Temp, @Fail_Hum, @Fail_Cause, @Fail_Reason, @Fail_Status, @Fail_Part, @Fail_Period, @Fail_Finder, @Fail_Repairer, @Fail_Supervisor, @Fail_Repair_Date, @Fail_Writer)";

                    int affectedRows = dbHelper.Conn.Execute(query, dccbFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCB 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCB 고장이력 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDCCBFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDCCBFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // DCCB 고장이력 데이터 업데이트
        public Result UpdateDCCBFHRepo(DCCBFailureHistory dccbFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DCCB_FAILURE_HISTORY
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
                WHERE DCCB_CODE = @DCCB_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, dccbFH);
                    res.Message = affectedRows > 0 ? "DCCB 고장이력 데이터 업데이트 성공" : "DCCB 고장이력 데이터 업데이트 실패";
                }
            }

            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCBFHRepo 실패: {ex.Message}";
            }
            return res;
        }

        // DCCB 고장이력 데이터 삭제
        public Result DeleteDCCBFHRepo(string dccbCode, string gojangName)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DCCB_FAILURE_HISTORY WHERE DCCB_CODE = @DCCB_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCB_Code = dccbCode, FAIL_Gojang_Name = gojangName });
                    res.Message = affectedRows > 0 ? "DCCB 고장이력 데이터 삭제 성공" : "DCCB 고장이력 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCBFHRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

