using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Management
{
    public class ActionData : IDsmAction
    {
        public int Id { get; }

        public string Type { get; }

        public IReadOnlyDictionary<string, string> Data { get; }

        public IEnumerable<IDsmAction> Actions { get; }

        public ActionData(IAction action)
        {
            Id = 0;
            Type = action.Type.ToString();
            Data = action.Data;
            Actions = (action.GetType().GetProperty("Actions")?.
                    GetValue(action) as IEnumerable<IAction>)?.Select(a => new ActionData(a));
        }
    }
}
