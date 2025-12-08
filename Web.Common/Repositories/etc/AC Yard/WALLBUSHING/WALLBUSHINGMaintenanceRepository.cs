
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class WALLBUSHINGMaintenanceRepository
    {

        public Result GetWALLBUSHINGMRByWALLBUSHINGCode(string wallbushingCode, out List<WALLBUSHINGMaintenanceHistory> wallbushingMRList)
        {
            Result res = new Result(true);
            wallbushingMRList = new List<WALLBUSHINGMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM WALLBUSHING_MAINTENANCE_HISTORY 
                WHERE WALLBUSHING_CODE = @WALLBUSHING_Code";

                    wallbushingMRList = dbHelper.Conn.Query<WALLBUSHINGMaintenanceHistory>(query, new { WALLBUSHING_Code = wallbushingCode }).AsList();
                    res.Message = $"GetWALLBUSHINGMRByWALLBUSHINGCode 성공: WALLBUSHING_CODE = {wallbushingCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetWALLBUSHINGMRByWALLBUSHINGCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(WALLBUSHING_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 WALLBUSHING 유지보수 데이터 조회
        public Result GetTotalWALLBUSHINGMaintenance(out List<WALLBUSHINGMaintenanceHistory> wallbushingMRList)
        {
            Result res = new Result(true);
            wallbushingMRList = new List<WALLBUSHINGMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM WALLBUSHING_MAINTENANCE_HISTORY";
                    wallbushingMRList = dbHelper.Conn.Query<WALLBUSHINGMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalWALLBUSHINGMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalWALLBUSHINGMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(WALLBUSHING_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetWALLBUSHINGMRDetailByWALLBUSHINGCode(string wallbushingCode, string tblIdx, out List<WALLBUSHINGMaintenanceHistory> wallbushingMRList)
        {
            Result res = new Result(true);
            wallbushingMRList = new List<WALLBUSHINGMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM WALLBUSHING_MAINTENANCE_HISTORY 
                WHERE WALLBUSHING_CODE = @WALLBUSHING_Code AND TBL_IDX = @Tbl_Idx";

                    wallbushingMRList = dbHelper.Conn.Query<WALLBUSHINGMaintenanceHistory>(query, new { WALLBUSHING_Code = wallbushingCode, Tbl_Idx = tblIdx }).AsList();
                    if (wallbushingMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetWALLBUSHINGMRDetailByWALLBUSHINGCode 성공: WALLBUSHING_CODE = {wallbushingCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetWALLBUSHINGMRDetailByWALLBUSHINGCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateWALLBUSHINGMRRepo(WALLBUSHINGMaintenanceHistory wallbushingMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO WALLBUSHING_MAINTENANCE_HISTORY (
                    WALLBUSHING_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @WALLBUSHING_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, wallbushingMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "WALLBUSHING 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "WALLBUSHING 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateWALLBUSHINGMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateWALLBUSHINGMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // WALLBUSHING 유지보수 데이터 업데이트
        public Result UpdateWALLBUSHINGMRRepo(WALLBUSHINGMaintenanceHistory wallbushingMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE WALLBUSHING_MAINTENANCE_HISTORY
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
                WHERE WALLBUSHING_CODE = @WALLBUSHING_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, wallbushingMR);
                    res.Message = affectedRows > 0 ? "WALLBUSHING 유지보수 데이터 업데이트 성공" : "WALLBUSHING 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateWALLBUSHINGMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // WALLBUSHING 유지보수 데이터 삭제
        public Result DeleteWALLBUSHINGMRRepo(string wallbushingCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM WALLBUSHING_MAINTENANCE_HISTORY WHERE WALLBUSHING_CODE = @WALLBUSHING_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { WALLBUSHING_Code = wallbushingCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "WALLBUSHING 유지보수 데이터 삭제 성공" : "WALLBUSHING 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteWALLBUSHINGMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

