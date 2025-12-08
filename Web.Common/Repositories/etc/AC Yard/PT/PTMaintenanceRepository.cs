
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class PTMaintenanceRepository
    {

        public Result GetPTMRByPTCode(string ptCode, out List<PTMaintenanceHistory> ptMRList)
        {
            Result res = new Result(true);
            ptMRList = new List<PTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM PT_MAINTENANCE_HISTORY 
                WHERE PT_CODE = @PT_Code";

                    ptMRList = dbHelper.Conn.Query<PTMaintenanceHistory>(query, new { PT_Code = ptCode }).AsList();
                    res.Message = $"GetPTMRByPTCode 성공: PT_CODE = {ptCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPTMRByPTCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PT_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 PT 유지보수 데이터 조회
        public Result GetTotalPTMaintenance(out List<PTMaintenanceHistory> ptMRList)
        {
            Result res = new Result(true);
            ptMRList = new List<PTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM PT_MAINTENANCE_HISTORY";
                    ptMRList = dbHelper.Conn.Query<PTMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalPTMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalPTMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(PT_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetPTMRDetailByPTCode(string ptCode, string tblIdx, out List<PTMaintenanceHistory> ptMRList)
        {
            Result res = new Result(true);
            ptMRList = new List<PTMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM PT_MAINTENANCE_HISTORY 
                WHERE PT_CODE = @PT_Code AND TBL_IDX = @Tbl_Idx";

                    ptMRList = dbHelper.Conn.Query<PTMaintenanceHistory>(query, new { PT_Code = ptCode, Tbl_Idx = tblIdx }).AsList();
                    if (ptMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetPTMRDetailByPTCode 성공: PT_CODE = {ptCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetPTMRDetailByPTCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreatePTMRRepo(PTMaintenanceHistory ptMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO PT_MAINTENANCE_HISTORY (
                    PT_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @PT_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, ptMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "PT 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "PT 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreatePTMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreatePTMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // PT 유지보수 데이터 업데이트
        public Result UpdatePTMRRepo(PTMaintenanceHistory ptMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE PT_MAINTENANCE_HISTORY
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
                WHERE PT_CODE = @PT_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, ptMR);
                    res.Message = affectedRows > 0 ? "PT 유지보수 데이터 업데이트 성공" : "PT 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdatePTMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // PT 유지보수 데이터 삭제
        public Result DeletePTMRRepo(string ptCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM PT_MAINTENANCE_HISTORY WHERE PT_CODE = @PT_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { PT_Code = ptCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "PT 유지보수 데이터 삭제 성공" : "PT 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeletePTMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

