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
    public class ITRChk2Repository
    {
        // 시리얼 번호로 VCB 보통점검 데이터 조회
        public Result GetITRChk2ByITRCode(string itrCode, out List<ITRChk2> itrChk2List)
        {
            Result res = new Result(true);
            itrChk2List = new List<ITRChk2>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ITR_CHK2 WHERE ITR_CODE = @ITR_Code";
                    itrChk2List = dbHelper.Conn.Query<ITRChk2>(query, new { ITR_Code = itrCode }).AsList();
                    res.Message = $"GetITRChk2ByITRCode 성공: ITR_CODE = {itrCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRChk2ByITRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_CHK2)", res.Message);
            }
            return res;
        }

        //최근 점검 데이터 한개만 불러오기 
        public Result GetLatestITRChk2ByITRCode(string itrCode, out List<ITRChk2> itrChk2List)
        {
            Result res = new Result(true);
            itrChk2List = new List<ITRChk2>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                    SELECT TOP 1 *
                    FROM ITR_CHK2
                    WHERE ITR_CODE = @ITR_Code
                    ORDER BY CHK2_TBL_GETDATE DESC;";

                    itrChk2List = dbHelper.Conn
                        .Query<ITRChk2>(query, new { ITR_Code = itrCode })
                        .AsList();
                }

                res.Message = $"GetLatestITRChk2ByITRCode 성공(최신 1건): ITR_CODE = {itrCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLatestITRChk2ByITRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_CHK2)", res.Message);
            }

            return res;
        }

        // 전체 ITR 보통점검 데이터 조회
        public Result GetTotalITRChk2(out List<ITRChk2> itrChkList)
        {
            Result res = new Result(true);
            itrChkList = new List<ITRChk2>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ITR_CHK2";
                    itrChkList = dbHelper.Conn.Query<ITRChk2>(query).AsList();
                    res.Message = $"GetTotalITRChk2 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalITRChk2 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_Chk2)", res.Message);
            }
            return res;
        }

        public Result GetITRChk2DetailByITRCode(string itrCode, string tblIdx, out List<ITRChk2> itrChk2List)
        {
            Result res = new Result(true);
            itrChk2List = new List<ITRChk2>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM ITR_CHK2 
                WHERE ITR_CODE = @ITR_Code AND TBL_IDX = @Tbl_Idx";

                    itrChk2List = dbHelper.Conn.Query<ITRChk2>(query, new { ITR_Code = itrCode, Tbl_Idx = tblIdx }).AsList();
                    if (itrChk2List.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetITRChk2DetailByITRCode 성공: ITR_CODE = {itrCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRChk2DetailByITRCode 실패: {ex.Message}";
            }

            return res;
        }

        public List<dynamic> GetMonthlyAllITRChk2Counts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = @"
                    SELECT 
                        FORMAT(CHK2_Start_Date, 'yyyy-MM') AS Month, 
                        COUNT(*) AS Count,
                        'ITR2' AS Type
                    FROM ITR_CHK2
                    WHERE CHK2_Start_Date IS NOT NULL
                    GROUP BY FORMAT(CHK2_Start_Date, 'yyyy-MM')
                    ORDER BY Month;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        // ITR 정밀점검 데이터 추가
        public Result CreateITRChk2InfoRepo(ITRChk2 itrChk2)
        {
            var res = new Result(true);

            using (var dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    const string query = @"
INSERT INTO ITR_CHK2 (
    ITR_CODE, CHK2_GONGSA_NAME, CHK2_WEATHER, CHK2_TEMP, CHK2_HUM, CHK2_COMPANY,
    CHK2_WORKER, CHK2_MANAGER, CHK2_URGENT_NO, CHK2_TYPE, CHK2_START_DATE,
    CHK2_END_DATE, CHK2_COMPUTERIZED_PRICE, CHK2_WATER_CONTENT, CHK2_FURFURAL,
    CHK2_EXCITATION_CURRENT, CHK2_SHORT_CURRENT, CHK2_VOLTAGE_RATIO, CHK2_PD,
    FOLDINGFUNCTION, CHK2_WRITER, CHK2_TBL_GETDATE
) VALUES (
    @ITR_Code, @CHK2_Gongsa_Name, @CHK2_Weather, @CHK2_Temp, @CHK2_Hum, @CHK2_Company,
    @CHK2_Worker, @CHK2_Manager, @CHK2_Urgent_No, @CHK2_Type, @CHK2_Start_Date,
    @CHK2_End_Date, @CHK2_Computerized_Price, @CHK2_Water_Content, @CHK2_Furfural,
    @CHK2_Excitation_Current, @CHK2_Short_Current, @CHK2_Voltage_Ratio, @CHK2_PD,
    @FoldingFunction, @CHK2_Writer, @CHK2_Tbl_GetDate
)";
                    int affected = conn.Execute(query, itrChk2, tran);
                    if (affected <= 0)
                        throw new Exception("ITR_CHK2 레코드 삽입 실패");

                    tran.Commit();
                    res.Message = "ITR 정밀점검 데이터 추가 성공";
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    res.IsSuccess = false;
                    res.Message = "저장 오류: " + ex.Message;
                    LogHelper.WriteLog("DB(ITR_CHK2)", res.Message);
                }
            }

            return res;
        }

        // VCB 보통점검 데이터 업데이트
        public Result UpdateITRChk2InfoRepo(ITRChk2 itrChk2)
        {
            var res = new Result(true);
            try
            {
                using (var dbHelper = new DBHelper())
                {
                    const string query = @"
UPDATE ITR_CHK2
SET
    CHK2_GONGSA_NAME         = @CHK2_Gongsa_Name,
    CHK2_WEATHER             = @CHK2_Weather,
    CHK2_TEMP                = @CHK2_Temp,
    CHK2_HUM                 = @CHK2_Hum,
    CHK2_COMPANY             = @CHK2_Company,
    CHK2_WORKER              = @CHK2_Worker,
    CHK2_MANAGER             = @CHK2_Manager,
    CHK2_URGENT_NO           = @CHK2_Urgent_No,
    CHK2_TYPE                = @CHK2_Type,
    CHK2_START_DATE          = @CHK2_Start_Date,
    CHK2_END_DATE            = @CHK2_End_Date,
    CHK2_COMPUTERIZED_PRICE  = @CHK2_Computerized_Price,
    CHK2_WATER_CONTENT       = @CHK2_Water_Content,
    CHK2_FURFURAL            = @CHK2_Furfural,
    CHK2_EXCITATION_CURRENT  = @CHK2_Excitation_Current,
    CHK2_SHORT_CURRENT       = @CHK2_Short_Current,
    CHK2_VOLTAGE_RATIO       = @CHK2_Voltage_Ratio,
    CHK2_PD                  = @CHK2_PD,
    CHK2_WRITER              = @CHK2_Writer
WHERE ITR_CODE = @ITR_Code
  AND TBL_IDX   = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, itrChk2);
                    res.Message = affectedRows > 0
                        ? "ITR 정밀점검 데이터 업데이트 성공"
                        : "ITR 정밀점검 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateITRChk2InfoRepo 실패: {ex.Message}";
                Web.Common.Log.LogHelper.WriteLog("DB(ITR_CHK2)", res.Message);
            }
            return res;
        }

        // VCB 보통점검 데이터 삭제
        public Result DeleteITRChk2InfoRepo(string itrCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM ITR_CHK2 WHERE ITR_CODE = @ITR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ITR_Code = itrCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "ITR 정밀점검 데이터 삭제 성공" : "ITR 정밀점검 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteITRChk2InfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        public int? GetLatestFoldingFunction(string itrCode)
        {
            using (var db = new DBHelper())
            {
                const string sql = @"
                SELECT TOP 1 FoldingFunction
                FROM ITR_CHK2
                WHERE ITR_Code = @Code
                ORDER BY CHK2_Tbl_GetDate DESC";
                return db.Conn.QueryFirstOrDefault<int?>(sql, new { Code = itrCode });
            }
        }
    }
}
