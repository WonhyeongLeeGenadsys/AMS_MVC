
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class VCBGojangRepository
    {
        /// <summary>
        /// 특정 VCB_Code에 대한 고장이력(VCB_FAILURE_HISTORY) 목록 조회
        /// </summary>
        public Result GetVCBFHByVCBCode(string vcbCode, out List<VCBFailureHistory> vcbFHList)
        {
            Result res = new Result(true);
            vcbFHList = new List<VCBFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT * 
                        FROM VCB_FAILURE_HISTORY
                        WHERE VCB_CODE = @VCB_Code";

                    vcbFHList = dbHelper.Conn
                        .Query<VCBFailureHistory>(query, new { VCB_Code = vcbCode })
                        .AsList();

                    res.Message = $"GetVCBFHByVCBCode 성공: VCB_CODE = {vcbCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBFHByVCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(VCB_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 전체 VCB 고장이력 목록 조회
        /// </summary>
        public Result GetTotalVCBGojang(out List<VCBFailureHistory> vcbGojangList)
        {
            Result res = new Result(true);
            vcbGojangList = new List<VCBFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM VCB_FAILURE_HISTORY";
                    vcbGojangList = dbHelper.Conn
                        .Query<VCBFailureHistory>(query)
                        .AsList();

                    res.Message = "GetTotalVCBGojang 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalVCBGojang 실패: {ex.Message}";
                LogHelper.WriteLog("DB(VCB_FAILURE_HISTORY)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// VCB_Code와 Tbl_Idx로 단일 고장이력 상세 조회
        /// </summary>
        public Result GetVCBFHDetailByVCBCode(string vcbCode, string tblIdx, out List<VCBFailureHistory> vcbFHList)
        {
            Result res = new Result(true);
            vcbFHList = new List<VCBFailureHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM VCB_FAILURE_HISTORY
                        WHERE VCB_CODE = @VCB_Code
                          AND TBL_IDX = @Tbl_Idx";

                    vcbFHList = dbHelper.Conn
                        .Query<VCBFailureHistory>(query, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (vcbFHList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetVCBFHDetailByVCBCode 성공: VCB_CODE = {vcbCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBFHDetailByVCBCode 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 추가
        /// </summary>
        public Result CreateVCBFHRepo(VCBFailureHistory vcbFH)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        INSERT INTO VCB_FAILURE_HISTORY (
                            VCB_CODE,
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
                            @VCB_Code,
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

                    int affectedRows = dbHelper.Conn.Execute(query, vcbFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "VCB 고장이력 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "VCB 고장이력 데이터 추가 실패: DB 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateVCBFHRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateVCBFHRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 업데이트 (마지막 쉼표 제거!)
        /// </summary>
        public Result UpdateVCBFHRepo(VCBFailureHistory vcbFH)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        UPDATE VCB_FAILURE_HISTORY
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
                        WHERE VCB_CODE = @VCB_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, vcbFH);
                    if (affectedRows > 0)
                    {
                        res.Message = "VCB 고장이력 데이터 업데이트 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "VCB 고장이력 데이터 업데이트 실패: 일치하는 레코드가 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateVCBFHRepo 실패: {ex.Message}";
            }

            return res;
        }

        /// <summary>
        /// 고장이력 데이터 삭제
        /// </summary>
        public Result DeleteVCBFHRepo(string vcbCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        DELETE FROM VCB_FAILURE_HISTORY
                        WHERE VCB_CODE = @VCB_Code
                          AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx });
                    if (affectedRows > 0)
                    {
                        res.Message = "VCB 고장이력 데이터 삭제 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "VCB 고장이력 데이터 삭제 실패: 대상 레코드를 찾을 수 없음.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteVCBFHRepo 실패: {ex.Message}";
            }

            return res;
        }
    }
}
