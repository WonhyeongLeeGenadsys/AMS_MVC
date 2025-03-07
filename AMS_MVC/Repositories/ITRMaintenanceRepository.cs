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
    public class ITRMaintenanceRepository
    {
        public Result GetITRMRByITRCode(string itrCode, out List<ITRMaintenanceHistory> itrMRList)
        {
            Result res = new Result(true);
            itrMRList = new List<ITRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * FROM INTERFACETR_MAINTENANCE_HISTORY 
                WHERE ITR_CODE = @ITR_Code";

                    itrMRList = dbHelper.Conn.Query<ITRMaintenanceHistory>(query, new { ITR_Code = itrCode }).AsList();
                    res.Message = $"GetITRMRByVCBCode 성공: ITR_CODE = {itrCode}";
                }

            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRMRByITRCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }
        // 전체 VCB 유지보수 데이터 조회
        public Result GetTotalITRMaintenance(out List<ITRMaintenanceHistory> itrMRList)
        {
            Result res = new Result(true);
            itrMRList = new List<ITRMaintenanceHistory>();
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM INTERFACETR_MAINTENANCE_HISTORY";
                    itrMRList = dbHelper.Conn.Query<ITRMaintenanceHistory>(query).AsList();
                }
                res.Message = $"GetTotalITRMaintenance 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalITRMaintenance 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ITR_MAINTENANCE_HISTORY)", res.Message);
            }
            return res;
        }

        public Result GetITRMRDetailByITRCode(string itrCode, string maintenanceName, out List<ITRMaintenanceHistory> itrMRList)
        {
            Result res = new Result(true);
            itrMRList = new List<ITRMaintenanceHistory>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM INTERFACETR_MAINTENANCE_HISTORY 
                WHERE ITR_CODE = @ITR_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    itrMRList = dbHelper.Conn.Query<ITRMaintenanceHistory>(query, new { ITR_Code = itrCode, MR_Bosu_Name = maintenanceName }).AsList();
                    if (itrMRList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetVCBMRDetailByVCBCode 성공: ITR_CODE = {itrCode}, MR_BOSU_NAME = {maintenanceName}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetITRMRDetailByITRCode 실패: {ex.Message}";
            }

            return res;
        }
        public Result CreateITRMRRepo(ITRMaintenanceHistory vcbMR)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
        INSERT INTO INTERFACETR_MAINTENANCE_HISTORY (
        ITR_CODE, MR_BOSU_NAME, MR_WEATHER, MR_TEMP, MR_HUM, MR_CONTENT, MR_STATUS, MR_PART, MR_COMPANY, MR_WORKER, MR_MANAGER, MR_DATE, MR_WRITER 
        ) VALUES (@ITR_Code, @MR_Bosu_Name, @MR_Weather, @MR_Temp, @MR_Hum, @MR_Content, @MR_Status, @MR_Part, @MR_Company, @MR_Worker, @MR_Mananger, @MR_Date, @MR_Writer)";

                    int affectedRows = dbHelper.Conn.Execute(query, vcbMR);
                    if (affectedRows > 0)
                    {
                        res.Message = "ITR 유지보수 데이터 추가 성공";
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "ITR 유지보수 데이터 추가 실패: 데이터베이스 작업이 영향을 미치지 못함.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateITRMRRepo 실패: {ex.Message}";
                LogHelper.WriteLog("CreateITRMRRepo", $"오류 발생: {ex.Message}");
            }

            return res;
        }


        // VCB 유지보수 데이터 업데이트
        public Result UpdateITRMRRepo(ITRMaintenanceHistory itrMR)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE INTERFACETR_MAINTENANCE_HISTORY
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
                    MR_WRITER = @MR_Writer,
                WHERE ITR_CODE = @ITR_Code AND MR_Bosu_NAME = @MR_Bosu_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, itrMR);
                    res.Message = affectedRows > 0 ? "ITR 유지보수 데이터 업데이트 성공" : "ITR 유지보수 데이터 업데이트 실패";
                }
            }

            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateITRMRRepo 실패: {ex.Message}";
            }
            return res;
        }

        // VCB 유지보수 데이터 삭제
        public Result DeleteITRMRRepo(string itrCode, string bosuName)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM INTERFACETR_MAINTENANCE_HISTORY WHERE ITR_CODE = @ITR_Code AND MR_BOSU_NAME = @MR_Bosu_Name";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ITR_Code = itrCode, MR_Bosu_Name = bosuName });
                    res.Message = affectedRows > 0 ? "ITR 유지보수 데이터 삭제 성공" : "ITR 유지보수 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteITRMRRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}
