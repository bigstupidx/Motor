
using Mono.Data.Sqlite;
using System;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient
{
    public class SpreeShowPointData
    {
        public ShowPoint ShowPoint;
        public int FreeSpreeID;
        public int RMBSpreeID;

        public SpreeShowPointData(SqliteDataReader reader)
        {
            ShowPoint = (ShowPoint) reader.GetValue("ShowPoint");
            FreeSpreeID = (int) reader.GetValue("Free");
            RMBSpreeID = (int) reader.GetValue("RMB");
        }

        public SpreeShowPointData(ShowPoint point, JArray json)
        {
            ShowPoint = point;
			FreeSpreeID = Convert.ToInt32( json[0].ToString());
			RMBSpreeID = Convert.ToInt32 (json[1].ToString());
        }

    }


}
