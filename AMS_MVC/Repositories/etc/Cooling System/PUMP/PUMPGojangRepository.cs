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
    public class PUMPGojangRepository
    {
        /// <summary>
        /// 특정 PUMP_Code에 대한 고장이력(PUMP_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetPUMPFHByPUMPCode(string pumpCode, out List<PUMPFailureHistory> pumpFHList)
        {
            Result res = new Result(true);
            pumpFHList = new List<PUMPFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM PUMP_FAILURE_HISTORY
                        WHERE PUMP_CODE = @PUMP_Code";

                    pumpFHList = dbHelper.Conn
                        .Query<PUMPFailureHistory>(query, new { PUMP_Code = pumpCode })
                        .AsList();

                    res.Message = $"GetPUMPFHByPUMPCode 성공: PUMP_CODE = {pumpCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPUMPFHByPUMPCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PUMP_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 PUMP 고장이력 목록 조회
        /// </summary>
        public Result GetTotalPUMPGojang(out List<PUMPFailureHistory> pumpGojangList)
        {
            Result res = new Result(true);
            pumpGojangList = new List<PUMPFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM PUMP_FAILURE_HISTORY";
                    pumpGojangList = dbHelper.Conn
                        .Query<PUMPFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalPUMPGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalPUMPGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PUMP_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// PUMP_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetPUMPFHDetailByPUMPCode(string pumpCode, string tblIdx, out List<PUMPFailureHistory> pumpFHList)
        {
            Result res = new Result(true);
            pumpFHList = new List<PUMPFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM PUMP_FAILURE_HISTORY
                        WHERE PUMP_CODE = @PUMP_Code
                          AND TBL_IDX = @Tbl_Idx";

                    pumpFHList = dbHelper.Conn
                        .Query<PUMPFailureHistory>(query, new { PUMP_Code = pumpCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (pumpFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetPUMPFHDetailByPUMPCode 성공: PUMP_CODE = {pumpCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPUMPFHDetailByPUMPCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreatePUMPFHRepo(PUMPFailureHistory pumpFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO PUMP_FAILURE_HISTORY (
                            PUMP_CODE,
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
                            @PUMP_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, pumpFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "PUMP 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PUMP 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreatePUMPFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreatePUMPFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdatePUMPFHRepo(PUMPFailureHistory pumpFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE PUMP_FAILURE_HISTORY
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
                        WHERE PUMP_CODE = @PUMP_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, pumpFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "PUMP 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PUMP 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdatePUMPFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeletePUMPFHRepo(string pumpCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM PUMP_FAILURE_HISTORY
                        WHERE PUMP_CODE = @PUMP_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { PUMP_Code = pumpCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "PUMP 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PUMP 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeletePUMPFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
