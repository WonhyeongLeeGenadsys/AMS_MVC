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
    public class ITRChk1Repository
    {

        // 시리얼 번호로 VCB 보통점검 데이터 조회
        public Result GetITRChk1ByITRCode(string itrCode, out List<ITRChk1> itrChk1List)
        {
            Result res = new Result(true);
            itrChk1List = new List<ITRChk1>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ITR_CHK1 WHERE ITR_CODE = @ITR_Code";
                    itrChk1List = dbHelper.Conn.Query<ITRChk1>(query, new { ITR_Code = itrCode }).AsList();
                    res.Message = $"GetITRChk1ByITRCode 성공: ITR_CODE = {itrCode}";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRChk1ByITRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_CHK1)", res.Message);
            }
            return res;
        }

        // 전체 ITR 보통점검 데이터 조회
        public Result GetTotalITRChk1(out List<ITRChk1> itrChkList)
        {
            Result res = new Result(true);
            itrChkList = new List<ITRChk1>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ITR_CHK1";
                    itrChkList = dbHelper.Conn.Query<ITRChk1>(query).AsList();
                    res.Message = $"GetTotalITRChk1 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalITRChk1 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_Chk1)", res.Message);
            }
            return res;
        }

        public Result GetITRChk1DetailByITRCode(string itrCode, string tblIdx, out List<ITRChk1> itrChk1List)
        {
            Result res = new Result(true);
            itrChk1List = new List<ITRChk1>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM ITR_CHK1 
                WHERE ITR_CODE = @ITR_Code AND TBL_IDX = @Tbl_Idx";

                    itrChk1List = dbHelper.Conn.Query<ITRChk1>(query, new { ITR_Code = itrCode, Tbl_Idx = tblIdx }).AsList();
                    if (itrChk1List.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetITRChk1DetailByITRCode 성공: ITR_CODE = {itrCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRChk1DetailByITRCode 실패: {ex.Message}";
            }

            return res;
        }

        public List<dynamic> GetMonthlyAllITRChk1Counts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = @"
                    SELECT 
                        FORMAT(CHK1_Start_Date, 'yyyy-MM') AS Month, 
                        COUNT(*) AS Count,
                        'ITR1' AS Type
                    FROM ITR_CHK1
                    WHERE CHK1_Start_Date IS NOT NULL
                    GROUP BY FORMAT(CHK1_Start_Date, 'yyyy-MM')
                    ORDER BY Month;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        // ITR 보통점검 데이터 추가
        public Result CreateITRChk1InfoRepo(ITRChk1 itrChk1)
        {
            var res = new Result(true);

            using (var dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    const string query = @"
INSERT INTO ITR_CHK1 (
    ITR_CODE, CHK1_GONGSA_NAME, CHK1_WEATHER, CHK1_TEMP, CHK1_HUM, CHK1_COMPANY,
    CHK1_WORKER, CHK1_MANAGER, CHK1_URGENT_NO, CHK1_TYPE, CHK1_START_DATE,
    CHK1_END_DATE, CHK1_H2, CHK1_C2H2, CHK1_C2H4, CHK1_CH4, CHK1_C2H6, CHK1_CO, CHK1_CO2,
    CHK1_DIELECTRIC_STRENGTH, CHK1_REMAIN_LIFE, CHK1_AGE, CHK1_GOJANG_HISTORY,
    CHK1_DOBLE, CHK1_SFRA, CHK1_HV_E, CHK1_LV_E, CHK1_TV_E, CHK1_HV_LV,
    CHK1_HV_TV, CHK1_LV_TV, FOLDINGFUNCTION, CHK1_WRITER, CHK1_TBL_GETDATE
) VALUES (
    @ITR_Code, @CHK1_Gongsa_Name, @CHK1_Weather, @CHK1_Temp, @CHK1_Hum, @CHK1_Company,
    @CHK1_Worker, @CHK1_Manager, @CHK1_Urgent_No, @CHK1_Type, @CHK1_Start_Date,
    @CHK1_End_Date, @CHK1_H2, @CHK1_C2H2, @CHK1_C2H4, @CHK1_CH4, @CHK1_C2H6, @CHK1_CO, @CHK1_CO2,
    @CHK1_Dielectric_Strength, @CHK1_Remain_Life, @CHK1_Age, @CHK1_Gojang_History,
    @CHK1_Doble, @CHK1_SFRA, @CHK1_HV_E, @CHK1_LV_E, @CHK1_TV_E, @CHK1_HV_LV,
    @CHK1_HV_TV, @CHK1_LV_TV, @FoldingFunction, @CHK1_Writer, @CHK1_Tbl_GetDate
)";
                    int affected = conn.Execute(query, itrChk1, tran);
                    if (affected <= 0)
                        throw new Exception("ITR_CHK1 레코드 삽입 실패");

                    tran.Commit();
                    res.Message = "ITR 보통점검 데이터 추가 성공";
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    res.IsSuccess = false;
                    res.Message = "저장 오류: " + ex.Message;
                    LogHelper.WriteLog("DB(ITR_CHK1)", res.Message);
                }
            }

            return res;
        }
        // VCB 보통점검 데이터 업데이트
        public Result UpdateITRChk1InfoRepo(ITRChk1 itrChk1)
        {
            Result res = new Result(true);

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {

                    const string query = @"
UPDATE ITR_CHK1
SET 
    CHK1_WEATHER      = @CHK1_Weather,
    CHK1_TEMP         = @CHK1_Temp,
    CHK1_HUM          = @CHK1_Hum,
    CHK1_COMPANY      = @CHK1_Company,
    CHK1_WORKER       = @CHK1_Worker,
    CHK1_MANAGER      = @CHK1_Manager,
    CHK1_URGENT_NO    = @CHK1_Urgent_No,
    CHK1_TYPE         = @CHK1_Type,
    CHK1_START_DATE   = @CHK1_Start_Date,
    CHK1_END_DATE     = @CHK1_End_Date,
    CHK1_H2           = @CHK1_H2,
    CHK1_C2H2         = @CHK1_C2H2,
    CHK1_C2H4         = @CHK1_C2H4,
    CHK1_CH4          = @CHK1_CH4,
    CHK1_C2H6         = @CHK1_C2H6,
    CHK1_CO           = @CHK1_CO,
    CHK1_CO2          = @CHK1_CO2,
    CHK1_Dielectric_Strength = @CHK1_Dielectric_Strength,
    CHK1_Remain_Life  = @CHK1_Remain_Life,
    CHK1_Age          = @CHK1_Age,
    CHK1_Gojang_History= @CHK1_Gojang_History,
    CHK1_Doble        = @CHK1_Doble,
    CHK1_SFRA         = @CHK1_SFRA,
    CHK1_HV_E         = @CHK1_HV_E,
    CHK1_LV_E         = @CHK1_LV_E,
    CHK1_HV_LV        = @CHK1_HV_LV,
    CHK1_HV_TV        = @CHK1_HV_TV,
    CHK1_LV_TV        = @CHK1_LV_TV,
    CHK1_Writer       = @CHK1_Writer
WHERE ITR_CODE = @ITR_Code AND Tbl_Idx = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, itrChk1);
                    res.Message = affectedRows > 0 ? "ITR 보통점검 데이터 업데이트 성공" : "ITR 보통점검 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateITRChk1InfoRepo 실패: {ex.Message}";
            }
            return res;
        }

        // VCB 보통점검 데이터 삭제
        public Result DeleteITRChk1InfoRepo(string itrCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM ITR_CHK1 WHERE ITR_CODE = @ITR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ITR_Code = itrCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "ITR 보통점검 데이터 삭제 성공" : "ITR 보통점검 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteITRChk1InfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        public int? GetLatestFoldingFunction(string itrCode)
        {
            using (var db = new DBHelper())
            {
                const string sql = @"
                SELECT TOP 1 FoldingFunction
                FROM ITR_CHK1
                WHERE ITR_Code = @Code
                ORDER BY CHK1_Tbl_GetDate DESC";
                return db.Conn.QueryFirstOrDefault<int?>(sql, new { Code = itrCode });
            }
        }
    }
}
