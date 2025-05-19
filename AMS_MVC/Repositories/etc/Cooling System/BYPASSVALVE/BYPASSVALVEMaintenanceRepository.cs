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
    public class BYPASSVALVEMaintenanceRepository
    {

        public Result GetBYPASSVALVEMRByBYPASSVALVECode(string bypassvalveCode, out List<BYPASSVALVEMaintenanceHistory> bypassvalveMRList)
        {
            Result res = new Result(true);
            bypassvalveMRList = new List<BYPASSVALVEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM BYPASSVALVE_MAINTENANCE_HISTORY 
                WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code";

                    bypassvalveMRList = dbHelper.Conn.Query<BYPASSVALVEMaintenanceHistory>(query, new { BYPASSVALVE_Code = bypassvalveCode }).AsList();
                    res.Message = $"GetBYPASSVALVEMRByBYPASSVALVECode 성공: BYPASSVALVE_CODE = {bypassvalveCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetBYPASSVALVEMRByBYPASSVALVECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(BYPASSVALVE_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 BYPASSVALVE 유지보수 데이터 조회
        public Result GetTotalBYPASSVALVEMaintenance(out List<BYPASSVALVEMaintenanceHistory> bypassvalveMRList)
        {
            Result res = new Result(true);
            bypassvalveMRList = new List<BYPASSVALVEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM BYPASSVALVE_MAINTENANCE_HISTORY";
                    bypassvalveMRList = dbHelper.Conn.Query<BYPASSVALVEMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalBYPASSVALVEMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalBYPASSVALVEMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(BYPASSVALVE_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetBYPASSVALVEMRDetailByBYPASSVALVECode(string bypassvalveCode, string tblIdx, out List<BYPASSVALVEMaintenanceHistory> bypassvalveMRList)
        {
            Result res = new Result(true);
            bypassvalveMRList = new List<BYPASSVALVEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM BYPASSVALVE_MAINTENANCE_HISTORY 
                WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code AND TBL_IDX = @Tbl_Idx";

                    bypassvalveMRList = dbHelper.Conn.Query<BYPASSVALVEMaintenanceHistory>(query, new { BYPASSVALVE_Code = bypassvalveCode, Tbl_Idx = tblIdx }).AsList();
                    if (bypassvalveMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetBYPASSVALVEMRDetailByBYPASSVALVECode 성공: BYPASSVALVE_CODE = {bypassvalveCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetBYPASSVALVEMRDetailByBYPASSVALVECode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateBYPASSVALVEMRRepo(BYPASSVALVEMaintenanceHistory bypassvalveMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO BYPASSVALVE_MAINTENANCE_HISTORY (
                    BYPASSVALVE_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @BYPASSVALVE_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, bypassvalveMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "BYPASSVALVE 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "BYPASSVALVE 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateBYPASSVALVEMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateBYPASSVALVEMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // BYPASSVALVE 유지보수 데이터 업데이트
        public Result UpdateBYPASSVALVEMRRepo(BYPASSVALVEMaintenanceHistory bypassvalveMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE BYPASSVALVE_MAINTENANCE_HISTORY
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
                WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, bypassvalveMR);
                    res.Message = affectedRows > 0 ? "BYPASSVALVE 유지보수 데이터 업데이트 성공" : "BYPASSVALVE 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateBYPASSVALVEMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // BYPASSVALVE 유지보수 데이터 삭제
        public Result DeleteBYPASSVALVEMRRepo(string bypassvalveCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM BYPASSVALVE_MAINTENANCE_HISTORY WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { BYPASSVALVE_Code = bypassvalveCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "BYPASSVALVE 유지보수 데이터 삭제 성공" : "BYPASSVALVE 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteBYPASSVALVEMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

