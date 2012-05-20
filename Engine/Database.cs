using System;
using System.Data;
using System.Collections.Generic;
using NerZul.Core.Utils;

	
namespace Engine
{
	public class DbRows:System.Collections.Generic.List<DbRow>
	{
        public static DbRows MixList(DbRows inputList)
        {
            DbRows randomList = new DbRows();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }
	}

	public class DbRow:System.Collections.Generic.Dictionary<string,object>
	{		
		
	}

	public class Database
	{
		private System.Collections.Generic.List<string> WhereArgs=
			new System.Collections.Generic.List<string>();
		
		private string WhereString;
        private string OrderString;
        public System.Data.IDbConnection DbConnection;
		public void ConnectToDb()
		{

			MySql.Data.MySqlClient.MySqlConnectionStringBuilder builder=
				new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            string dbname = Globals.Config.GetValue("database", "database", "erepublik");
			//if(dbname==null) throw new ArgumentException("Specify database->database in configuration");
			builder.Database=dbname;
			builder.Server=Globals.Config.GetValue("database","host","127.0.0.1");
			builder.Port=(uint)Globals.Config.GetValue("database", "port", 3306);
			builder.UserID =Globals.Config.GetValue("database", "user", "root");
			string password=Globals.Config.GetValue("database","password",(string)null);
			if (password!=null) builder.Password=password;
			DbConnection=new MySql.Data.MySqlClient.MySqlConnection(builder.ConnectionString);
			DbConnection.Open();
            CreateRows();
		}
		
		public void Where(string Column, object Value, bool Escape)
		{
			Where(Column,Value,"=",Escape);
		}
		public void Where(string Column, object Value, string Relation)
		{
			Where(Column,Value,Relation,true);
		}
		public void Where(string Column, object Value)
		{
			Where(Column,Value,"=");
		}
		public void Where(string Column, object Value, string Relation, bool Escape)
		{
			string arg=" `"+Column+"` "+Relation;
			string Value2=Value.ToString();
			
			if(Escape) Value2="\""+MySql.Data.MySqlClient.MySqlHelper.EscapeString(Value2)+"\"";
			arg+=" "+Value2+" ";
			WhereArgs.Add(arg);
		}
		public void Where(string where)
		{
			if(WhereString!=null) WhereString+=" AND ";
			else WhereString="";
			WhereString+=where;
		}

        public void Order(string order)
        {
            if (OrderString != null) 
                OrderString += ", ";
            else 
                OrderString = "";
            OrderString += order;
        }

		private string BuildWhere()
		{
			if((WhereString==null)&&(WhereArgs.Count==0)) return "";
			string wheres="";
			foreach(string arg in WhereArgs)
			{
				if(wheres.Length!=0) wheres+="AND ";
				wheres+=arg+" ";
			}
			if(WhereString!= null)
			{
				if(wheres.Length!=0) wheres+="AND ";
				wheres+=WhereString;
			}

            wheres = "WHERE "+ wheres;
            //ConsoleLog.WriteLine("Where: " + wheres);
			return wheres;
		}
		public DbRows Select(string From, params string[] What)
		{

			DbRows rv=new DbRows();
			string Command="SELECT distinct ";
			if(What.Length==0) Command+= "* ";
			bool NeedKoma=false;
			foreach(string arg in What)
			{
				if (NeedKoma) Command+=", ";
				Command+=arg;
				NeedKoma=true;
			}
			Command+=" from " +From;
			Command+=" "+BuildWhere();
            if (!String.IsNullOrEmpty(OrderString))
                Command += " order by " + OrderString;
            
//#if PUBLIC_BUILD

//#endif
            IDbCommand cmd = DbConnection.CreateCommand();
			cmd.CommandText=Command;
			IDataReader reader=cmd.ExecuteReader();
			while (reader.Read())
			{
				DbRow row=new DbRow();
				for(int i=0; i<reader.FieldCount;i++)
				{
					row.Add(reader.GetName(i),reader.GetValue(i));
				}
				rv.Add(row);
			}
			reader.Close();
			return rv;
			
		}
	
		public Dictionary<string,string> PrepareVarsFromParamArray(object[] Values)
		{

			if(Values.Length==0) throw new ArgumentNullException();
			Dictionary<string,string> rv=new Dictionary<string, string>();	
			int i=0;
			while(i<Values.Length)
			{
				string column=(string) Values[i];
				if(i+1>Values.Length) throw new ArgumentException("No matching key for column \""+
					column+"\"");
				string val=Values[i+1].ToString();
				i+=2;
				//Check for boolean arg
				bool needesc=true;
				if((i<Values.Length)&&(Values[i].GetType()==typeof(bool)))
				{
					needesc= (bool)Values[i];
					i++;
				}
				if(needesc) val="\""+MySql.Data.MySqlClient.MySqlHelper.EscapeString(val)+"\"";
				rv.Add(column,val);
			}
			return rv;
			
		}
		//Insert("table", "colname", value, [NeedEscape=true,] "colname", value, ...) 
		public void Insert(string Into, params object[] Values)
		{
			String columns="";
			String values="";
			Dictionary<string,string> arglst=PrepareVarsFromParamArray(Values);
			
			//Add saparators, if needed
			foreach(string column in arglst.Keys)
			{
				if(columns.Length!=0)
				{
					
					columns+=", ";
					values+=", ";
				}
				columns+="`"+column+"`";
				values+=arglst[column];
			}
			string command="INSERT INTO "+Into+" ("+columns+") VALUES ("+values+")";
			IDbCommand cmd= DbConnection.CreateCommand();
			cmd.CommandText=command;
			cmd.ExecuteNonQuery();
		}
		public int Update(string Table,params object[] args)
		{
			var updargs=PrepareVarsFromParamArray(args);
			string query="";
			foreach (var arg in updargs)
			{
				if(query.Length!=0) query+=", ";
				query+=arg.Key+"="+arg.Value;
			}
			query="UPDATE "+Table+" SET "+query+" "+BuildWhere();
			IDbCommand cmd=DbConnection.CreateCommand();
			cmd.CommandText=query;
			return cmd.ExecuteNonQuery();
		}

		public int Update(string Table, DbRow row)
		{
			object[] args=new object[row.Count*2];
			int c=0;
			foreach(var vals in row)
			{
				args[c*2]=vals.Key;
				args[c*2+1]=vals.Value;
			}
			return Update(Table,args);
		}
		public void Reset()
		{
			WhereArgs.Clear();
			WhereString = null;
            OrderString = null;
		}

        public void CreateRows()
        {
            AddRow("`last_day_work` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`last_day_train` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`last_day_study` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`last_day_relax` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`last_day_fight` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`last_day_vote` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`disabled` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`banned` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`industry` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`activated` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`country` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`gold` FLOAT NOT NULL DEFAULT '0'");
            AddRow("`nat_occur` FLOAT NOT NULL DEFAULT '0'");
            AddRow("`add_info01` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`email` varchar(255) collate latin1_general_ci NOT NULL default ''");
            AddRow("`food_qty` INT(10) NOT NULL DEFAULT '0'");
            AddRow("`items_qty` INT(10) NOT NULL DEFAULT '0'");
        }
        void AddRow(string str)
        {
            try
            {
                IDbCommand cmd = DbConnection.CreateCommand();
                cmd.CommandText = "ALTER TABLE `bots` ADD" + str;
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
                //ConsoleLog.WriteLine("Error: " + e.Message);
                // Здесь выводит плановые ошибки типа "колонка уже есть".
            }
        }

	}
}

