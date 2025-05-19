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
    public class PUMPMaintenanceRepository
    {

        public Result GetPUMPMRByPUMPCode(string pumpCode, out List<PUMPMaintenanceHistory> pumpMRList)
        {
            Result res = new Result(true);
            pumpMRList = new List<PUMPMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM PUMP_MAINTENANCE_HISTORY 
                WHERE PUMP_CODE = @PUMP_Code";

                    pumpMRList = dbHelper.Conn.Query<PUMPMaintenanceHistory>(query, new { PUMP_Code = pumpCode }).AsList();
                    res.Message = $"GetPUMPMRByPUMPCode 성공: PUMP_CODE = {pumpCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPUMPMRByPUMPCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PUMP_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 PUMP 유지보수 데이터 조회
        public Result GetTotalPUMPMaintenance(out List<PUMPMaintenanceHistory> pumpMRList)
        {
            Result res = new Result(true);
            pumpMRList = new List<PUMPMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM PUMP_MAINTENANCE_HISTORY";
                    pumpMRList = dbHelper.Conn.Query<PUMPMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalPUMPMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalPUMPMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PUMP_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetPUMPMRDetailByPUMPCode(string pumpCode, string tblIdx, out List<PUMPMaintenanceHistory> pumpMRList)
        {
            Result res = new Result(true);
            pumpMRList = new List<PUMPMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM PUMP_MAINTENANCE_HISTORY 
                WHERE PUMP_CODE = @PUMP_Code AND TBL_IDX = @Tbl_Idx";

                    pumpMRList = dbHelper.Conn.Query<PUMPMaintenanceHistory>(query, new { PUMP_Code = pumpCode, Tbl_Idx = tblIdx }).AsList();
                    if (pumpMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetPUMPMRDetailByPUMPCode 성공: PUMP_CODE = {pumpCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPUMPMRDetailByPUMPCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreatePUMPMRRepo(PUMPMaintenanceHistory pumpMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO PUMP_MAINTENANCE_HISTORY (
                    PUMP_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @PUMP_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, pumpMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "PUMP 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PUMP 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreatePUMPMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreatePUMPMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // PUMP 유지보수 데이터 업데이트
        public Result UpdatePUMPMRRepo(PUMPMaintenanceHistory pumpMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE PUMP_MAINTENANCE_HISTORY
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
                WHERE PUMP_CODE = @PUMP_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, pumpMR);
                    res.Message = affectedRows > 0 ? "PUMP 유지보수 데이터 업데이트 성공" : "PUMP 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdatePUMPMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // PUMP 유지보수 데이터 삭제
        public Result DeletePUMPMRRepo(string pumpCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM PUMP_MAINTENANCE_HISTORY WHERE PUMP_CODE = @PUMP_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { PUMP_Code = pumpCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "PUMP 유지보수 데이터 삭제 성공" : "PUMP 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeletePUMPMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

