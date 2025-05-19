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
    public class NGRGojangRepository
    {
        /// <summary>
        /// 특정 NGR_Code에 대한 고장이력(NGR_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetNGRFHByNGRCode(string ngrCode, out List<NGRFailureHistory> ngrFHList)
        {
            Result res = new Result(true);
            ngrFHList = new List<NGRFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM NGR_FAILURE_HISTORY
                        WHERE NGR_CODE = @NGR_Code";

                    ngrFHList = dbHelper.Conn
                        .Query<NGRFailureHistory>(query, new { NGR_Code = ngrCode })
                        .AsList();

                    res.Message = $"GetNGRFHByNGRCode 성공: NGR_CODE = {ngrCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetNGRFHByNGRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(NGR_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 NGR 고장이력 목록 조회
        /// </summary>
        public Result GetTotalNGRGojang(out List<NGRFailureHistory> ngrGojangList)
        {
            Result res = new Result(true);
            ngrGojangList = new List<NGRFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM NGR_FAILURE_HISTORY";
                    ngrGojangList = dbHelper.Conn
                        .Query<NGRFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalNGRGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalNGRGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(NGR_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// NGR_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetNGRFHDetailByNGRCode(string ngrCode, string tblIdx, out List<NGRFailureHistory> ngrFHList)
        {
            Result res = new Result(true);
            ngrFHList = new List<NGRFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM NGR_FAILURE_HISTORY
                        WHERE NGR_CODE = @NGR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    ngrFHList = dbHelper.Conn
                        .Query<NGRFailureHistory>(query, new { NGR_Code = ngrCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (ngrFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetNGRFHDetailByNGRCode 성공: NGR_CODE = {ngrCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetNGRFHDetailByNGRCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateNGRFHRepo(NGRFailureHistory ngrFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO NGR_FAILURE_HISTORY (
                            NGR_CODE,
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
                            @NGR_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, ngrFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "NGR 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "NGR 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateNGRFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateNGRFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateNGRFHRepo(NGRFailureHistory ngrFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE NGR_FAILURE_HISTORY
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
                        WHERE NGR_CODE = @NGR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, ngrFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "NGR 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "NGR 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateNGRFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteNGRFHRepo(string ngrCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM NGR_FAILURE_HISTORY
                        WHERE NGR_CODE = @NGR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { NGR_Code = ngrCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "NGR 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "NGR 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteNGRFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
