using AMS_MVC.Database;
using AMS_MVC.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS_MVC.Repositories
{
    public class EquipmentSearchRepository
    {
        /// <summary>
        /// dateType : "INSTALL"이면 Install_Date, "OPERATE"이면 Operating_Date 기준으로 검색
        /// 모든 장비 (VCB, ITR, DCCB, DCCABLE, SUBMODULE)를 검색하여 공통 포맷의 결과를 반환함.
        /// </summary>
        public List<dynamic> SearchAllEquipments(string dateType, DateTime start, DateTime end)
        {
            var results = new List<dynamic>();
            string dateColumn = (dateType.ToUpper() == "INSTALL") ? "Install_Date" : "Operating_Date";

            using (var db = new DBHelper())
            {
                // VCB 검색 쿼리
                string queryVCB = $@"
                    SELECT 'AC' as Category,
                           'VCB' as Sort,
                           b.VCB_Code as Code,
                           b.Serial_No,
                           b.Name,
                           b.Install_Date,
                           b.Operating_Date,
                           b.Price,
                           b.Install_Place,
                           b.Make_Company,
                           r.CoF, 
                           r.PoF, 
                           r.HI,
                           b.Rated_V,
                           b.Rated_A
                    FROM VCB_BASICINFO b
                    LEFT JOIN RISKMATRIX r ON b.VCB_Code = r.Code
                    WHERE b.{dateColumn} BETWEEN @Start AND @End
                ";

                // ITR 검색 쿼리
                string queryITR = $@"
                    SELECT 'AC' as Category,
                           'ITR' as Sort,
                           i.ITR_Code as Code,
                           i.Serial_No,
                           i.Name,
                           i.Install_Date,
                           i.Operating_Date,
                           i.Price,
                           i.Install_Place,
                           i.Make_Company,
                           r.CoF,
                           r.PoF,
                           r.HI,
                           i.Rated_V,
                           i.Rated_A
                    FROM INTERFACETR_BASICINFO i
                    LEFT JOIN RISKMATRIX r ON i.ITR_Code = r.Code
                    WHERE i.{dateColumn} BETWEEN @Start AND @End
                ";

                // DCCB 검색 쿼리
                string queryDCCB = $@"
                    SELECT 'DC' as Category,
                           'DCCB' as Sort,
                           d.DCCB_Code as Code,
                           d.Serial_No,
                           d.Name,
                           d.Install_Date,
                           d.Operating_Date,
                           d.Price,
                           d.Install_Place,
                           d.Make_Company,
                           r.CoF,
                           r.PoF,
                           r.HI,
                           d.Rated_V,
                           d.Rated_A
                    FROM DCCB_BASICINFO d
                    LEFT JOIN RISKMATRIX r ON d.DCCB_Code = r.Code
                    WHERE d.{dateColumn} BETWEEN @Start AND @End
                ";

                // DCCABLE 검색 쿼리
                string queryDCCABLE = $@"
                    SELECT 'DC' as Category,
                           'DCCABLE' as Sort,
                           c.DCCABLE_Code as Code,
                           c.Serial_No,
                           c.Name,
                           c.Install_Date,
                           c.Operating_Date,
                           c.Price,
                           c.Install_Place,
                           c.Make_Company,
                           r.CoF,
                           r.PoF,
                           r.HI,
                           c.Rated_V,
                           c.Rated_A
                    FROM DCCABLE_BASICINFO c
                    LEFT JOIN RISKMATRIX r ON c.DCCABLE_Code = r.Code
                    WHERE c.{dateColumn} BETWEEN @Start AND @End
                ";

                // SUBMODULE 검색 쿼리
                string querySUB = $@"
                    SELECT 'DC' as Category,
                           'SUBMODULE' as Sort,
                           s.SUBMODULE_Code as Code,
                           s.Serial_No,
                           s.Name,
                           s.Install_Date,
                           s.Operating_Date,
                           s.Price,
                           s.Install_Place,
                           s.Make_Company,
                           r.CoF,
                           r.PoF,
                           r.HI,
                           s.Rated_V,
                           s.Rated_A
                    FROM SUBMODULE_BASICINFO s
                    LEFT JOIN RISKMATRIX r ON s.SUBMODULE_Code = r.Code
                    WHERE s.{dateColumn} BETWEEN @Start AND @End
                ";

                var vcb = db.Conn.Query(queryVCB, new { Start = start, End = end }).ToList();
                var itr = db.Conn.Query(queryITR, new { Start = start, End = end }).ToList();
                var dccb = db.Conn.Query(queryDCCB, new { Start = start, End = end }).ToList();
                var dccable = db.Conn.Query(queryDCCABLE, new { Start = start, End = end }).ToList();
                var subm = db.Conn.Query(querySUB, new { Start = start, End = end }).ToList();

                results.AddRange(vcb);
                results.AddRange(itr);
                results.AddRange(dccb);
                results.AddRange(dccable);
                results.AddRange(subm);
            }

            return results;
        }
    }
}
