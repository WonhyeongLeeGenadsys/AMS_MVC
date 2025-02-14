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
    public class CompanyRepository
    {
        private readonly DBHelper mDb;
        public CompanyRepository()
        {
            mDb = new DBHelper();
        }

        public string GetLatestComCode()
        {
            var query = "SELECT MAX(COM_CODE) FROM COMPANY";
            return mDb.Conn.QuerySingleOrDefault<string>(query);
        }
        public Company GetCompanyInfoByComCodeRepo(string comCode)
        {
            var query = "SELECT * FROM COMPANY WHERE COM_CODE = @Com_Code";
            return mDb.Conn.QueryFirstOrDefault<Company>(query, new { Com_Code = comCode });
        }
        private static string GenerateNextComCode(string latestCode)
        {
            int codeNumber;
            if (!int.TryParse(latestCode, out codeNumber))
            {
                codeNumber = 0;
            }
            codeNumber += 1;
            return codeNumber.ToString("D3"); // 001, 002 형식으로 변환
        }
        /// <summary>
        /// 모든 업체 목록 가져오기
        /// </summary>
        /// <param name="companies"></param>
        /// <returns></returns>
        public Result GetAllCompanies(out List<Company> companies)
        {
            companies = new List<Company>();
            Result result = new Result(true);
            try
            {
                string query = "SELECT * FROM COMPANY";
                companies = mDb.Conn.Query<Company>(query).AsList();

                result.Message = "업체 목록을 성공적으로 가져왔습니다.";
                LogHelper.WriteLog("DB(COMPANY)", result.Message);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "업체 목록 가져오기 실패: " + ex.Message;
                LogHelper.WriteLog("DB(COMPANY)", result.Message);
            }
            return result;
        }

        /// <summary>
        /// 업체 생성 메서드
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public Result CreateCompanyRepository(Company company)
        {
            Result result = new Result(true);
            try
            {
                string latestCode = GetLatestComCode();
                company.Com_Code = GenerateNextComCode(latestCode);

                string query = @"
            INSERT INTO COMPANY (COM_CODE, COM_NAME, COM_ADDRESS, COM_PHONE, COM_BUSEO, COM_EMAIL) 
            VALUES (@Com_Code, @Com_Name, @Com_Address, @Com_Phone, @Com_Buseo, @Com_Email)";

                int affectedRows = mDb.Conn.Execute(query, company);
                if (affectedRows > 0)
                {
                    result.Message = "업체가 성공적으로 생성되었습니다.";
                    LogHelper.WriteLog("DB(COMPANY) CreateCompany", result.Message);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "업체 생성 실패: 데이터 삽입에 실패했습니다.";
                    LogHelper.WriteLog("DB(COMPANY) CreateCompany", result.Message);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "업체 생성 실패: " + ex.Message;
                LogHelper.WriteLog("DB(COMPANY) CreateCompany", result.Message);
            }
            return result;
        }

        /// <summary>
        ///  업체 정보 수정
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public Result UpdateCompany(Company company)
        {
            Result result = new Result(true);

            try
            {
                string query = @"
                    UPDATE COMPANY 
                    SET COM_NAME = @Com_Name, 
                        COM_ADDRESS = @Com_Address, 
                        COM_PHONE = @Com_Phone, 
                        COM_BUSEO = @Com_Buseo, 
                        COM_EMAIL = @Com_Email
                    WHERE COM_CODE = @Com_Code";
                mDb.Conn.Execute(query, company);
                result.Message = "업체 정보가 성공적으로 수정되었습니다.";
                LogHelper.WriteLog("DB(COMPANY)", result.Message);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "업체 정보 수정 실패: " + ex.Message;
                LogHelper.WriteLog("DB(COMPANY)", result.Message);

            }
            return result;
        }

        /// <summary>
        /// 업체 삭제 메서드
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public Result DeleteCompany(string comCode)
        {
            Result result = new Result(true);
            try
            {
                string query = "DELETE FROM COMPANY WHERE COM_CODE = @Com_Code";
                int affectedRows = mDb.Conn.Execute(query, new { Com_Code = comCode });
                if (affectedRows > 0)
                {
                    result.Message = "업체가 성공적으로 삭제되었습니다.";
                    LogHelper.WriteLog("DB(COMPANY)", result.Message);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "업체 삭제 실패: 해당 회사 코드를 찾을 수 없습니다.";
                    LogHelper.WriteLog("DB(COMPANY)", result.Message);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "업체 삭제 실패: " + ex.Message;
                LogHelper.WriteLog("DB(COMPANY)", result.Message);
            }
            return result;
        }
    }
}


