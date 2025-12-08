
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DSGojangRepository
    {
        /// <summary>
        /// 특정 DS_Code에 대한 고장이력(DS_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetDSFHByDSCode(string dsCode, out List<DSFailureHistory> dsFHList)
        {
            Result res = new Result(true);
            dsFHList = new List<DSFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM DS_FAILURE_HISTORY
                        WHERE DS_CODE = @DS_Code";

                    dsFHList = dbHelper.Conn
                        .Query<DSFailureHistory>(query, new { DS_Code = dsCode })
                        .AsList();

                    res.Message = $"GetDSFHByDSCode 성공: DS_CODE = {dsCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDSFHByDSCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DS_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 DS 고장이력 목록 조회
        /// </summary>
        public Result GetTotalDSGojang(out List<DSFailureHistory> dsGojangList)
        {
            Result res = new Result(true);
            dsGojangList = new List<DSFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DS_FAILURE_HISTORY";
                    dsGojangList = dbHelper.Conn
                        .Query<DSFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalDSGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDSGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DS_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// DS_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetDSFHDetailByDSCode(string dsCode, string tblIdx, out List<DSFailureHistory> dsFHList)
        {
            Result res = new Result(true);
            dsFHList = new List<DSFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM DS_FAILURE_HISTORY
                        WHERE DS_CODE = @DS_Code
                          AND TBL_IDX = @Tbl_Idx";

                    dsFHList = dbHelper.Conn
                        .Query<DSFailureHistory>(query, new { DS_Code = dsCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (dsFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDSFHDetailByDSCode 성공: DS_CODE = {dsCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDSFHDetailByDSCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateDSFHRepo(DSFailureHistory dsFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO DS_FAILURE_HISTORY (
                            DS_CODE,
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
                            @DS_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, dsFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "DS 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DS 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDSFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDSFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateDSFHRepo(DSFailureHistory dsFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE DS_FAILURE_HISTORY
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
                        WHERE DS_CODE = @DS_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dsFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "DS 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DS 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDSFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteDSFHRepo(string dsCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM DS_FAILURE_HISTORY
                        WHERE DS_CODE = @DS_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DS_Code = dsCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "DS 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DS 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDSFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
