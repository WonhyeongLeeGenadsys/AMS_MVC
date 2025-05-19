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
    public class LAMaintenanceRepository
    {

        public Result GetLAMRByLACode(string laCode, out List<LAMaintenanceHistory> laMRList)
        {
            Result res = new Result(true);
            laMRList = new List<LAMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM LA_MAINTENANCE_HISTORY 
                WHERE LA_CODE = @LA_Code";

                    laMRList = dbHelper.Conn.Query<LAMaintenanceHistory>(query, new { LA_Code = laCode }).AsList();
                    res.Message = $"GetLAMRByLACode 성공: LA_CODE = {laCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLAMRByLACode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(LA_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 LA 유지보수 데이터 조회
        public Result GetTotalLAMaintenance(out List<LAMaintenanceHistory> laMRList)
        {
            Result res = new Result(true);
            laMRList = new List<LAMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM LA_MAINTENANCE_HISTORY";
                    laMRList = dbHelper.Conn.Query<LAMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalLAMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalLAMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(LA_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetLAMRDetailByLACode(string laCode, string tblIdx, out List<LAMaintenanceHistory> laMRList)
        {
            Result res = new Result(true);
            laMRList = new List<LAMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM LA_MAINTENANCE_HISTORY 
                WHERE LA_CODE = @LA_Code AND TBL_IDX = @Tbl_Idx";

                    laMRList = dbHelper.Conn.Query<LAMaintenanceHistory>(query, new { LA_Code = laCode, Tbl_Idx = tblIdx }).AsList();
                    if (laMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetLAMRDetailByLACode 성공: LA_CODE = {laCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLAMRDetailByLACode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateLAMRRepo(LAMaintenanceHistory laMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO LA_MAINTENANCE_HISTORY (
                    LA_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @LA_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, laMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "LA 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "LA 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateLAMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateLAMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // LA 유지보수 데이터 업데이트
        public Result UpdateLAMRRepo(LAMaintenanceHistory laMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE LA_MAINTENANCE_HISTORY
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
                WHERE LA_CODE = @LA_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, laMR);
                    res.Message = affectedRows > 0 ? "LA 유지보수 데이터 업데이트 성공" : "LA 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateLAMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // LA 유지보수 데이터 삭제
        public Result DeleteLAMRRepo(string laCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM LA_MAINTENANCE_HISTORY WHERE LA_CODE = @LA_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { LA_Code = laCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "LA 유지보수 데이터 삭제 성공" : "LA 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteLAMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

