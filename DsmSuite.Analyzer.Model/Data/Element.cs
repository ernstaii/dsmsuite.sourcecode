using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Data
{
    /// <summary>
    /// Represents element of a component. Both the ElementId and Name uniquely identify an element.
    /// </summary>
    public class Element : IElement
    {
        public Element(int id, string name, string type, string source){
            Id = id;
            Name = name;
            Type = type;
            Source = source;
        }

        public int Id { get; }
        public string Name { get; set;}
        public string Type { get; }
        public string Source { get; }
    }
}