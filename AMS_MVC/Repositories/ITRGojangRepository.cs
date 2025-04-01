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
    public class ITRGojangRepository
    {
        public Result GetITRFHByITRCode(string itrCode, out List<ITRFailureHistory> itrGojnagList)
        {
            Result res = new Result(true);
            itrGojnagList = new List<ITRFailureHistory>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM INTERFACETR_FAILURE_HISTORY 
                WHERE ITR_CODE = @ITR_Code";

                    itrGojnagList = dbHelper.Conn.Query<ITRFailureHistory>(query, new { ITR_Code = itrCode }).AsList();
                    res.Message = $"GetITRChkByVCBCode 성공: ITR_CODE = {itrCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRFHByITRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_FAILURE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 ITR 고장이력 데이터 조회
        public Result GetTotalITRGojang(out List<ITRFailureHistory> itrGojangList)
        {
            Result res = new Result(true);
            itrGojangList = new List<ITRFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM INTERFACETR_FAILURE_HISTORY";
                    itrGojangList = dbHelper.Conn.Query<ITRFailureHistory>(query).AsList();
                }
                res.Message = $"GetTotalITRGojang 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalITRGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_FAILURE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetITRFHDetailByITRCode(string itrCode, string gojangName, out List<ITRFailureHistory> itrGojangList)
        {
            Result res = new Result(true);
            itrGojangList = new List<ITRFailureHistory>();
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM INTERFACETR_FAILURE_HISTORY 
                WHERE ITR_CODE = @ITR_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    itrGojangList = dbHelper.Conn.Query<ITRFailureHistory>(query, new { ITR_Code = itrCode, Fail_Gojang_Name = gojangName }).AsList();
                    if (itrGojangList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetITRFHDetailByITRCode 성공: ITR_CODE = {itrCode}, FAIL_GOJANG_NAME = {gojangName}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRFHDetailByITRCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateITRFHRepo(ITRFailureHistory itrGojangList)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
        INSERT INTO INTERFACETR_FAILURE_HISTORY (
        ITR_CODE, FAIL_GOJANG_NAME, FAIL_WEATHER, FAIL_TEMP, FAIL_HUM, FAIL_CAUSE, FAIL_REASON, FAIL_STATUS, FAIL_PART, FAIL_PERIOD, FAIL_FINDER, FAIL_REPAIRER, FAIL_SUPERVISOR, FAIL_REPAIR_DATE, FAIL_WRITER 
        ) VALUES (@ITR_Code, @Fail_Gojang_Name, @Fail_Weather, @Fail_Temp, @Fail_Hum, @Fail_Cause, @Fail_Reason, @Fail_Status, @Fail_Part, @Fail_Period, @Fail_Finder, @Fail_Repairer, @Fail_Supervisor, @Fail_Repair_Date, @Fail_Writer)";

                    int affectedRows = dbHelper.Conn.Execute(query, itrGojangList);
                    if (affectedRows > 0)
                    {
                        res.Message = "ITR 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ITR 고장이력 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateITRFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateITRFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // VCB 고장이력 데이터 업데이트
        public Result UpdateITRFHRepo(ITRFailureHistory itrGojangList)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE INTERFACETR_FAILURE_HISTORY
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
                WHERE ITR_CODE = @ITR_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, itrGojangList);
                    res.Message = affectedRows > 0 ? "ITR 고장이력 데이터 업데이트 성공" : "ITR 고장이력 데이터 업데이트 실패";
                }
            }

            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateITRFHRepo 실패: {ex.Message}";
            }
            return res;
        }

        // VCB 고장이력 데이터 삭제
        public Result DeleteITRFHRepo(string itrCode, string gojangName)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM INTERFACETR_FAILURE_HISTORY WHERE ITR_CODE = @ITR_Code AND FAIL_GOJANG_NAME = @Fail_Gojang_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ITR_Code = itrCode, FAIL_Gojang_Name = gojangName });
                    res.Message = affectedRows > 0 ? "ITR 고장이력 데이터 삭제 성공" : "ITR 고장이력 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteITRFHRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}
