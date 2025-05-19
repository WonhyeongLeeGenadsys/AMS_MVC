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
    public class DSMaintenanceRepository
    {

        public Result GetDSMRByDSCode(string dsCode, out List<DSMaintenanceHistory> dsMRList)
        {
            Result res = new Result(true);
            dsMRList = new List<DSMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM DS_MAINTENANCE_HISTORY 
                WHERE DS_CODE = @DS_Code";

                    dsMRList = dbHelper.Conn.Query<DSMaintenanceHistory>(query, new { DS_Code = dsCode }).AsList();
                    res.Message = $"GetDSMRByDSCode 성공: DS_CODE = {dsCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDSMRByDSCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DS_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 DS 유지보수 데이터 조회
        public Result GetTotalDSMaintenance(out List<DSMaintenanceHistory> dsMRList)
        {
            Result res = new Result(true);
            dsMRList = new List<DSMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DS_MAINTENANCE_HISTORY";
                    dsMRList = dbHelper.Conn.Query<DSMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalDSMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDSMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DS_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetDSMRDetailByDSCode(string dsCode, string tblIdx, out List<DSMaintenanceHistory> dsMRList)
        {
            Result res = new Result(true);
            dsMRList = new List<DSMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DS_MAINTENANCE_HISTORY 
                WHERE DS_CODE = @DS_Code AND TBL_IDX = @Tbl_Idx";

                    dsMRList = dbHelper.Conn.Query<DSMaintenanceHistory>(query, new { DS_Code = dsCode, Tbl_Idx = tblIdx }).AsList();
                    if (dsMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDSMRDetailByDSCode 성공: DS_CODE = {dsCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDSMRDetailByDSCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateDSMRRepo(DSMaintenanceHistory dsMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO DS_MAINTENANCE_HISTORY (
                    DS_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @DS_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, dsMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "DS 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DS 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDSMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDSMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // DS 유지보수 데이터 업데이트
        public Result UpdateDSMRRepo(DSMaintenanceHistory dsMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DS_MAINTENANCE_HISTORY
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
                WHERE DS_CODE = @DS_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dsMR);
                    res.Message = affectedRows > 0 ? "DS 유지보수 데이터 업데이트 성공" : "DS 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDSMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // DS 유지보수 데이터 삭제
        public Result DeleteDSMRRepo(string dsCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DS_MAINTENANCE_HISTORY WHERE DS_CODE = @DS_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DS_Code = dsCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "DS 유지보수 데이터 삭제 성공" : "DS 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDSMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

