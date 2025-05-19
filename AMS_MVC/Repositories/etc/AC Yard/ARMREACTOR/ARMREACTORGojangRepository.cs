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
    public class ARMREACTORGojangRepository
    {
        /// <summary>
        /// 특정 ARMREACTOR_Code에 대한 고장이력(ARMREACTOR_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetARMREACTORFHByARMREACTORCode(string armreactorCode, out List<ARMREACTORFailureHistory> armreactorFHList)
        {
            Result res = new Result(true);
            armreactorFHList = new List<ARMREACTORFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM ARMREACTOR_FAILURE_HISTORY
                        WHERE ARMREACTOR_CODE = @ARMREACTOR_Code";

                    armreactorFHList = dbHelper.Conn
                        .Query<ARMREACTORFailureHistory>(query, new { ARMREACTOR_Code = armreactorCode })
                        .AsList();

                    res.Message = $"GetARMREACTORFHByARMREACTORCode 성공: ARMREACTOR_CODE = {armreactorCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetARMREACTORFHByARMREACTORCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ARMREACTOR_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 ARMREACTOR 고장이력 목록 조회
        /// </summary>
        public Result GetTotalARMREACTORGojang(out List<ARMREACTORFailureHistory> armreactorGojangList)
        {
            Result res = new Result(true);
            armreactorGojangList = new List<ARMREACTORFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ARMREACTOR_FAILURE_HISTORY";
                    armreactorGojangList = dbHelper.Conn
                        .Query<ARMREACTORFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalARMREACTORGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalARMREACTORGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ARMREACTOR_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// ARMREACTOR_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetARMREACTORFHDetailByARMREACTORCode(string armreactorCode, string tblIdx, out List<ARMREACTORFailureHistory> armreactorFHList)
        {
            Result res = new Result(true);
            armreactorFHList = new List<ARMREACTORFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM ARMREACTOR_FAILURE_HISTORY
                        WHERE ARMREACTOR_CODE = @ARMREACTOR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    armreactorFHList = dbHelper.Conn
                        .Query<ARMREACTORFailureHistory>(query, new { ARMREACTOR_Code = armreactorCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (armreactorFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetARMREACTORFHDetailByARMREACTORCode 성공: ARMREACTOR_CODE = {armreactorCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetARMREACTORFHDetailByARMREACTORCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateARMREACTORFHRepo(ARMREACTORFailureHistory armreactorFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO ARMREACTOR_FAILURE_HISTORY (
                            ARMREACTOR_CODE,
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
                            @ARMREACTOR_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, armreactorFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "ARMREACTOR 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ARMREACTOR 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateARMREACTORFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateARMREACTORFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateARMREACTORFHRepo(ARMREACTORFailureHistory armreactorFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE ARMREACTOR_FAILURE_HISTORY
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
                        WHERE ARMREACTOR_CODE = @ARMREACTOR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, armreactorFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "ARMREACTOR 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ARMREACTOR 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateARMREACTORFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteARMREACTORFHRepo(string armreactorCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM ARMREACTOR_FAILURE_HISTORY
                        WHERE ARMREACTOR_CODE = @ARMREACTOR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ARMREACTOR_Code = armreactorCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "ARMREACTOR 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ARMREACTOR 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteARMREACTORFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
