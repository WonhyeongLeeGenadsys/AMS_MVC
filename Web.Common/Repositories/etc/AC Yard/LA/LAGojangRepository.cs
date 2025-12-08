
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class LAGojangRepository
    {
        /// <summary>
        /// 특정 LA_Code에 대한 고장이력(LA_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetLAFHByLACode(string laCode, out List<LAFailureHistory> laFHList)
        {
            Result res = new Result(true);
            laFHList = new List<LAFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM LA_FAILURE_HISTORY
                        WHERE LA_CODE = @LA_Code";

                    laFHList = dbHelper.Conn
                        .Query<LAFailureHistory>(query, new { LA_Code = laCode })
                        .AsList();

                    res.Message = $"GetLAFHByLACode 성공: LA_CODE = {laCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLAFHByLACode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(LA_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 LA 고장이력 목록 조회
        /// </summary>
        public Result GetTotalLAGojang(out List<LAFailureHistory> laGojangList)
        {
            Result res = new Result(true);
            laGojangList = new List<LAFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM LA_FAILURE_HISTORY";
                    laGojangList = dbHelper.Conn
                        .Query<LAFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalLAGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalLAGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(LA_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// LA_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetLAFHDetailByLACode(string laCode, string tblIdx, out List<LAFailureHistory> laFHList)
        {
            Result res = new Result(true);
            laFHList = new List<LAFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM LA_FAILURE_HISTORY
                        WHERE LA_CODE = @LA_Code
                          AND TBL_IDX = @Tbl_Idx";

                    laFHList = dbHelper.Conn
                        .Query<LAFailureHistory>(query, new { LA_Code = laCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (laFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetLAFHDetailByLACode 성공: LA_CODE = {laCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLAFHDetailByLACode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateLAFHRepo(LAFailureHistory laFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO LA_FAILURE_HISTORY (
                            LA_CODE,
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
                            @LA_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, laFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "LA 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "LA 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateLAFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateLAFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateLAFHRepo(LAFailureHistory laFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE LA_FAILURE_HISTORY
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
                        WHERE LA_CODE = @LA_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, laFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "LA 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "LA 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateLAFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteLAFHRepo(string laCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM LA_FAILURE_HISTORY
                        WHERE LA_CODE = @LA_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { LA_Code = laCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "LA 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "LA 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteLAFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
