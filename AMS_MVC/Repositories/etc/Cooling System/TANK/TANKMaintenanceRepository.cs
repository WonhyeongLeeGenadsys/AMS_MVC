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
    public class TANKMaintenanceRepository
    {

        public Result GetTANKMRByTANKCode(string tankCode, out List<TANKMaintenanceHistory> tankMRList)
        {
            Result res = new Result(true);
            tankMRList = new List<TANKMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM TANK_MAINTENANCE_HISTORY 
                WHERE TANK_CODE = @TANK_Code";

                    tankMRList = dbHelper.Conn.Query<TANKMaintenanceHistory>(query, new { TANK_Code = tankCode }).AsList();
                    res.Message = $"GetTANKMRByTANKCode 성공: TANK_CODE = {tankCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTANKMRByTANKCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(TANK_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 TANK 유지보수 데이터 조회
        public Result GetTotalTANKMaintenance(out List<TANKMaintenanceHistory> tankMRList)
        {
            Result res = new Result(true);
            tankMRList = new List<TANKMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM TANK_MAINTENANCE_HISTORY";
                    tankMRList = dbHelper.Conn.Query<TANKMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalTANKMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalTANKMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(TANK_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetTANKMRDetailByTANKCode(string tankCode, string tblIdx, out List<TANKMaintenanceHistory> tankMRList)
        {
            Result res = new Result(true);
            tankMRList = new List<TANKMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM TANK_MAINTENANCE_HISTORY 
                WHERE TANK_CODE = @TANK_Code AND TBL_IDX = @Tbl_Idx";

                    tankMRList = dbHelper.Conn.Query<TANKMaintenanceHistory>(query, new { TANK_Code = tankCode, Tbl_Idx = tblIdx }).AsList();
                    if (tankMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetTANKMRDetailByTANKCode 성공: TANK_CODE = {tankCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTANKMRDetailByTANKCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateTANKMRRepo(TANKMaintenanceHistory tankMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO TANK_MAINTENANCE_HISTORY (
                    TANK_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @TANK_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, tankMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "TANK 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "TANK 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateTANKMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateTANKMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // TANK 유지보수 데이터 업데이트
        public Result UpdateTANKMRRepo(TANKMaintenanceHistory tankMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE TANK_MAINTENANCE_HISTORY
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
                WHERE TANK_CODE = @TANK_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, tankMR);
                    res.Message = affectedRows > 0 ? "TANK 유지보수 데이터 업데이트 성공" : "TANK 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateTANKMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // TANK 유지보수 데이터 삭제
        public Result DeleteTANKMRRepo(string tankCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM TANK_MAINTENANCE_HISTORY WHERE TANK_CODE = @TANK_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { TANK_Code = tankCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "TANK 유지보수 데이터 삭제 성공" : "TANK 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteTANKMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

