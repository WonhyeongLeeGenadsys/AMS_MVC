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
    public class UserAccountRepository
    {
        //private SqlConnection conn;
        private readonly DBHelper mDb;

        /// <summary>
        /// 생성자에서 DB 연결
        /// </summary>
        public UserAccountRepository()
        {
            mDb = new DBHelper();
        }

        public UserAccount GetUserById(string userId)
        {
            var query = "SELECT * FROM USER_ACCOUNT WHERE ID = @Id";

            return mDb.Conn.QueryFirstOrDefault<UserAccount>(query, new { Id = userId });
        }

        /// <summary>
        /// 모든 유저 목록 가져오기
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Result GetAllUsers(out List<UserAccount> users)
        {
            Result res = new Result(true);
            users = new List<UserAccount>();

            try
            {
                var query = "SELECT * FROM USER_ACCOUNT";
                users = mDb.Conn.Query<UserAccount>(query).AsList();

                res.Message = "GetAllUsers 동작 성공";
                LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllUsers 실패: " + ex.StackTrace + ex.Message;

                LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
            }

            return res;
        }

        /// <summary>
        /// 유저 데이터 생성
        /// </summary>
        /// <param name="newUser"></param>
        // 유저 생성
        public Result CreateUser(UserAccount newUser)
        {
            Result res = new Result(true);
            try
            {
                var query = "INSERT INTO USER_ACCOUNT (ID, PW, USER_NAME, PERMISSION, BUSEO, PHONE_NO) " +
                            "VALUES (@Id, @Pw, @User_Name, @Permission, @Buseo, @Phone_No)";

                int affectedRows = mDb.Conn.Execute(query, newUser);
                if (affectedRows > 0)
                {
                    res.Message = "CreateUser 성공: User ID: " + newUser.Id;
                    LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "CreateUser 실패: 데이터 삽입에 실패했습니다.";
                    LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "CreateUser 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
            }
            return res;
        }

        // 유저 정보 수정
        public Result UpdateUser(UserAccount user)
        {
            Result res = new Result(true);
            try
            {
                var query = "UPDATE USER_ACCOUNT SET PW = @Pw, USER_NAME = @User_Name, PERMISSION = @Permission, BUSEO = @Buseo, PHONE_NO = @PhoneNo " +
                            "WHERE ID = @Id";

                int affectedRows = mDb.Conn.Execute(query, user);
                if (affectedRows > 0)
                {
                    res.Message = "UpdateUser 성공: User ID: " + user.Id;
                    LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "UpdateUser 실패: 데이터 수정에 실패했습니다.";
                    LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateUser 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
            }
            return res;
        }

        // 유저 삭제
        public Result DeleteUser(string userId)
        {
            Result res = new Result(true);
            try
            {
                var query = "DELETE FROM USER_ACCOUNT WHERE ID = @Id";

                int affectedRows = mDb.Conn.Execute(query, new { Id = userId });
                if (affectedRows > 0)
                {
                    res.Message = "DeleteUser 성공: User ID: " + userId;
                    LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "DeleteUser 실패: 해당 ID를 찾을 수 없습니다.";
                    LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteUser 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(USER_ACCOUNT)", res.Message);
            }
            return res;
        }
    }
}