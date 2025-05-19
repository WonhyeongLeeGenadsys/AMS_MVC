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
    public class NGRMaintenanceRepository
    {

        public Result GetNGRMRByNGRCode(string ngrCode, out List<NGRMaintenanceHistory> ngrMRList)
        {
            Result res = new Result(true);
            ngrMRList = new List<NGRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM NGR_MAINTENANCE_HISTORY 
                WHERE NGR_CODE = @NGR_Code";

                    ngrMRList = dbHelper.Conn.Query<NGRMaintenanceHistory>(query, new { NGR_Code = ngrCode }).AsList();
                    res.Message = $"GetNGRMRByNGRCode 성공: NGR_CODE = {ngrCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetNGRMRByNGRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(NGR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 NGR 유지보수 데이터 조회
        public Result GetTotalNGRMaintenance(out List<NGRMaintenanceHistory> ngrMRList)
        {
            Result res = new Result(true);
            ngrMRList = new List<NGRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM NGR_MAINTENANCE_HISTORY";
                    ngrMRList = dbHelper.Conn.Query<NGRMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalNGRMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalNGRMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(NGR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetNGRMRDetailByNGRCode(string ngrCode, string tblIdx, out List<NGRMaintenanceHistory> ngrMRList)
        {
            Result res = new Result(true);
            ngrMRList = new List<NGRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM NGR_MAINTENANCE_HISTORY 
                WHERE NGR_CODE = @NGR_Code AND TBL_IDX = @Tbl_Idx";

                    ngrMRList = dbHelper.Conn.Query<NGRMaintenanceHistory>(query, new { NGR_Code = ngrCode, Tbl_Idx = tblIdx }).AsList();
                    if (ngrMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetNGRMRDetailByNGRCode 성공: NGR_CODE = {ngrCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetNGRMRDetailByNGRCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateNGRMRRepo(NGRMaintenanceHistory ngrMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO NGR_MAINTENANCE_HISTORY (
                    NGR_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @NGR_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, ngrMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "NGR 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "NGR 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateNGRMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateNGRMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // NGR 유지보수 데이터 업데이트
        public Result UpdateNGRMRRepo(NGRMaintenanceHistory ngrMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE NGR_MAINTENANCE_HISTORY
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
                WHERE NGR_CODE = @NGR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, ngrMR);
                    res.Message = affectedRows > 0 ? "NGR 유지보수 데이터 업데이트 성공" : "NGR 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateNGRMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // NGR 유지보수 데이터 삭제
        public Result DeleteNGRMRRepo(string ngrCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM NGR_MAINTENANCE_HISTORY WHERE NGR_CODE = @NGR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { NGR_Code = ngrCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "NGR 유지보수 데이터 삭제 성공" : "NGR 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteNGRMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

