
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class CTGojangRepository
    {
        /// <summary>
        /// 특정 CT_Code에 대한 고장이력(CT_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetCTFHByCTCode(string ctCode, out List<CTFailureHistory> ctFHList)
        {
            Result res = new Result(true);
            ctFHList = new List<CTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM CT_FAILURE_HISTORY
                        WHERE CT_CODE = @CT_Code";

                    ctFHList = dbHelper.Conn
                        .Query<CTFailureHistory>(query, new { CT_Code = ctCode })
                        .AsList();

                    res.Message = $"GetCTFHByCTCode 성공: CT_CODE = {ctCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetCTFHByCTCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(CT_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 CT 고장이력 목록 조회
        /// </summary>
        public Result GetTotalCTGojang(out List<CTFailureHistory> ctGojangList)
        {
            Result res = new Result(true);
            ctGojangList = new List<CTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM CT_FAILURE_HISTORY";
                    ctGojangList = dbHelper.Conn
                        .Query<CTFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalCTGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalCTGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(CT_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// CT_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetCTFHDetailByCTCode(string ctCode, string tblIdx, out List<CTFailureHistory> ctFHList)
        {
            Result res = new Result(true);
            ctFHList = new List<CTFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM CT_FAILURE_HISTORY
                        WHERE CT_CODE = @CT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    ctFHList = dbHelper.Conn
                        .Query<CTFailureHistory>(query, new { CT_Code = ctCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (ctFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetCTFHDetailByCTCode 성공: CT_CODE = {ctCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetCTFHDetailByCTCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateCTFHRepo(CTFailureHistory ctFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO CT_FAILURE_HISTORY (
                            CT_CODE,
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
                            @CT_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, ctFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "CT 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "CT 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateCTFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateCTFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateCTFHRepo(CTFailureHistory ctFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE CT_FAILURE_HISTORY
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
                        WHERE CT_CODE = @CT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, ctFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "CT 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "CT 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateCTFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteCTFHRepo(string ctCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM CT_FAILURE_HISTORY
                        WHERE CT_CODE = @CT_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { CT_Code = ctCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "CT 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "CT 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteCTFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
