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
    public class SUBMODULEMaintenanceRepository
    {

        public Result GetSUBMODULEMRBySUBMODULECode(string submoduleCode, out List<SUBMODULEMaintenanceHistory> submoduleMRList)
        {
            Result res = new Result(true);
            submoduleMRList = new List<SUBMODULEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM SUBMODULE_MAINTENANCE_HISTORY 
                WHERE SUBMODULE_CODE = @SUBMODULE_Code";

                    submoduleMRList = dbHelper.Conn.Query<SUBMODULEMaintenanceHistory>(query, new { SUBMODULE_Code = submoduleCode }).AsList();
                    res.Message = $"GetSUBMODULEMRBySUBMODULECode 성공: SUBMODULE_CODE = {submoduleCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSUBMODULEMRBySUBMODULECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SUBMODULE_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }
        // 전체 SUBMODULE 유지보수 데이터 조회
        public Result GetTotalSUBMODULEMaintenance(out List<SUBMODULEMaintenanceHistory> submoduleMRList)
        {
            Result res = new Result(true);
            submoduleMRList = new List<SUBMODULEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM SUBMODULE_MAINTENANCE_HISTORY";
                    submoduleMRList = dbHelper.Conn.Query<SUBMODULEMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalSUBMODULEMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalSUBMODULEMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SUBMODULE_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetSUBMODULEMRDetailBySUBMODULECode(string submoduleCode, string maintenanceName, out List<SUBMODULEMaintenanceHistory> submoduleMRList)
        {
            Result res = new Result(true);
            submoduleMRList = new List<SUBMODULEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM SUBMODULE_MAINTENANCE_HISTORY 
                WHERE SUBMODULE_CODE = @SUBMODULE_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    submoduleMRList = dbHelper.Conn.Query<SUBMODULEMaintenanceHistory>(query, new { SUBMODULE_Code = submoduleCode, MR_Bosu_Name = maintenanceName }).AsList();
                    if (submoduleMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetSUBMODULEMRDetailBySUBMODULECode 성공: SUBMODULE_CODE = {submoduleCode}, MR_BOSU_NAME = {maintenanceName}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSUBMODULEMRDetailBySUBMODULECode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateSUBMODULEMRRepo(SUBMODULEMaintenanceHistory submoduleMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
        INSERT INTO SUBMODULE_MAINTENANCE_HISTORY (
        SUBMODULE_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, MR_COMPANY, MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
        ) VALUES (@SUBMODULE_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, @MR_Company, @MR_Worker, @MR_Mananger, @MR_Date, @MR_Writer)";

                    int affectedRows = dbHelper.Conn.Execute(query, submoduleMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "SUBMODULE 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SUBMODULE 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateSUBMODULEMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateSUBMODULEMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // SUBMODULE 유지보수 데이터 업데이트
        public Result UpdateSUBMODULEMRRepo(SUBMODULEMaintenanceHistory submoduleMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE SUBMODULE_MAINTENANCE_HISTORY
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
                WHERE SUBMODULE_CODE = @SUBMODULE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, submoduleMR);
                    res.Message = affectedRows > 0 ? "SUBMODULE 유지보수 데이터 업데이트 성공" : "SUBMODULE 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateSUBMODULEMRRepo 실패: {ex.Message}";
            }
            return res;
        }
        // SUBMODULE 유지보수 데이터 삭제
        public Result DeleteSUBMODULEMRRepo(string submoduleCode, string bosuName)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM SUBMODULE_MAINTENANCE_HISTORY WHERE SUBMODULE_CODE = @SUBMODULE_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, new { SUBMODULE_Code = submoduleCode, MR_Bosu_Name = bosuName });
                    res.Message = affectedRows > 0 ? "SUBMODULE 유지보수 데이터 삭제 성공" : "SUBMODULE 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteSUBMODULEMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

