using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TizTaboo
{
    enum faType
    {
        None = 0,
        URL = 1,
        Windows = 2,
        MultiAlias = 3,
        Batch = 4
    }

    [Serializable]
    class faNote
    {

        #region Public Fields

        public string Alias;
        public string Command;
        public DateTime LastExec;
        public string Name;
        public string Param;
        public int RunCount = 0;
        public faType Type;

        #endregion Public Fields

        #region Public Constructors

        public faNote(string Name, string Alias, string Command, string Param, faType Type)
        {
            this.Name = Name;
            this.Alias = Alias;
            this.Command = Command;
            this.Param = Param;
            this.Type = Type;
            this.LastExec = DateTime.Now;
        }

        #endregion Public Constructors

    }

    class faNotes
    {

        #region Public Fields

        public List<faNote> Items;

        #endregion Public Fields

        #region Private Fields

        const string English = "qwertyuiop[]asdfghjkl;'zxcvbnm,.";
        const string Russian = "йцукенгшщзхъфывапролджэячсмитьбю";
        private string _filepath;

        #endregion Private Fields

        #region Public Constructors

        public faNotes(string FilePath)
        {
            Items = new List<faNote>();
            _filepath = FilePath;
        }

        #endregion Public Constructors

        #region Public Properties

        public int Count
        {
            get { return this.Items.Count; }
        }

        #endregion Public Properties

        #region Public Methods

        public void Add(faNote Note)
        {
            Items.Add(Note);
        }

        public bool DeleteNodeByAlias(string Alias)
        {
            faNote note = null;
            note = Items.Find(item => item.Alias == Alias);
            if (note != null)
            {
                Items.Remove(note);
                return true;
            }
            else
                return false;
        }

        public faNote GetNodeByAlias(string Alias)
        {
            return Items.Find(item => item.Alias == Alias);
        }

        public bool Load()
        {
            try
            {
                using (Stream stream = File.Open(_filepath, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    Items = (List<faNote>)bformatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool Save()
        {
            try
            {
                using (Stream stream = File.Open(_filepath, FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bformatter.Serialize(stream, Items);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public List<faNote> Seek(string query)
        {
            List<faNote> found = new List<faNote>();
            string query_rus = ConvertEngToRus(query).ToLower();
            string query_eng = ConvertRusToEng(query).ToLower();
            found = Items.FindAll(
                delegate(faNote n)
                {
                    return n.Name.ToLower().Contains(query_rus) || n.Name.ToLower().Contains(query_eng) || n.Alias.ToLower().Contains(query_rus) || n.Alias.ToLower().Contains(query_eng);
                });
            found.Sort((a, b) => b.RunCount.CompareTo(a.RunCount));
            return found;
        }

        #endregion Public Methods

        #region Private Methods

        private string ConvertEngToRus(string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input)
                result.Append((index = English.IndexOf(symbol)) != -1 ? Russian[index] : symbol);
            return result.ToString();
        }

        private string ConvertRusToEng(string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input)
                result.Append((index = Russian.IndexOf(symbol)) != -1 ? English[index] : symbol);
            return result.ToString();
        }

        #endregion Private Methods

    }
}
