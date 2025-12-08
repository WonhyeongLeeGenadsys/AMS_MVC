
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class ARMREACTORMaintenanceRepository
    {

        public Result GetARMREACTORMRByARMREACTORCode(string armreactorCode, out List<ARMREACTORMaintenanceHistory> armreactorMRList)
        {
            Result res = new Result(true);
            armreactorMRList = new List<ARMREACTORMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM ARMREACTOR_MAINTENANCE_HISTORY 
                WHERE ARMREACTOR_CODE = @ARMREACTOR_Code";

                    armreactorMRList = dbHelper.Conn.Query<ARMREACTORMaintenanceHistory>(query, new { ARMREACTOR_Code = armreactorCode }).AsList();
                    res.Message = $"GetARMREACTORMRByARMREACTORCode 성공: ARMREACTOR_CODE = {armreactorCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetARMREACTORMRByARMREACTORCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ARMREACTOR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 ARMREACTOR 유지보수 데이터 조회
        public Result GetTotalARMREACTORMaintenance(out List<ARMREACTORMaintenanceHistory> armreactorMRList)
        {
            Result res = new Result(true);
            armreactorMRList = new List<ARMREACTORMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ARMREACTOR_MAINTENANCE_HISTORY";
                    armreactorMRList = dbHelper.Conn.Query<ARMREACTORMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalARMREACTORMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalARMREACTORMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ARMREACTOR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetARMREACTORMRDetailByARMREACTORCode(string armreactorCode, string tblIdx, out List<ARMREACTORMaintenanceHistory> armreactorMRList)
        {
            Result res = new Result(true);
            armreactorMRList = new List<ARMREACTORMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM ARMREACTOR_MAINTENANCE_HISTORY 
                WHERE ARMREACTOR_CODE = @ARMREACTOR_Code AND TBL_IDX = @Tbl_Idx";

                    armreactorMRList = dbHelper.Conn.Query<ARMREACTORMaintenanceHistory>(query, new { ARMREACTOR_Code = armreactorCode, Tbl_Idx = tblIdx }).AsList();
                    if (armreactorMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetARMREACTORMRDetailByARMREACTORCode 성공: ARMREACTOR_CODE = {armreactorCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetARMREACTORMRDetailByARMREACTORCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateARMREACTORMRRepo(ARMREACTORMaintenanceHistory armreactorMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO ARMREACTOR_MAINTENANCE_HISTORY (
                    ARMREACTOR_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @ARMREACTOR_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, armreactorMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "ARMREACTOR 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ARMREACTOR 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateARMREACTORMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateARMREACTORMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // ARMREACTOR 유지보수 데이터 업데이트
        public Result UpdateARMREACTORMRRepo(ARMREACTORMaintenanceHistory armreactorMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE ARMREACTOR_MAINTENANCE_HISTORY
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
                WHERE ARMREACTOR_CODE = @ARMREACTOR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, armreactorMR);
                    res.Message = affectedRows > 0 ? "ARMREACTOR 유지보수 데이터 업데이트 성공" : "ARMREACTOR 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateARMREACTORMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // ARMREACTOR 유지보수 데이터 삭제
        public Result DeleteARMREACTORMRRepo(string armreactorCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM ARMREACTOR_MAINTENANCE_HISTORY WHERE ARMREACTOR_CODE = @ARMREACTOR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ARMREACTOR_Code = armreactorCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "ARMREACTOR 유지보수 데이터 삭제 성공" : "ARMREACTOR 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteARMREACTORMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

