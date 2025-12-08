
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class TANKGojangRepository
    {
        /// <summary>
        /// 특정 TANK_Code에 대한 고장이력(TANK_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetTANKFHByTANKCode(string tankCode, out List<TANKFailureHistory> tankFHList)
        {
            Result res = new Result(true);
            tankFHList = new List<TANKFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM TANK_FAILURE_HISTORY
                        WHERE TANK_CODE = @TANK_Code";

                    tankFHList = dbHelper.Conn
                        .Query<TANKFailureHistory>(query, new { TANK_Code = tankCode })
                        .AsList();

                    res.Message = $"GetTANKFHByTANKCode 성공: TANK_CODE = {tankCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTANKFHByTANKCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(TANK_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 TANK 고장이력 목록 조회
        /// </summary>
        public Result GetTotalTANKGojang(out List<TANKFailureHistory> tankGojangList)
        {
            Result res = new Result(true);
            tankGojangList = new List<TANKFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM TANK_FAILURE_HISTORY";
                    tankGojangList = dbHelper.Conn
                        .Query<TANKFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalTANKGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalTANKGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(TANK_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// TANK_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetTANKFHDetailByTANKCode(string tankCode, string tblIdx, out List<TANKFailureHistory> tankFHList)
        {
            Result res = new Result(true);
            tankFHList = new List<TANKFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM TANK_FAILURE_HISTORY
                        WHERE TANK_CODE = @TANK_Code
                          AND TBL_IDX = @Tbl_Idx";

                    tankFHList = dbHelper.Conn
                        .Query<TANKFailureHistory>(query, new { TANK_Code = tankCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (tankFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetTANKFHDetailByTANKCode 성공: TANK_CODE = {tankCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTANKFHDetailByTANKCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateTANKFHRepo(TANKFailureHistory tankFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO TANK_FAILURE_HISTORY (
                            TANK_CODE,
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
                            @TANK_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, tankFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "TANK 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "TANK 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateTANKFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateTANKFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateTANKFHRepo(TANKFailureHistory tankFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE TANK_FAILURE_HISTORY
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
                        WHERE TANK_CODE = @TANK_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, tankFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "TANK 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "TANK 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateTANKFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteTANKFHRepo(string tankCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM TANK_FAILURE_HISTORY
                        WHERE TANK_CODE = @TANK_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { TANK_Code = tankCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "TANK 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "TANK 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteTANKFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
