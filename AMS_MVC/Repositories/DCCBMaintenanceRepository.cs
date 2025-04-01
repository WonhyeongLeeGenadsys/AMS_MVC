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
    public class DCCBMaintenanceRepository
    {

        public Result GetDCCBMRByDCCBCode(string dccbCode, out List<DCCBMaintenanceHistory> dccbMRList)
        {
            Result res = new Result(true);
            dccbMRList = new List<DCCBMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM DCCB_MAINTENANCE_HISTORY 
                WHERE DCCB_CODE = @DCCB_Code";

                    dccbMRList = dbHelper.Conn.Query<DCCBMaintenanceHistory>(query, new { DCCB_Code = dccbCode }).AsList();
                    res.Message = $"GetDCCBMRByDCCBCode 성공: DCCB_CODE = {dccbCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCBMRByDCCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCB_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }
        // 전체 DCCB 유지보수 데이터 조회
        public Result GetTotalDCCBMaintenance(out List<DCCBMaintenanceHistory> dccbMRList)
        {
            Result res = new Result(true);
            dccbMRList = new List<DCCBMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCB_MAINTENANCE_HISTORY";
                    dccbMRList = dbHelper.Conn.Query<DCCBMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalDCCBMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCBMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCB_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetDCCBMRDetailByDCCBCode(string dccbCode, string maintenanceName, out List<DCCBMaintenanceHistory> dccbMRList)
        {
            Result res = new Result(true);
            dccbMRList = new List<DCCBMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DCCB_MAINTENANCE_HISTORY 
                WHERE DCCB_CODE = @DCCB_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    dccbMRList = dbHelper.Conn.Query<DCCBMaintenanceHistory>(query, new { DCCB_Code = dccbCode, MR_Bosu_Name = maintenanceName }).AsList();
                    if (dccbMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCBMRDetailByDCCBCode 성공: DCCB_CODE = {dccbCode}, MR_BOSU_NAME = {maintenanceName}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCBMRDetailByDCCBCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateDCCBMRRepo(DCCBMaintenanceHistory dccbMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
        INSERT INTO DCCB_MAINTENANCE_HISTORY (
        DCCB_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, MR_COMPANY, MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
        ) VALUES (@DCCB_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, @MR_Company, @MR_Worker, @MR_Mananger, @MR_Date, @MR_Writer)";

                    int affectedRows = dbHelper.Conn.Execute(query, dccbMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCB 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCB 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDCCBMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDCCBMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // DCCB 유지보수 데이터 업데이트
        public Result UpdateDCCBMRRepo(DCCBMaintenanceHistory dccbMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DCCB_MAINTENANCE_HISTORY
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
                WHERE DCCB_CODE = @DCCB_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dccbMR);
                    res.Message = affectedRows > 0 ? "DCCB 유지보수 데이터 업데이트 성공" : "DCCB 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCBMRRepo 실패: {ex.Message}";
            }
            return res;
        }
        // DCCB 유지보수 데이터 삭제
        public Result DeleteDCCBMRRepo(string dccbCode, string bosuName)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DCCB_MAINTENANCE_HISTORY WHERE DCCB_CODE = @DCCB_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCB_Code = dccbCode, MR_Bosu_Name = bosuName });
                    res.Message = affectedRows > 0 ? "DCCB 유지보수 데이터 삭제 성공" : "DCCB 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCBMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

