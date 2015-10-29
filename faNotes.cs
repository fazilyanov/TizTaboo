using System;

namespace TizTaboo
{
    enum faType
    {
        None,
        WebLink,
        Note,
        WinLink,
        Batch
    }
    class faAlias
    {
        byte Count { get; set; }
        string[] Name { get; set; }
        public faAlias()
        {
            Count = 0;
        }
        public void Add(string _n)
        {
            Count++;
            Name[Count - 1] = _n;
        }

    }
    class faLink
    {
        byte Count { get; set; }
        string[] Link { get; set; }
        public faLink()
        {
            Count = 0;
        }
        public void Add(string _n)
        {
            Count++;
            Link[Count - 1] = _n;
        }

    }
    class faNote
    {
        public faAlias Alias { get; set; }
        public faLink Link { get; set; }

        public faType Type { get; set; }
        
        public faNote()
        {
            Type = faType.None;        
        }
    }



    class faNotes
    {
        public int Count { get; set; }
        private faNote[] _notes;
        public faNotes()
        {
            Count = 0;
        }

        public void Add(faNote Note)
        {
            Count++;
            Array.Resize(ref _notes, Count);
            _notes[Count - 1] = new faNote();
            _notes[Count - 1] = Note;
        }
    }
}
