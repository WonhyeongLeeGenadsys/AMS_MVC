
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class SAGojangRepository
    {
        /// <summary>
        /// 특정 SA_Code에 대한 고장이력(SA_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetSAFHBySACode(string saCode, out List<SAFailureHistory> saFHList)
        {
            Result res = new Result(true);
            saFHList = new List<SAFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM SA_FAILURE_HISTORY
                        WHERE SA_CODE = @SA_Code";

                    saFHList = dbHelper.Conn
                        .Query<SAFailureHistory>(query, new { SA_Code = saCode })
                        .AsList();

                    res.Message = $"GetSAFHBySACode 성공: SA_CODE = {saCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSAFHBySACode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SA_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 SA 고장이력 목록 조회
        /// </summary>
        public Result GetTotalSAGojang(out List<SAFailureHistory> saGojangList)
        {
            Result res = new Result(true);
            saGojangList = new List<SAFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM SA_FAILURE_HISTORY";
                    saGojangList = dbHelper.Conn
                        .Query<SAFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalSAGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalSAGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SA_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// SA_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetSAFHDetailBySACode(string saCode, string tblIdx, out List<SAFailureHistory> saFHList)
        {
            Result res = new Result(true);
            saFHList = new List<SAFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM SA_FAILURE_HISTORY
                        WHERE SA_CODE = @SA_Code
                          AND TBL_IDX = @Tbl_Idx";

                    saFHList = dbHelper.Conn
                        .Query<SAFailureHistory>(query, new { SA_Code = saCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (saFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetSAFHDetailBySACode 성공: SA_CODE = {saCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSAFHDetailBySACode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateSAFHRepo(SAFailureHistory saFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO SA_FAILURE_HISTORY (
                            SA_CODE,
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
                            @SA_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, saFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "SA 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SA 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateSAFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateSAFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateSAFHRepo(SAFailureHistory saFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE SA_FAILURE_HISTORY
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
                        WHERE SA_CODE = @SA_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, saFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "SA 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SA 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateSAFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteSAFHRepo(string saCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM SA_FAILURE_HISTORY
                        WHERE SA_CODE = @SA_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { SA_Code = saCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "SA 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SA 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteSAFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
