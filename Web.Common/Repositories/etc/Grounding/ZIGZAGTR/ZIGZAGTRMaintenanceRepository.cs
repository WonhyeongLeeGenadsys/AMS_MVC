
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class ZIGZAGTRMaintenanceRepository
    {

        public Result GetZIGZAGTRMRByZIGZAGTRCode(string zigzagtrCode, out List<ZIGZAGTRMaintenanceHistory> zigzagtrMRList)
        {
            Result res = new Result(true);
            zigzagtrMRList = new List<ZIGZAGTRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM ZIGZAGTR_MAINTENANCE_HISTORY 
                WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code";

                    zigzagtrMRList = dbHelper.Conn.Query<ZIGZAGTRMaintenanceHistory>(query, new { ZIGZAGTR_Code = zigzagtrCode }).AsList();
                    res.Message = $"GetZIGZAGTRMRByZIGZAGTRCode 성공: ZIGZAGTR_CODE = {zigzagtrCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetZIGZAGTRMRByZIGZAGTRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ZIGZAGTR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 ZIGZAGTR 유지보수 데이터 조회
        public Result GetTotalZIGZAGTRMaintenance(out List<ZIGZAGTRMaintenanceHistory> zigzagtrMRList)
        {
            Result res = new Result(true);
            zigzagtrMRList = new List<ZIGZAGTRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ZIGZAGTR_MAINTENANCE_HISTORY";
                    zigzagtrMRList = dbHelper.Conn.Query<ZIGZAGTRMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalZIGZAGTRMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalZIGZAGTRMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ZIGZAGTR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetZIGZAGTRMRDetailByZIGZAGTRCode(string zigzagtrCode, string tblIdx, out List<ZIGZAGTRMaintenanceHistory> zigzagtrMRList)
        {
            Result res = new Result(true);
            zigzagtrMRList = new List<ZIGZAGTRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM ZIGZAGTR_MAINTENANCE_HISTORY 
                WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code AND TBL_IDX = @Tbl_Idx";

                    zigzagtrMRList = dbHelper.Conn.Query<ZIGZAGTRMaintenanceHistory>(query, new { ZIGZAGTR_Code = zigzagtrCode, Tbl_Idx = tblIdx }).AsList();
                    if (zigzagtrMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetZIGZAGTRMRDetailByZIGZAGTRCode 성공: ZIGZAGTR_CODE = {zigzagtrCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetZIGZAGTRMRDetailByZIGZAGTRCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateZIGZAGTRMRRepo(ZIGZAGTRMaintenanceHistory zigzagtrMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO ZIGZAGTR_MAINTENANCE_HISTORY (
                    ZIGZAGTR_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @ZIGZAGTR_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, zigzagtrMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "ZIGZAGTR 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ZIGZAGTR 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateZIGZAGTRMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateZIGZAGTRMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // ZIGZAGTR 유지보수 데이터 업데이트
        public Result UpdateZIGZAGTRMRRepo(ZIGZAGTRMaintenanceHistory zigzagtrMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE ZIGZAGTR_MAINTENANCE_HISTORY
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
                WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, zigzagtrMR);
                    res.Message = affectedRows > 0 ? "ZIGZAGTR 유지보수 데이터 업데이트 성공" : "ZIGZAGTR 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateZIGZAGTRMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // ZIGZAGTR 유지보수 데이터 삭제
        public Result DeleteZIGZAGTRMRRepo(string zigzagtrCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM ZIGZAGTR_MAINTENANCE_HISTORY WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ZIGZAGTR_Code = zigzagtrCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "ZIGZAGTR 유지보수 데이터 삭제 성공" : "ZIGZAGTR 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteZIGZAGTRMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

