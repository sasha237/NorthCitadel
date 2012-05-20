using System;
using NerZul.Core.Utils;
namespace Engine
{
	public static class Utils
	{
		
		public static DbRow UpdateDbWithBasicInfo(uint id, NerZul.Core.Utils.BasicBotInfo nfo)
		{
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return null;
//#endif

			var Database=Globals.Database;
			lock (Globals.DBLocker)
			{
				Database.Reset();
				Database.Where("id",id);
				Database.Update("bots",
                                "citizen_id",nfo.CitizenID,
				                "experience",nfo.Experience,
				                "wellness",nfo.Wellness,
                                "country",nfo.Country,
                                "gold",nfo.Gold,
                                "nat_occur",nfo.Nat_occur);
				var DbRow=Database.Select("bots")[0];
				Database.Reset();
				return DbRow; 
			}
		}

        public static DbRow UpdateDbWithStorageInfo(uint id, NerZul.Core.Utils.StorageBotInfo nfo)
        {
            //#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return null;
            //#endif

            var Database = Globals.Database;
            lock (Globals.DBLocker)
            {
                Database.Reset();
                Database.Where("id", id);
                Database.Update("bots",
                                "food_qty", nfo.foodQty,
                                "items_qty", nfo.itemsQty);
                var DbRow = Database.Select("bots")[0];
                Database.Reset();
                return DbRow;
            }
        }

        public static DbRow TryToUpdateDbWithBasicInfo(DbRow botinfo, ManagedCitizen bot)
		{
			if(!bot.GetInfoFromCommonResponse()) return botinfo;
			return UpdateDbWithBasicInfo((uint)botinfo["id"],bot.Info);
		}
        public static DbRow TryToUpdateDbWithCurrentInfo(DbRow botinfo, ManagedCitizen bot)
        {
            return UpdateDbWithBasicInfo((uint)botinfo["id"], bot.Info);
        }
        public static DbRow TryToUpdateDbWithStorageInfo(DbRow botinfo, ManagedCitizen bot)
        {
            return UpdateDbWithStorageInfo((uint)botinfo["id"], bot.storageInfo);
        }
        public static void UpdateDbWithFullBotInfo(DbRow info)
		{
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

			lock (Globals.DBLocker)
			{
				Globals.Database.Reset();
				Globals.Database.Where("id",(uint)info["id"]);
				Globals.Database.Update("bots",info);
			}
		}
		public static void UpdateDbWithCustomBotInfo(DbRow info, params String[] fields)
		{
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

			lock (Globals.DBLocker)
			{
				Globals.Database.Reset();
				Globals.Database.Where("id",(uint)info["id"]);
				object[] args=new object[fields.Length*2];
				for(int i=0;i<fields.Length;i++)
				{
					args[i*2]=fields[i];
					args[i*2+1]=info[fields[i]];
				}
				Globals.Database.Update("bots",args);
			}
		}
	}
}

