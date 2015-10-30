﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TizTaboo
{
    enum faType
    {
        None=0,
        URL=1,
        FileName=2,
        MultiAlias=3,
        Batch=4
    }

    [Serializable]
    class faNote
    {
        public string Name;
        public string Alias;
        public string Command;
        public faType Type;
        public DateTime LastExec;

        public faNote(string Name, string Alias, string Command, faType Type)
        {
            this.Name = Name;
            this.Alias = Alias;
            this.Command = Command;
            this.Type = Type;
            this.LastExec = DateTime.Now;
        }
    }

    class faNotes
    {
        const string English = "qwertyuiop[]asdfghjkl;'zxcvbnm,.";
        public string Russian = "йцукенгшщзхъфывапролджэячсмитьбю";
        public int Count
        {
            get { return this.Items.Count; }
        }
        public List<faNote> Items;
        private string _filepath;

        public faNotes(string FilePath)
        {
            Items = new List<faNote>();
            _filepath = FilePath;
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

        public void Add(faNote Note)
        {
            Items.Add(Note);
        }

        public faNote GetNodeByAlias(string Alias)
        {
            return Items.Find(item => item.Alias == Alias);
        }

        public bool DeleteNodeByAlias(string Alias)
        {
            faNote note = null;
            note=Items.Find(item => item.Alias == Alias);
            if (note != null)
            {
                Items.Remove(note);
                return true;
            }
            else
                return false;
        }

        public List<faNote> Seek(string query)
        {
            List<faNote> found = new List<faNote>();
            string query_rus = ConvertEngToRus(query);
            string query_eng = ConvertRusToEng(query);
            found = Items.FindAll(
                delegate(faNote n)
                {
                    return n.Name.Contains(query_rus) || n.Name.Contains(query_eng) || n.Alias.Contains(query_rus) || n.Alias.Contains(query_eng);
                });
            return found;
        }

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

    }
}
