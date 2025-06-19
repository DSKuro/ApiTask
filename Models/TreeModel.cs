using System.Collections.ObjectModel;

namespace ApiTask.Models
{
    public class CodeCategory : Codes
    {
        public CodeCategory(string name) : base(name) { }

        public ObservableCollection<Codes> Codes = new ObservableCollection<Codes>();
    }

    public class Codes
    {
        public Codes(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
