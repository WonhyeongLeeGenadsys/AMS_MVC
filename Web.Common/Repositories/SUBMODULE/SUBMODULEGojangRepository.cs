
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class SUBMODULEGojangRepository
    {
        /// <summary>
        /// 특정 SUBMODULE_Code에 대한 고장이력(SUBMODULE_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetSUBMODULEFHBySUBMODULECode(string submoduleCode, out List<SUBMODULEFailureHistory> submoduleFHList)
        {
            Result res = new Result(true);
            submoduleFHList = new List<SUBMODULEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM SUBMODULE_FAILURE_HISTORY
                        WHERE SUBMODULE_CODE = @SUBMODULE_Code";

                    submoduleFHList = dbHelper.Conn
                        .Query<SUBMODULEFailureHistory>(query, new { SUBMODULE_Code = submoduleCode })
                        .AsList();

                    res.Message = $"GetSUBMODULEFHBySUBMODULECode 성공: SUBMODULE_CODE = {submoduleCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSUBMODULEFHBySUBMODULECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SUBMODULE_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 SUBMODULE 고장이력 목록 조회
        /// </summary>
        public Result GetTotalSUBMODULEGojang(out List<SUBMODULEFailureHistory> submoduleGojangList)
        {
            Result res = new Result(true);
            submoduleGojangList = new List<SUBMODULEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM SUBMODULE_FAILURE_HISTORY";
                    submoduleGojangList = dbHelper.Conn
                        .Query<SUBMODULEFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalSUBMODULEGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalSUBMODULEGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SUBMODULE_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// SUBMODULE_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetSUBMODULEFHDetailBySUBMODULECode(string submoduleCode, string tblIdx, out List<SUBMODULEFailureHistory> submoduleFHList)
        {
            Result res = new Result(true);
            submoduleFHList = new List<SUBMODULEFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM SUBMODULE_FAILURE_HISTORY
                        WHERE SUBMODULE_CODE = @SUBMODULE_Code
                          AND TBL_IDX = @Tbl_Idx";

                    submoduleFHList = dbHelper.Conn
                        .Query<SUBMODULEFailureHistory>(query, new { SUBMODULE_Code = submoduleCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (submoduleFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetSUBMODULEFHDetailBySUBMODULECode 성공: SUBMODULE_CODE = {submoduleCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSUBMODULEFHDetailBySUBMODULECode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateSUBMODULEFHRepo(SUBMODULEFailureHistory submoduleFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO SUBMODULE_FAILURE_HISTORY (
                            SUBMODULE_CODE,
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
                            @SUBMODULE_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, submoduleFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "SUBMODULE 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SUBMODULE 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateSUBMODULEFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateSUBMODULEFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateSUBMODULEFHRepo(SUBMODULEFailureHistory submoduleFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE SUBMODULE_FAILURE_HISTORY
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
                        WHERE SUBMODULE_CODE = @SUBMODULE_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, submoduleFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "SUBMODULE 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SUBMODULE 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateSUBMODULEFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteSUBMODULEFHRepo(string submoduleCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM SUBMODULE_FAILURE_HISTORY
                        WHERE SUBMODULE_CODE = @SUBMODULE_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { SUBMODULE_Code = submoduleCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "SUBMODULE 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SUBMODULE 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteSUBMODULEFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
