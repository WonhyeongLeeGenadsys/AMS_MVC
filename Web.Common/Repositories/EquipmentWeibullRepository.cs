
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Web.Common
{
    public class EquipmentWeibullRepository
    {
        public List<EquipmentWeibull> GetAll()
        {
            var list = new List<EquipmentWeibull>();
            using (DBHelper db = new DBHelper())
            {
                string query = @"SELECT Id, Category, EquipmentName, ShapeParam, ScaleParam, FailureRate 
                                 FROM EquipmentWeibull";
                using (SqlCommand cmd = new SqlCommand(query, db.Conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var eq = new EquipmentWeibull
                            {
                                Id = reader.GetInt32(0),
                                Category = reader.GetString(1),
                                EquipmentName = reader.GetString(2),
                                ShapeParam = reader.IsDBNull(3) ? (double?)null : reader.GetDouble(3),
                                ScaleParam = reader.IsDBNull(4) ? (double?)null : reader.GetDouble(4),
                                FailureRate = reader.IsDBNull(5) ? (double?)null : reader.GetDouble(5)
                            };
                            list.Add(eq);
                        }
                    }
                }
            }
            return list;
        }
    }
}
