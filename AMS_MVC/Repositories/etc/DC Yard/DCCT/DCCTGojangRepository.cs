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
    public class DCCTGojangRepository
    {
        /// <summary>
        /// 특정 DCCT_Code에 대한 고장이력(DCCT_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetDCCTFHByDCCTCode(string dcctCode, out List<DCCTFailureHistory> dcctFHList)
        {
            Result res = new Result(true);
            dcctFHList = new List<DCCTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM DCCT_FAILURE_HISTORY
                        WHERE DCCT_CODE = @DCCT_Code";

                    dcctFHList = dbHelper.Conn
                        .Query<DCCTFailureHistory>(query, new { DCCT_Code = dcctCode })
                        .AsList();

                    res.Message = $"GetDCCTFHByDCCTCode 성공: DCCT_CODE = {dcctCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCTFHByDCCTCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCT_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 DCCT 고장이력 목록 조회
        /// </summary>
        public Result GetTotalDCCTGojang(out List<DCCTFailureHistory> dcctGojangList)
        {
            Result res = new Result(true);
            dcctGojangList = new List<DCCTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCT_FAILURE_HISTORY";
                    dcctGojangList = dbHelper.Conn
                        .Query<DCCTFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalDCCTGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCTGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCT_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// DCCT_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetDCCTFHDetailByDCCTCode(string dcctCode, string tblIdx, out List<DCCTFailureHistory> dcctFHList)
        {
            Result res = new Result(true);
            dcctFHList = new List<DCCTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM DCCT_FAILURE_HISTORY
                        WHERE DCCT_CODE = @DCCT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    dcctFHList = dbHelper.Conn
                        .Query<DCCTFailureHistory>(query, new { DCCT_Code = dcctCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (dcctFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCTFHDetailByDCCTCode 성공: DCCT_CODE = {dcctCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCTFHDetailByDCCTCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateDCCTFHRepo(DCCTFailureHistory dcctFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO DCCT_FAILURE_HISTORY (
                            DCCT_CODE,
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
                            @DCCT_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, dcctFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCT 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCT 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDCCTFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDCCTFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateDCCTFHRepo(DCCTFailureHistory dcctFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE DCCT_FAILURE_HISTORY
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
                        WHERE DCCT_CODE = @DCCT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dcctFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCT 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCT 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCTFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteDCCTFHRepo(string dcctCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM DCCT_FAILURE_HISTORY
                        WHERE DCCT_CODE = @DCCT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCT_Code = dcctCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCT 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCT 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCTFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
