using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmAction : IDsmAction
    {
        private List<IDsmAction> _actions;

        public DsmAction(int id, string type, IReadOnlyDictionary<string, string> data,
                IEnumerable<IDsmAction> actions = null)
        {
            Id = id;
            Type = type;
            Data = data;
            _actions = actions != null ? new List<IDsmAction>(actions) : null;
        }

        public int Id { get; }

        public string Type { get; }

        public IReadOnlyDictionary<string, string> Data { get; }

        public IEnumerable<IDsmAction> Actions { get { return _actions; } }
    }
}
