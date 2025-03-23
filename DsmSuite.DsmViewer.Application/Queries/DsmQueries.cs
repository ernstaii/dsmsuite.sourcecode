using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Queries
{
    /// <summary>
    /// Contains some but not all queries the application executes on the model
    /// </summary>
    /// TODO Can this class be made more useful?
    /// DsmApplicaton does some queries as well and dispatches others here. Can we make this class more useful
    /// by forwarding once and dispatching here?
    public class DsmQueries
    {

        private readonly IDsmModel _model;
        public DsmQueries(IDsmModel model)
        {
            _model = model;
        }

        public IEnumerable<WeightedElement> GetElementProvidedElements(IDsmElement element)
        {
            var elements = _model.FindIngoingRelations(element)
                .OrderBy(x => x.Provider.Fullname)
                .GroupBy(x => x.Provider.Fullname)
                .Select( g => new WeightedElement(g.First().Provider, g.Sum(x => x.Weight)) )
                .ToList();

            return elements;
        }

        public IEnumerable<WeightedElement> GetElementProviders(IDsmElement element)
        {
            var elements = _model.FindOutgoingRelations(element)
                .OrderBy(x => x.Provider.Fullname)
                .GroupBy(x => x.Provider.Fullname)
                .Select( g => new WeightedElement(g.First().Provider, g.Sum(x => x.Weight)) )
                .ToList();

            return elements;
        }

        public IEnumerable<WeightedElement> GetElementConsumers(IDsmElement element)
        {
            var elements = _model.FindIngoingRelations(element)
                .OrderBy(x => x.Consumer.Fullname)
                .GroupBy(x => x.Consumer.Fullname)
                .Select( g => new WeightedElement(g.First().Consumer, g.Sum(x => x.Weight)) )
                .ToList();

            return elements;
        }

        public IEnumerable<WeightedElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider)
        {
            var elements = _model.FindRelations(consumer, provider)
                .OrderBy(x => x.Consumer.Fullname)
                .GroupBy(x => x.Consumer.Fullname)
                .Select(g => new WeightedElement(g.First().Consumer, g.Sum(x => x.Weight)))
                .ToList();

            return elements;
        }

        public IEnumerable<WeightedElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            var elements = _model.FindRelations(consumer, provider)
                .OrderBy(x => x.Provider.Fullname)
                .GroupBy(x => x.Provider.Fullname)
                .Select(g => new WeightedElement(g.First().Provider, g.Sum(x => x.Weight)))
                .ToList();

            return elements;
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            var relations = _model.FindRelations(consumer, provider)
                .OrderBy(x => x.Provider.Fullname)
                .ThenBy(x => x.Consumer.Fullname)
                .ToList();
            return relations;
        }
        public IEnumerable<IDsmRelation> FindIngoingRelations(IDsmElement element)
        {
            return _model.FindIngoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindOutgoingRelations(IDsmElement element)
        {
            return _model.FindOutgoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindInternalRelations(IDsmElement element)
        {
            return _model.FindInternalRelations(element);
        }

        public IEnumerable<IDsmRelation> FindExternalRelations(IDsmElement element)
        {
            return _model.FindExternalRelations(element);
        }
    }
}
