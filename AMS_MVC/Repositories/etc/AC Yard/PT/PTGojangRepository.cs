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
    public class PTGojangRepository
    {
        /// <summary>
        /// 특정 PT_Code에 대한 고장이력(PT_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetPTFHByPTCode(string ptCode, out List<PTFailureHistory> ptFHList)
        {
            Result res = new Result(true);
            ptFHList = new List<PTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM PT_FAILURE_HISTORY
                        WHERE PT_CODE = @PT_Code";

                    ptFHList = dbHelper.Conn
                        .Query<PTFailureHistory>(query, new { PT_Code = ptCode })
                        .AsList();

                    res.Message = $"GetPTFHByPTCode 성공: PT_CODE = {ptCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPTFHByPTCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PT_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 PT 고장이력 목록 조회
        /// </summary>
        public Result GetTotalPTGojang(out List<PTFailureHistory> ptGojangList)
        {
            Result res = new Result(true);
            ptGojangList = new List<PTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM PT_FAILURE_HISTORY";
                    ptGojangList = dbHelper.Conn
                        .Query<PTFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalPTGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalPTGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PT_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// PT_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetPTFHDetailByPTCode(string ptCode, string tblIdx, out List<PTFailureHistory> ptFHList)
        {
            Result res = new Result(true);
            ptFHList = new List<PTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM PT_FAILURE_HISTORY
                        WHERE PT_CODE = @PT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    ptFHList = dbHelper.Conn
                        .Query<PTFailureHistory>(query, new { PT_Code = ptCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (ptFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetPTFHDetailByPTCode 성공: PT_CODE = {ptCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPTFHDetailByPTCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreatePTFHRepo(PTFailureHistory ptFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO PT_FAILURE_HISTORY (
                            PT_CODE,
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
                            @PT_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, ptFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "PT 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PT 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreatePTFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreatePTFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdatePTFHRepo(PTFailureHistory ptFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE PT_FAILURE_HISTORY
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
                        WHERE PT_CODE = @PT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, ptFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "PT 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PT 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdatePTFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeletePTFHRepo(string ptCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM PT_FAILURE_HISTORY
                        WHERE PT_CODE = @PT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { PT_Code = ptCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "PT 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PT 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeletePTFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
