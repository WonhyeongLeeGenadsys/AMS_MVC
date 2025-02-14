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
    public class VCBBasicInfoRepository
    {
        private readonly DBHelper mDb;

        public VCBBasicInfoRepository()
        {
            mDb = new DBHelper();
        }
        // 가장 큰 VCB_CODE 값을 반환
        public string GetLatestVCBCode()
        {
            var query = "SELECT MAX(VCB_CODE) FROM VCB_BASICINFO WHERE VCB_CODE LIKE 'V%'";
            return mDb.Conn.QuerySingleOrDefault<string>(query);
        }

        public VCBBasicInfo GetVCBBasicInfoByTblIdxRepo(string tblIdx)
        {
            var query = "SELECT * FROM VCB_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

            return mDb.Conn.QueryFirstOrDefault<VCBBasicInfo>(query, new { Tbl_Idx = tblIdx });
        }
        /// <summary>
        /// VCB 기본정보 전체 불러오기
        /// </summary>
        /// <param name="vcbBasicInfo"></param>
        /// <returns></returns>
        public Result GetAllVCBBasicInfoRepo(out List<VCBBasicInfo> vcbBasicInfo)
        {
            Result res = new Result(true);
            vcbBasicInfo = new List<VCBBasicInfo>();

            try
            {
                var query = "SELECT TBL_IDX, VCB_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, IS_DIAGNOSTICS, IS_HEALTH, WRITER, TBL_GETDATE FROM VCB_BASICINFO";
                vcbBasicInfo = mDb.Conn.Query<VCBBasicInfo>(query).AsList();

                LogHelper.WriteLog("vcbBasicInfo Data", $"{vcbBasicInfo}");
                res.Message = "GetAllVCBBasicInfoRepo 동작 성공";
                LogHelper.WriteLog("DB(VCB_BASICINFO", res.Message);
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllVCBBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;

                LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result CreateVCBBasicInfoRepo(VCBBasicInfo newVCBBasicInfo)
        {
            Result res = new Result(true);
            using (var conn = mDb.Conn)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // VCB_BASICINFO 테이블에 데이터 삽입
                        var queryBasicInfo = @"
                INSERT INTO VCB_BASICINFO (VCB_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                IS_HEALTH, WRITER) 
                VALUES (@VCB_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                @Is_Health, @Writer)";

                        int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newVCBBasicInfo, transaction);

                        if (affectedRowsBasicInfo > 0)
                        {
                            // RISKMATRIX 테이블에 데이터 삽입
                            var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@VCB_Code, @DefaultCof, @DefaultPof)";

                            // 초기 COF와 POF 값은 기본값으로 설정 (변경해야됨)
                            var riskMatrixData = new
                            {
                                VCB_Code = newVCBBasicInfo.VCB_Code,
                                DefaultCof = "0",
                                DefaultPof = "0"
                            };

                            int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                            if (affectedRowsRiskMatrix > 0)
                            {
                                // 트랜잭션 커밋
                                transaction.Commit();
                                res.Message = "CreateVCBBasicInfoRepo 성공: VCB Serial_No: " + newVCBBasicInfo.Serial_No;
                                LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                            }
                            else
                            {
                                throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                            }
                        }
                        else
                        {
                            throw new Exception("VCB_BASICINFO 테이블에 데이터 삽입 실패");
                        }
                    }
                    catch (Exception ex)
                    {
                        // 트랜잭션 롤백
                        transaction.Rollback();
                        res.IsSuccess = false;
                        res.Message = "CreateVCBBasicInfoRepo 실패: " + ex.Message;
                        LogHelper.WriteLog("DB(VCB_BASICINFO)", "CreateVCBBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                    }
                }
            }
            return res;
        }

        public Result UpdateVCBBasicInfoRepo(VCBBasicInfo vcbBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                var query = "UPDATE VCB_BASICINFO SET NAME = @Name, INSTALL_DATE = @Install_Date, OPERATING_DATE = @Operating_Date, PRICE=@Price, INSTALL_PLACE=@Install_Place, CAPACITY=@Capacity, RATED_V=@Rated_V, RATED_A=@Rated_A, MAKE_COMPANY=@Make_Company, MAKE_NO=@Make_No, PHOTO=@Photo, IS_DIAGNOSTICS=@Is_Diagnostics, IS_HEALTH=@Is_Health, WRITER=@Writer " +
                            "WHERE SERIAL_NO = @Serial_No";

                int affectedRows = mDb.Conn.Execute(query, vcbBasicInfo);
                if (affectedRows > 0)
                {
                    res.Message = "UpdateVCBBasicInfoRepo 성공 SERIAL_NO: " + vcbBasicInfo.Serial_No;
                    LogHelper.WriteLog("DB(VCB_BasicInfo)", res.Message);
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "UpdateVCBBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                    LogHelper.WriteLog("DB(VCB_BasicInfo)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateVCBBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(VCB_BasicInfo)", res.Message);
            }
            return res;
        }

        public Result DeleteVCBBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                var query = "DELETE FROM VCB_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

                int affectedRows = mDb.Conn.Execute(query, new { Tbl_Idx = tblIdx });
                if (affectedRows > 0)
                {
                    res.Message = "DeleteVCBBasicInfoRepo 성공: VCBBasicInfo Tbl_Idx: " + tblIdx;
                    LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "DeleteVCBBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                    LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteVCBBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}