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
    public class CTMaintenanceRepository
    {

        public Result GetCTMRByCTCode(string ctCode, out List<CTMaintenanceHistory> ctMRList)
        {
            Result res = new Result(true);
            ctMRList = new List<CTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM CT_MAINTENANCE_HISTORY 
                WHERE CT_CODE = @CT_Code";

                    ctMRList = dbHelper.Conn.Query<CTMaintenanceHistory>(query, new { CT_Code = ctCode }).AsList();
                    res.Message = $"GetCTMRByCTCode 성공: CT_CODE = {ctCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetCTMRByCTCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(CT_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 CT 유지보수 데이터 조회
        public Result GetTotalCTMaintenance(out List<CTMaintenanceHistory> ctMRList)
        {
            Result res = new Result(true);
            ctMRList = new List<CTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM CT_MAINTENANCE_HISTORY";
                    ctMRList = dbHelper.Conn.Query<CTMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalCTMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalCTMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(CT_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetCTMRDetailByCTCode(string ctCode, string tblIdx, out List<CTMaintenanceHistory> ctMRList)
        {
            Result res = new Result(true);
            ctMRList = new List<CTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM CT_MAINTENANCE_HISTORY 
                WHERE CT_CODE = @CT_Code AND TBL_IDX = @Tbl_Idx";

                    ctMRList = dbHelper.Conn.Query<CTMaintenanceHistory>(query, new { CT_Code = ctCode, Tbl_Idx = tblIdx }).AsList();
                    if (ctMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetCTMRDetailByCTCode 성공: CT_CODE = {ctCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetCTMRDetailByCTCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateCTMRRepo(CTMaintenanceHistory ctMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO CT_MAINTENANCE_HISTORY (
                    CT_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @CT_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, ctMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "CT 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "CT 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateCTMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateCTMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // CT 유지보수 데이터 업데이트
        public Result UpdateCTMRRepo(CTMaintenanceHistory ctMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE CT_MAINTENANCE_HISTORY
                SET 
                    MR_BOSU_NAME = @MR_Bosu_Name,
                    MR_WEATHER = @MR_Weather,
                    MR_TEMP = @MR_Temp,
                    MR_HUM = @MR_Hum,
                    MR_CONTENT = @MR_Content,
                    MR_STATUS = @MR_Status,
                    MR_PART = @MR_Part,
                    MR_WORKER = @MR_Worker,
                    MR_MANAGER = @MR_Manager,
                    MR_DATE = @MR_Date,
                    MR_WRITER = @MR_Writer
                WHERE CT_CODE = @CT_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, ctMR);
                    res.Message = affectedRows > 0 ? "CT 유지보수 데이터 업데이트 성공" : "CT 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateCTMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // CT 유지보수 데이터 삭제
        public Result DeleteCTMRRepo(string ctCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM CT_MAINTENANCE_HISTORY WHERE CT_CODE = @CT_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { CT_Code = ctCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "CT 유지보수 데이터 삭제 성공" : "CT 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteCTMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

