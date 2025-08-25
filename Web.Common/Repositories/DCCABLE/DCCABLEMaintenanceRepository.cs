
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCABLEMaintenanceRepository
    {

        public Result GetDCCABLEMRByDCCABLECode(string dccableCode, out List<DCCABLEMaintenanceHistory> dccableMRList)
        {
            Result res = new Result(true);
            dccableMRList = new List<DCCABLEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM DCCABLE_MAINTENANCE_HISTORY 
                WHERE DCCABLE_CODE = @DCCABLE_Code";

                    dccableMRList = dbHelper.Conn.Query<DCCABLEMaintenanceHistory>(query, new { DCCABLE_Code = dccableCode }).AsList();
                    res.Message = $"GetDCCABLEMRByDCCABLECode 성공: DCCABLE_CODE = {dccableCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCABLEMRByDCCABLECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCABLE_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }
        // 전체 DCCABLE 유지보수 데이터 조회
        public Result GetTotalDCCABLEMaintenance(out List<DCCABLEMaintenanceHistory> dccableMRList)
        {
            Result res = new Result(true);
            dccableMRList = new List<DCCABLEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCABLE_MAINTENANCE_HISTORY";
                    dccableMRList = dbHelper.Conn.Query<DCCABLEMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalDCCABLEMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCABLEMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCABLE_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetDCCABLEMRDetailByDCCABLECode(string dccableCode, string maintenanceName, out List<DCCABLEMaintenanceHistory> dccableMRList)
        {
            Result res = new Result(true);
            dccableMRList = new List<DCCABLEMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DCCABLE_MAINTENANCE_HISTORY 
                WHERE DCCABLE_CODE = @DCCABLE_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    dccableMRList = dbHelper.Conn.Query<DCCABLEMaintenanceHistory>(query, new { DCCABLE_Code = dccableCode, MR_Bosu_Name = maintenanceName }).AsList();
                    if (dccableMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCABLEMRDetailByDCCABLECode 성공: DCCABLE_CODE = {dccableCode}, MR_BOSU_NAME = {maintenanceName}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCABLEMRDetailByDCCABLECode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateDCCABLEMRRepo(DCCABLEMaintenanceHistory dccableMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
        INSERT INTO DCCABLE_MAINTENANCE_HISTORY (
        DCCABLE_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, MR_COMPANY, MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
        ) VALUES (@DCCABLE_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, @MR_Company, @MR_Worker, @MR_Mananger, @MR_Date, @MR_Writer)";

                    int affectedRows = dbHelper.Conn.Execute(query, dccableMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCABLE 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCABLE 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDCCABLEMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDCCABLEMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // DCCABLE 유지보수 데이터 업데이트
        public Result UpdateDCCABLEMRRepo(DCCABLEMaintenanceHistory dccableMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DCCABLE_MAINTENANCE_HISTORY
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
                WHERE DCCABLE_CODE = @DCCABLE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dccableMR);
                    res.Message = affectedRows > 0 ? "DCCABLE 유지보수 데이터 업데이트 성공" : "DCCABLE 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCABLEMRRepo 실패: {ex.Message}";
            }
            return res;
        }
        // DCCABLE 유지보수 데이터 삭제
        public Result DeleteDCCABLEMRRepo(string dccableCode, string bosuName)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DCCABLE_MAINTENANCE_HISTORY WHERE DCCABLE_CODE = @DCCABLE_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCABLE_Code = dccableCode, MR_Bosu_Name = bosuName });
                    res.Message = affectedRows > 0 ? "DCCABLE 유지보수 데이터 삭제 성공" : "DCCABLE 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCABLEMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

