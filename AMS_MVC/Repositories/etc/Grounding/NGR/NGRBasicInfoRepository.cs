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
    public class NGRBasicInfoRepository
    {
        // 가장 큰 NGR_CODE 값을 반환
        public string GetLatestNGRCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(NGR_CODE) FROM NGR_BASICINFO WHERE NGR_CODE LIKE 'NGR%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public NGRBasicInfo GetNGRBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM NGR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<NGRBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public NGRBasicInfo GetNGRBasicInfoByCode(string ngrCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM NGR_BASICINFO WHERE NGR_CODE = @NGR_Code";
                return dbHelper.Conn.QueryFirstOrDefault<NGRBasicInfo>(query, new { NGR_Code = ngrCode });
            }
        }

        /// <summary>
        /// NGR 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllNGRBasicInfoRepo(out List<NGRBasicInfo> ngrBasicInfo)
        {
            Result res = new Result(true);
            ngrBasicInfo = new List<NGRBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, NGR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, WRITER, TBL_GETDATE 
                                  FROM NGR_BASICINFO";
                    ngrBasicInfo = dbHelper.Conn.Query<NGRBasicInfo>(query).AsList();

                    LogHelper.WriteLog("ngrBasicInfo Data", $"총 {ngrBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllNGRBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllNGRBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
            }
            return res;
        }

        //NGR Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllNGRBasicInfoWithRiskMatrixRepo(out List<dynamic> ngrInfoWithRisk)
        {
            Result res = new Result(true);
            ngrInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.NGR_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM NGR_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.NGR_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    ngrInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "NGR 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateNGRBasicInfoRepo(NGRBasicInfo newNGRBasicInfo)
        {
            Result res = new Result(true);
            using (DBHelper dbHelper = new DBHelper())
            {
                using (var conn = dbHelper.Conn)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // NGR_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO NGR_BASICINFO (NGR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, WRITER) 
                VALUES (@NGR_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newNGRBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@NGR_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    NGR_Code = newNGRBasicInfo.NGR_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateNGRBasicInfoRepo 성공: NGR Serial_No: " + newNGRBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("NGR_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateNGRBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(NGR_BASICINFO)", "CreateNGRBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateNGRBasicInfoRepo(NGRBasicInfo ngrBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // NGR_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE NGR_BASICINFO
            SET 
                NAME = @Name, 
                INSTALL_DATE = @Install_Date, 
                OPERATING_DATE = @Operating_Date, 
                PRICE = @Price, 
                INSTALL_PLACE = @Install_Place, 
                CAPACITY = @Capacity, 
                RATED_V = @Rated_V, 
                RATED_A = @Rated_A, 
                MAKE_COMPANY = @Make_Company, 
                MAKE_NO = @Make_No, 
                PHOTO = @Photo, 
                IS_DIAGNOSTICS = @Is_Diagnostics, 
                IS_HEALTH = @Is_Health, 
                WRITER = @Writer
            WHERE NGR_CODE = @NGR_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, ngrBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateNGRBasicInfoRepo 성공. NGR_CODE: " + ngrBasicInfo.NGR_Code;
                        LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateNGRBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateNGRBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteNGRBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM NGR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteNGRBasicInfoRepo 성공: NGRBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteNGRBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteNGRBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(NGR_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
