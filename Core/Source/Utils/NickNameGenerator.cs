
using System;
using System.Collections.Generic;
using NerZul.Core.Network;

namespace NerZul.Core.Utils
{
    public enum GeneratorTypes
    {
        normalGenerator = 1,
        webLoader = 2
    }

    public class RegMail
    {
        public string mailBox;
        public string popServer;
        public string login;
        public string password;
        public bool processed;
        public bool activated;

        public RegMail(string data)
        {
            string[] info = data.Split(';');

            if (info.Length < 4 || info.Length > 6)
            {
                throw new Exception(
                    "Incorrect string: " + data + 
                    Environment.NewLine +
                    "Format should be: mail_box;pop_server;login;password[;processed[;activated]]");
            }

            mailBox = info[0];
            popServer = info[1];
            login = info[2];
            password = info[3];

            processed = false;
            if (info.Length > 4)
                processed = (info[4] == "processed");
            activated = false;
            if (info.Length > 5)
                activated = (info[5] == "activated");
        }

        public string AsString()
        {
            string result = 
                mailBox + ';' +
                popServer + ';' +
                login + ';' +
                password;

            if (processed)
            {
                result += ";processed";
                if (activated)
                {
                    result += ";activated";
                }
            }

            return result;
        }
    }

    public class RegMails
    {
        public string fileName;
        public bool inited = false;
        public List<RegMail> boxes = new List<RegMail>();

        public void LoadFromFile(string fileName)
        {
            this.fileName = fileName;

            foreach (string line in System.IO.File.ReadAllLines(fileName))
            {
                RegMail mail = new RegMail(line);
                boxes.Add(mail);
            }

            inited = true;
        }

        public void SaveToFile()
        {
            List<string> lines = new List<string>();

            foreach(var regMail in boxes)
                lines.Add(regMail.AsString());

            System.IO.File.WriteAllLines(fileName, lines.ToArray());
        }
    }

	public class NickNameAndPasswordGenerator
	{
		private string [] m_Dic;
		private System.Random m_Random=new System.Random();
        private GeneratorTypes generatorType = 0;

        private RegMails regMails = new RegMails();

		public NickNameAndPasswordGenerator (string[] Dictionary)
		{
			if(Dictionary==null) throw new ArgumentNullException();
			if (Dictionary.GetLength(0)<2) throw new ArgumentException();
			m_Dic=Dictionary;
            generatorType = GeneratorTypes.normalGenerator;
		}
		public NickNameAndPasswordGenerator(string DictionaryPath):
			this(System.IO.File.ReadAllLines(DictionaryPath)){}

        public NickNameAndPasswordGenerator(string webURL, int flag)
        {
            webSource = webURL;
            generatorType = GeneratorTypes.webLoader;
        }

		private string[] m_names = null;
		private string[] m_surnames = null;

        private string webSource = null;
        private List<string> webNames = new List<string>();
        private HttpClient httpClient;

