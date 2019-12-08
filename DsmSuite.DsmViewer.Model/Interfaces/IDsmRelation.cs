﻿namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmRelation
    {
        /// <summary>
        /// Unique and non-modifiable Number identifying the relation.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The consumer element id.
        /// </summary>
        int ConsumerId { get; }

        /// <summary>
        /// The provider element id.
        /// </summary>
        int ProviderId { get; }

        /// <summary>
        /// Type of relation.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Strength or weight of the relation
        /// </summary>
        int Weight { get; }
    }
}
