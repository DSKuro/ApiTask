using System.Collections.Generic;

namespace ApiTask.Services
{
    public class SortingTreeMemento
    {
        public List<string> Content { get; set; }
        public List<bool?> States { get; set; } = new List<bool?>();

        public SortingTreeMemento(List<string> parameters) 
        {
            Content = parameters;
            InitializeStates(parameters.Count);
        }

        private void InitializeStates(int count)
        {
            for (int i = 0; i < count; i++)
            {
                States.Add(false);
            }
        }
    }
}
