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
    public class HEATEXCHANGERMaintenanceRepository
    {

        public Result GetHEATEXCHANGERMRByHEATEXCHANGERCode(string heatexchangerCode, out List<HEATEXCHANGERMaintenanceHistory> heatexchangerMRList)
        {
            Result res = new Result(true);
            heatexchangerMRList = new List<HEATEXCHANGERMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM HEATEXCHANGER_MAINTENANCE_HISTORY 
                WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code";

                    heatexchangerMRList = dbHelper.Conn.Query<HEATEXCHANGERMaintenanceHistory>(query, new { HEATEXCHANGER_Code = heatexchangerCode }).AsList();
                    res.Message = $"GetHEATEXCHANGERMRByHEATEXCHANGERCode 성공: HEATEXCHANGER_CODE = {heatexchangerCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetHEATEXCHANGERMRByHEATEXCHANGERCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(HEATEXCHANGER_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 HEATEXCHANGER 유지보수 데이터 조회
        public Result GetTotalHEATEXCHANGERMaintenance(out List<HEATEXCHANGERMaintenanceHistory> heatexchangerMRList)
        {
            Result res = new Result(true);
            heatexchangerMRList = new List<HEATEXCHANGERMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM HEATEXCHANGER_MAINTENANCE_HISTORY";
                    heatexchangerMRList = dbHelper.Conn.Query<HEATEXCHANGERMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalHEATEXCHANGERMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalHEATEXCHANGERMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(HEATEXCHANGER_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetHEATEXCHANGERMRDetailByHEATEXCHANGERCode(string heatexchangerCode, string tblIdx, out List<HEATEXCHANGERMaintenanceHistory> heatexchangerMRList)
        {
            Result res = new Result(true);
            heatexchangerMRList = new List<HEATEXCHANGERMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM HEATEXCHANGER_MAINTENANCE_HISTORY 
                WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code AND TBL_IDX = @Tbl_Idx";

                    heatexchangerMRList = dbHelper.Conn.Query<HEATEXCHANGERMaintenanceHistory>(query, new { HEATEXCHANGER_Code = heatexchangerCode, Tbl_Idx = tblIdx }).AsList();
                    if (heatexchangerMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetHEATEXCHANGERMRDetailByHEATEXCHANGERCode 성공: HEATEXCHANGER_CODE = {heatexchangerCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetHEATEXCHANGERMRDetailByHEATEXCHANGERCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateHEATEXCHANGERMRRepo(HEATEXCHANGERMaintenanceHistory heatexchangerMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO HEATEXCHANGER_MAINTENANCE_HISTORY (
                    HEATEXCHANGER_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @HEATEXCHANGER_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, heatexchangerMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "HEATEXCHANGER 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "HEATEXCHANGER 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateHEATEXCHANGERMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateHEATEXCHANGERMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // HEATEXCHANGER 유지보수 데이터 업데이트
        public Result UpdateHEATEXCHANGERMRRepo(HEATEXCHANGERMaintenanceHistory heatexchangerMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE HEATEXCHANGER_MAINTENANCE_HISTORY
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
                WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, heatexchangerMR);
                    res.Message = affectedRows > 0 ? "HEATEXCHANGER 유지보수 데이터 업데이트 성공" : "HEATEXCHANGER 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateHEATEXCHANGERMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // HEATEXCHANGER 유지보수 데이터 삭제
        public Result DeleteHEATEXCHANGERMRRepo(string heatexchangerCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM HEATEXCHANGER_MAINTENANCE_HISTORY WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { HEATEXCHANGER_Code = heatexchangerCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "HEATEXCHANGER 유지보수 데이터 삭제 성공" : "HEATEXCHANGER 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteHEATEXCHANGERMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