		public string GenerateNick()
		{
            if (generatorType == GeneratorTypes.normalGenerator)
            {
                if (m_names == null)
                {
                    m_names = System.IO.File.ReadAllLines("data/dic_names.txt");
                }
                if (m_surnames == null)
                {
                    m_surnames = System.IO.File.ReadAllLines("data/dic_surnames.txt");
                }
                int type = m_Random.Next(0, 5);
                string s = "";
                int n;
                switch (type)
                {
                    case 0: //тип: ник+цифры
                        s = m_Dic[m_Random.Next(m_Dic.Length)].Trim();
                        //добавить циферок
                        n = m_Random.Next(1, 4);
                        for (int i = 0; i < n; i++)
                        {
                            s += m_Random.Next(10);
                        }
                        break;
                    case 1: //тип: имя+фамилия
                    case 2:
                        s = m_names[m_Random.Next(m_names.Length)].Trim() + " " +
                            m_surnames[m_Random.Next(m_surnames.Length)].Trim();
                        break;
                    case 3: //тип ник+фамилия
                        s = m_Dic[m_Random.Next(0, m_Dic.Length - 1)] + " " +
                            m_surnames[m_Random.Next(m_surnames.Length)].Trim();
                        break;
                    case 4: //старый метод ник+ник (не вызывается)
                        for (int c = 0; c < 2; c++)
                        {
                            s = m_Dic[m_Random.Next(0, m_Dic.Length - 1)] + " " + s;
                            s = Char.ToUpper(s[0]) + s.Substring(1);
                        }
                        s = s.Trim();
                        break;
                }
                return s;
            }

            if (generatorType == GeneratorTypes.webLoader)
            {
                if (httpClient == null)
                    httpClient = new HttpClient();

                if (webNames.Count == 0)
                {
                    string response = httpClient.DownloadString(webSource);
                    webNames.AddRange(response.Split(new char[] { '\x000D', '\x000A' }));
                }

                if (webNames.Count == 0)
                {
                    throw new Exception("Unable to load webNames");
                }

                string name = webNames[0];
                webNames.RemoveAt(0);

                return name;
            }

            return null;
		}
		public string GeneratePassword()
		{
			//string rv=m_Dic[m_Random.Next(0,m_Dic.Length)];
			//for (int c=0;c<5;c++) rv+=m_Random.Next(0,9).ToString();

            string rv = "";
            int len = m_Random.Next(8, 12);

            for (int i = 1; i <= len; i++)
            {
                switch (m_Random.Next(0, 3))
                {
                    case 0:
                        rv += (char)(m_Random.Next(48, 58)); // цифра
                        break;
                    case 1:
                        rv += (char)(m_Random.Next(65, 91)); // большая буква
                        break;
                    case 2:
                        rv += (char)(m_Random.Next(97,123)); // маленькая буква
                        break;
                }
            }
			return rv;
		}
        /*
		public string GenerateEmail()
		{
			return m_Dic[m_Random.Next(0,m_Dic.Length)]+"@"+m_Dic[m_Random.Next(0,m_Dic.Length)]+".com";
		}
        */
		public string GenerateRandomGMail() 
		{
			string s = GenerateNick();
			s = s.Replace(" ", ".");
			int n = m_Random.Next(1, 5);
			//добавить точек для уникальности
			for (int i = 0; i < n; i++) 
			{
				int k = m_Random.Next(1, s.Length - 1);
				s = s.Substring(0, k) + "." + s.Substring(k + 1);
				s = s.Replace("..", ".");
			}
			//добавить циферок
			n = m_Random.Next(3);
			for (int i = 0; i < n; i++) {
				s += m_Random.Next(10);
			}
			s += "@gmail.com";
			return s;
		}

		public string GenerateGMail(string base_login)
		{
			string rv="";
			for(int c=0; c<base_login.Length;c++)
			{
				rv+=base_login[c];
				if((c+1)!=base_login.Length)
				{
					int rnd=m_Random.Next(0,30000);
					if (rnd>15000)
						rv+=".";
				}
			}
			return rv+"@gmail.com";
		}

        public string GenerateDomain(string login, string mailSuffix)
        {
            string source = login.Replace(" ", "");
            /*    
                int count = login.Split(' ').Length;
                string mailbox = "";
                Random rnd = new Random();

                for (int i = 0; i < count; i++)
                {
                    int start = rnd.Next(0, source.Length / count);
                    int length = rnd.Next(source.Length/2, source.Length - start + 1);
                    mailbox += source.Substring(start, length);
                }

                return mailbox + mailSuffix;
             */
            return (source + mailSuffix).ToLower();
        }

        public string GenerateMailList(string fileName)
        {
            if (!regMails.inited)
                regMails.LoadFromFile(fileName);

            RegMail mailBox = regMails.boxes.Find(m => m.processed == false);

            if (mailBox == null)
            {
                throw new Exception("Mailboxes list finished");
            }
            return mailBox.mailBox;
        }

        public List<string> GetMailListBoxs(string fileName)
        {
            if (!regMails.inited)
                regMails.LoadFromFile(fileName);

            List<string> result = new List<string>();

            foreach (var box in regMails.boxes.FindAll(m => (m.processed == true && m.activated == false)))
            {
                result.Add(box.popServer + ';' + box.login + ';' + box.password + ';' + box.mailBox);
            }

            return result;
        }

        public void FixBoxReg(string mailBoxName)
        {
            RegMail mailBox = regMails.boxes.Find(m => m.mailBox == mailBoxName);

            if (mailBox != null)
            {
                mailBox.processed = true;
                regMails.SaveToFile();
            }
        }

        public void FixBoxAct(string mailBoxName)
        {
            RegMail mailBox = regMails.boxes.Find(m => m.mailBox == mailBoxName);

            if (mailBox != null)
            {
                mailBox.activated = true;
                regMails.SaveToFile();
            }
        }
    }
}
