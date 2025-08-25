
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCTMaintenanceRepository
    {

        public Result GetDCCTMRByDCCTCode(string dcctCode, out List<DCCTMaintenanceHistory> dcctMRList)
        {
            Result res = new Result(true);
            dcctMRList = new List<DCCTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM DCCT_MAINTENANCE_HISTORY 
                WHERE DCCT_CODE = @DCCT_Code";

                    dcctMRList = dbHelper.Conn.Query<DCCTMaintenanceHistory>(query, new { DCCT_Code = dcctCode }).AsList();
                    res.Message = $"GetDCCTMRByDCCTCode 성공: DCCT_CODE = {dcctCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCTMRByDCCTCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCT_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 DCCT 유지보수 데이터 조회
        public Result GetTotalDCCTMaintenance(out List<DCCTMaintenanceHistory> dcctMRList)
        {
            Result res = new Result(true);
            dcctMRList = new List<DCCTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCT_MAINTENANCE_HISTORY";
                    dcctMRList = dbHelper.Conn.Query<DCCTMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalDCCTMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCTMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCT_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetDCCTMRDetailByDCCTCode(string dcctCode, string tblIdx, out List<DCCTMaintenanceHistory> dcctMRList)
        {
            Result res = new Result(true);
            dcctMRList = new List<DCCTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DCCT_MAINTENANCE_HISTORY 
                WHERE DCCT_CODE = @DCCT_Code AND TBL_IDX = @Tbl_Idx";

                    dcctMRList = dbHelper.Conn.Query<DCCTMaintenanceHistory>(query, new { DCCT_Code = dcctCode, Tbl_Idx = tblIdx }).AsList();
                    if (dcctMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCTMRDetailByDCCTCode 성공: DCCT_CODE = {dcctCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCTMRDetailByDCCTCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateDCCTMRRepo(DCCTMaintenanceHistory dcctMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO DCCT_MAINTENANCE_HISTORY (
                    DCCT_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @DCCT_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, dcctMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "DCCT 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DCCT 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDCCTMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateDCCTMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // DCCT 유지보수 데이터 업데이트
        public Result UpdateDCCTMRRepo(DCCTMaintenanceHistory dcctMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DCCT_MAINTENANCE_HISTORY
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
                WHERE DCCT_CODE = @DCCT_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dcctMR);
                    res.Message = affectedRows > 0 ? "DCCT 유지보수 데이터 업데이트 성공" : "DCCT 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCTMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // DCCT 유지보수 데이터 삭제
        public Result DeleteDCCTMRRepo(string dcctCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DCCT_MAINTENANCE_HISTORY WHERE DCCT_CODE = @DCCT_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCT_Code = dcctCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "DCCT 유지보수 데이터 삭제 성공" : "DCCT 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCTMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

