
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class SAMaintenanceRepository
    {

        public Result GetSAMRBySACode(string saCode, out List<SAMaintenanceHistory> saMRList)
        {
            Result res = new Result(true);
            saMRList = new List<SAMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM SA_MAINTENANCE_HISTORY 
                WHERE SA_CODE = @SA_Code";

                    saMRList = dbHelper.Conn.Query<SAMaintenanceHistory>(query, new { SA_Code = saCode }).AsList();
                    res.Message = $"GetSAMRBySACode 성공: SA_CODE = {saCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSAMRBySACode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SA_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 SA 유지보수 데이터 조회
        public Result GetTotalSAMaintenance(out List<SAMaintenanceHistory> saMRList)
        {
            Result res = new Result(true);
            saMRList = new List<SAMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM SA_MAINTENANCE_HISTORY";
                    saMRList = dbHelper.Conn.Query<SAMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalSAMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalSAMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SA_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetSAMRDetailBySACode(string saCode, string tblIdx, out List<SAMaintenanceHistory> saMRList)
        {
            Result res = new Result(true);
            saMRList = new List<SAMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM SA_MAINTENANCE_HISTORY 
                WHERE SA_CODE = @SA_Code AND TBL_IDX = @Tbl_Idx";

                    saMRList = dbHelper.Conn.Query<SAMaintenanceHistory>(query, new { SA_Code = saCode, Tbl_Idx = tblIdx }).AsList();
                    if (saMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetSAMRDetailBySACode 성공: SA_CODE = {saCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSAMRDetailBySACode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateSAMRRepo(SAMaintenanceHistory saMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO SA_MAINTENANCE_HISTORY (
                    SA_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @SA_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, saMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "SA 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "SA 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateSAMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateSAMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // SA 유지보수 데이터 업데이트
        public Result UpdateSAMRRepo(SAMaintenanceHistory saMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE SA_MAINTENANCE_HISTORY
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
                WHERE SA_CODE = @SA_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, saMR);
                    res.Message = affectedRows > 0 ? "SA 유지보수 데이터 업데이트 성공" : "SA 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateSAMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // SA 유지보수 데이터 삭제
        public Result DeleteSAMRRepo(string saCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM SA_MAINTENANCE_HISTORY WHERE SA_CODE = @SA_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { SA_Code = saCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "SA 유지보수 데이터 삭제 성공" : "SA 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteSAMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

