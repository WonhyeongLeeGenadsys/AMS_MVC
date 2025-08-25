
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class WALLBUSHINGGojangRepository
    {
        /// <summary>
        /// 특정 WALLBUSHING_Code에 대한 고장이력(WALLBUSHING_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetWALLBUSHINGFHByWALLBUSHINGCode(string wallbushingCode, out List<WALLBUSHINGFailureHistory> wallbushingFHList)
        {
            Result res = new Result(true);
            wallbushingFHList = new List<WALLBUSHINGFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM WALLBUSHING_FAILURE_HISTORY
                        WHERE WALLBUSHING_CODE = @WALLBUSHING_Code";

                    wallbushingFHList = dbHelper.Conn
                        .Query<WALLBUSHINGFailureHistory>(query, new { WALLBUSHING_Code = wallbushingCode })
                        .AsList();

                    res.Message = $"GetWALLBUSHINGFHByWALLBUSHINGCode 성공: WALLBUSHING_CODE = {wallbushingCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetWALLBUSHINGFHByWALLBUSHINGCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(WALLBUSHING_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 WALLBUSHING 고장이력 목록 조회
        /// </summary>
        public Result GetTotalWALLBUSHINGGojang(out List<WALLBUSHINGFailureHistory> wallbushingGojangList)
        {
            Result res = new Result(true);
            wallbushingGojangList = new List<WALLBUSHINGFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM WALLBUSHING_FAILURE_HISTORY";
                    wallbushingGojangList = dbHelper.Conn
                        .Query<WALLBUSHINGFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalWALLBUSHINGGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalWALLBUSHINGGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(WALLBUSHING_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// WALLBUSHING_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetWALLBUSHINGFHDetailByWALLBUSHINGCode(string wallbushingCode, string tblIdx, out List<WALLBUSHINGFailureHistory> wallbushingFHList)
        {
            Result res = new Result(true);
            wallbushingFHList = new List<WALLBUSHINGFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM WALLBUSHING_FAILURE_HISTORY
                        WHERE WALLBUSHING_CODE = @WALLBUSHING_Code
                          AND TBL_IDX = @Tbl_Idx";

                    wallbushingFHList = dbHelper.Conn
                        .Query<WALLBUSHINGFailureHistory>(query, new { WALLBUSHING_Code = wallbushingCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (wallbushingFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetWALLBUSHINGFHDetailByWALLBUSHINGCode 성공: WALLBUSHING_CODE = {wallbushingCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetWALLBUSHINGFHDetailByWALLBUSHINGCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateWALLBUSHINGFHRepo(WALLBUSHINGFailureHistory wallbushingFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO WALLBUSHING_FAILURE_HISTORY (
                            WALLBUSHING_CODE,
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
                            @WALLBUSHING_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, wallbushingFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "WALLBUSHING 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "WALLBUSHING 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateWALLBUSHINGFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateWALLBUSHINGFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateWALLBUSHINGFHRepo(WALLBUSHINGFailureHistory wallbushingFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE WALLBUSHING_FAILURE_HISTORY
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
                        WHERE WALLBUSHING_CODE = @WALLBUSHING_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, wallbushingFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "WALLBUSHING 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "WALLBUSHING 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateWALLBUSHINGFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteWALLBUSHINGFHRepo(string wallbushingCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM WALLBUSHING_FAILURE_HISTORY
                        WHERE WALLBUSHING_CODE = @WALLBUSHING_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { WALLBUSHING_Code = wallbushingCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "WALLBUSHING 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "WALLBUSHING 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteWALLBUSHINGFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
