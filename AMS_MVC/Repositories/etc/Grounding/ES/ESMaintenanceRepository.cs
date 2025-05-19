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
    public class ESMaintenanceRepository
    {

        public Result GetESMRByESCode(string esCode, out List<ESMaintenanceHistory> esMRList)
        {
            Result res = new Result(true);
            esMRList = new List<ESMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM ES_MAINTENANCE_HISTORY 
                WHERE ES_CODE = @ES_Code";

                    esMRList = dbHelper.Conn.Query<ESMaintenanceHistory>(query, new { ES_Code = esCode }).AsList();
                    res.Message = $"GetESMRByESCode 성공: ES_CODE = {esCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetESMRByESCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ES_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 ES 유지보수 데이터 조회
        public Result GetTotalESMaintenance(out List<ESMaintenanceHistory> esMRList)
        {
            Result res = new Result(true);
            esMRList = new List<ESMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ES_MAINTENANCE_HISTORY";
                    esMRList = dbHelper.Conn.Query<ESMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalESMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalESMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ES_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetESMRDetailByESCode(string esCode, string tblIdx, out List<ESMaintenanceHistory> esMRList)
        {
            Result res = new Result(true);
            esMRList = new List<ESMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM ES_MAINTENANCE_HISTORY 
                WHERE ES_CODE = @ES_Code AND TBL_IDX = @Tbl_Idx";

                    esMRList = dbHelper.Conn.Query<ESMaintenanceHistory>(query, new { ES_Code = esCode, Tbl_Idx = tblIdx }).AsList();
                    if (esMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetESMRDetailByESCode 성공: ES_CODE = {esCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetESMRDetailByESCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateESMRRepo(ESMaintenanceHistory esMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO ES_MAINTENANCE_HISTORY (
                    ES_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @ES_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, esMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "ES 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ES 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateESMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateESMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // ES 유지보수 데이터 업데이트
        public Result UpdateESMRRepo(ESMaintenanceHistory esMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE ES_MAINTENANCE_HISTORY
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
                WHERE ES_CODE = @ES_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, esMR);
                    res.Message = affectedRows > 0 ? "ES 유지보수 데이터 업데이트 성공" : "ES 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateESMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // ES 유지보수 데이터 삭제
        public Result DeleteESMRRepo(string esCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM ES_MAINTENANCE_HISTORY WHERE ES_CODE = @ES_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ES_Code = esCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "ES 유지보수 데이터 삭제 성공" : "ES 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteESMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

