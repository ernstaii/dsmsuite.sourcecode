using System.Collections;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public class ActionAttributes
    {
        readonly Dictionary<string, string> _data;

        public ActionAttributes()
        {
            _data = new Dictionary<string, string>();
        }

        public void SetString(string memberName, string memberValue)
        {
            _data[RemoveUnderscore(memberName)] = memberValue;
        }

        public void SetInt(string memberName, int memberValue)
        {
            _data[RemoveUnderscore(memberName)] = memberValue.ToString();
        }

        public void SetNullableInt(string memberName, int? memberValue)
        {
            if (memberValue.HasValue)
            {
                _data[RemoveUnderscore(memberName)] = memberValue.Value.ToString();
            }
        }

        public void SetListInt(string memberName, List<int> list)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < list.Count; i++)
            {
                s.Append(list[i]);
                if (i < list.Count - 1)
                    s.Append(',');
            }

            _data[RemoveUnderscore(memberName)] = s.ToString();
        }


        public IReadOnlyDictionary<string, string> Data => _data;

        private static string RemoveUnderscore(string memberName)
        {
            return memberName.Substring(1); 
        }
    }
}
