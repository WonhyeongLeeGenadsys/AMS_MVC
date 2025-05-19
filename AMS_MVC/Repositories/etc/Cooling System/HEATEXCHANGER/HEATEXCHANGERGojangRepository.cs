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
    public class HEATEXCHANGERGojangRepository
    {
        /// <summary>
        /// 특정 HEATEXCHANGER_Code에 대한 고장이력(HEATEXCHANGER_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetHEATEXCHANGERFHByHEATEXCHANGERCode(string heatexchangerCode, out List<HEATEXCHANGERFailureHistory> heatexchangerFHList)
        {
            Result res = new Result(true);
            heatexchangerFHList = new List<HEATEXCHANGERFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM HEATEXCHANGER_FAILURE_HISTORY
                        WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code";

                    heatexchangerFHList = dbHelper.Conn
                        .Query<HEATEXCHANGERFailureHistory>(query, new { HEATEXCHANGER_Code = heatexchangerCode })
                        .AsList();

                    res.Message = $"GetHEATEXCHANGERFHByHEATEXCHANGERCode 성공: HEATEXCHANGER_CODE = {heatexchangerCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetHEATEXCHANGERFHByHEATEXCHANGERCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(HEATEXCHANGER_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 HEATEXCHANGER 고장이력 목록 조회
        /// </summary>
        public Result GetTotalHEATEXCHANGERGojang(out List<HEATEXCHANGERFailureHistory> heatexchangerGojangList)
        {
            Result res = new Result(true);
            heatexchangerGojangList = new List<HEATEXCHANGERFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM HEATEXCHANGER_FAILURE_HISTORY";
                    heatexchangerGojangList = dbHelper.Conn
                        .Query<HEATEXCHANGERFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalHEATEXCHANGERGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalHEATEXCHANGERGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(HEATEXCHANGER_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// HEATEXCHANGER_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetHEATEXCHANGERFHDetailByHEATEXCHANGERCode(string heatexchangerCode, string tblIdx, out List<HEATEXCHANGERFailureHistory> heatexchangerFHList)
        {
            Result res = new Result(true);
            heatexchangerFHList = new List<HEATEXCHANGERFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM HEATEXCHANGER_FAILURE_HISTORY
                        WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code
                          AND TBL_IDX = @Tbl_Idx";

                    heatexchangerFHList = dbHelper.Conn
                        .Query<HEATEXCHANGERFailureHistory>(query, new { HEATEXCHANGER_Code = heatexchangerCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (heatexchangerFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetHEATEXCHANGERFHDetailByHEATEXCHANGERCode 성공: HEATEXCHANGER_CODE = {heatexchangerCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetHEATEXCHANGERFHDetailByHEATEXCHANGERCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateHEATEXCHANGERFHRepo(HEATEXCHANGERFailureHistory heatexchangerFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO HEATEXCHANGER_FAILURE_HISTORY (
                            HEATEXCHANGER_CODE,
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
                            @HEATEXCHANGER_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, heatexchangerFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "HEATEXCHANGER 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "HEATEXCHANGER 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateHEATEXCHANGERFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateHEATEXCHANGERFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateHEATEXCHANGERFHRepo(HEATEXCHANGERFailureHistory heatexchangerFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE HEATEXCHANGER_FAILURE_HISTORY
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
                        WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, heatexchangerFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "HEATEXCHANGER 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "HEATEXCHANGER 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateHEATEXCHANGERFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteHEATEXCHANGERFHRepo(string heatexchangerCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM HEATEXCHANGER_FAILURE_HISTORY
                        WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { HEATEXCHANGER_Code = heatexchangerCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "HEATEXCHANGER 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "HEATEXCHANGER 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteHEATEXCHANGERFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
