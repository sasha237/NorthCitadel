using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalBot {
	class PBotInfo {
		//базовые параеметры:

		public int Level = 0; //уровень
		public int Experience = 0; //экспа
		public double Wellness = 0; //велл
		public double Gold = 0; //голд, обчно обрезано до целых
		public double GoldExact = 0; //точное число глда, актуально только после вызова LoadExcactCurrency()
		public double Currency = 0; //нац валюта, обчно обрезано до целых
		public double CurrencyExact = 0;  //точное число нац валюты, актуально только после вызова LoadExcactCurrency()
		public string CurrencyCode = ""; //код нац валюты
		public int CitizenID = 0; //ид
		public bool HasAvatar = false; //имеет авку
		public int Country = 0; //страна
		public string Email = null; //мыло
		public bool Activated = true; //активировал почту?

		public string FoodKey = ""; //токен для жрачки
		public string Tocken = ""; // токен для основных форм

		public int Today = 0; //тек номер дня ереп

		public List<InventoryItem> Inventory;

		//доп параметры, загружаются из БД:
		public string Login;
		public string Password;
		public int WorkDay; //когда работал
		public int TrainDay; //когда тренькал
		public int ReadDay; //когда читал газеты
		public bool WaitingCaptcha; //застрял на капче?
		public bool Banned; //забанен
		public bool Disabled; //задизаблен
	}

	class InventoryItem {
		public int Type; //тип предмета
		public int Q; //кушность
	}

	//принимаемые имена параметров:
	//food<Country>
	//getMaps
	//ConnectionString
	//group
	//skipCaptcha
	public static class BotConfig {
		public static string Get(string key) {
			return System.Configuration.ConfigurationSettings.AppSettings[key];
		}
	}
}
