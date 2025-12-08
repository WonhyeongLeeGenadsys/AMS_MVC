
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class VCBMaintenanceRepository
    {

        public Result GetVCBMRByVCBCode(string vcbCode, out List<VCBMaintenanceHistory> vcbMRList)
        {
            Result res = new Result(true);
            vcbMRList = new List<VCBMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM VCB_MAINTENANCE_HISTORY 
                WHERE VCB_CODE = @VCB_Code";

                    vcbMRList = dbHelper.Conn.Query<VCBMaintenanceHistory>(query, new { VCB_Code = vcbCode }).AsList();
                    res.Message = $"GetVCBMRByVCBCode 성공: VCB_CODE = {vcbCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBMRByVCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(VCB_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        // 전체 VCB 유지보수 데이터 조회
        public Result GetTotalVCBMaintenance(out List<VCBMaintenanceHistory> vcbMRList)
        {
            Result res = new Result(true);
            vcbMRList = new List<VCBMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM VCB_MAINTENANCE_HISTORY";
                    vcbMRList = dbHelper.Conn.Query<VCBMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalVCBMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalVCBMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(VCB_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetVCBMRDetailByVCBCode(string vcbCode, string tblIdx, out List<VCBMaintenanceHistory> vcbMRList)
        {
            Result res = new Result(true);
            vcbMRList = new List<VCBMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM VCB_MAINTENANCE_HISTORY 
                WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";

                    vcbMRList = dbHelper.Conn.Query<VCBMaintenanceHistory>(query, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx }).AsList();
                    if (vcbMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetVCBMRDetailByVCBCode 성공: VCB_CODE = {vcbCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBMRDetailByVCBCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateVCBMRRepo(VCBMaintenanceHistory vcbMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO VCB_MAINTENANCE_HISTORY (
                    VCB_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, 
                    MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
                ) VALUES (
                    @VCB_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, 
                    @MR_Worker, @MR_Manager, @MR_Date, @MR_Writer
                )";

                    int affectedRows = dbHelper.Conn.Execute(query, vcbMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "VCB 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "VCB 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateVCBMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateVCBMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // VCB 유지보수 데이터 업데이트
        public Result UpdateVCBMRRepo(VCBMaintenanceHistory vcbMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE VCB_MAINTENANCE_HISTORY
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
                WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, vcbMR);
                    res.Message = affectedRows > 0 ? "VCB 유지보수 데이터 업데이트 성공" : "VCB 유지보수 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateVCBMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // VCB 유지보수 데이터 삭제
        public Result DeleteVCBMRRepo(string vcbCode, string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM VCB_MAINTENANCE_HISTORY WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "VCB 유지보수 데이터 삭제 성공" : "VCB 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteVCBMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}

