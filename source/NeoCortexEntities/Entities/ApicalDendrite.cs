﻿using NeoCortexApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoCortexApi.Entities
{
    /// <summary>
    /// Apical Segment used to interconnect cells across areas.
    /// </summary>
    public class ApicalDendrite : Segment, IComparable<ApicalDendrite>, IEquatable<ApicalDendrite>
    {
        private int m_Ordinal = -1;

        /// <summary>
        /// The seqence number of the segment. Specifies the order of the segment of the <see cref="Connections"/> instance.
        /// </summary>
        public int Ordinal { get => m_Ordinal; set => m_Ordinal = value; }

#pragma warning disable CS1729 // 'Segment' does not contain a constructor that takes 4 arguments
        public ApicalDendrite(Cell parentCell, int flatIdx, long lastUsedIteration, int ordinal, double synapsePermConnected, int numInputs) : base(flatIdx, lastUsedIteration, synapsePermConnected, numInputs)
#pragma warning restore CS1729 // 'Segment' does not contain a constructor that takes 4 arguments
        {
#pragma warning disable CS1061 // 'ApicalDendrite' does not contain a definition for 'ParentCell' and no accessible extension method 'ParentCell' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
            this.ParentCell = parentCell;
#pragma warning restore CS1061 // 'ApicalDendrite' does not contain a definition for 'ParentCell' and no accessible extension method 'ParentCell' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
            this.m_Ordinal = ordinal;
        }

#pragma warning disable CS1729 // 'Segment' does not contain a constructor that takes 4 arguments
        public ApicalDendrite(Cell parentCell, int index, double synapsePermConnected) : base(index, 0, synapsePermConnected, -1)
#pragma warning restore CS1729 // 'Segment' does not contain a constructor that takes 4 arguments
        {
#pragma warning disable CS1061 // 'ApicalDendrite' does not contain a definition for 'ParentCell' and no accessible extension method 'ParentCell' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
            this.ParentCell = parentCell;
#pragma warning restore CS1061 // 'ApicalDendrite' does not contain a definition for 'ParentCell' and no accessible extension method 'ParentCell' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
            this.m_Ordinal = index;
        }


        /// <summary>
        /// Compares by index.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ApicalDendrite other)
        {
            if (this.SegmentIndex > other.SegmentIndex)
                return 1;
            else if (this.SegmentIndex < other.SegmentIndex)
                return -1;
            else
                return 0;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(ApicalDendrite obj)
        {
            if (this == obj)
                return true;

            if (obj == null)
                return false;

            ApicalDendrite other = (ApicalDendrite)obj;
#pragma warning disable CS0103 // The name 'ParentCell' does not exist in the current context
            if (ParentCell == null)
            {
#pragma warning disable CS1061 // 'ApicalDendrite' does not contain a definition for 'ParentCell' and no accessible extension method 'ParentCell' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
                if (other.ParentCell != null)
                    return false;
#pragma warning restore CS1061 // 'ApicalDendrite' does not contain a definition for 'ParentCell' and no accessible extension method 'ParentCell' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
            }
#pragma warning restore CS0103 // The name 'ParentCell' does not exist in the current context
            // We check here the cell id only! The cell as parent must be correctlly created to avoid having different cells with the same id.
            // If we would use here ParenCell.Equals method, that method would cause a cicular invoke of this.Equals etc.
            //else if (ParentCell.CellId != other.ParentCell.CellId)
            //    return false;
#pragma warning disable CS1061 // 'ApicalDendrite' does not contain a definition for 'LastUsedIteration' and no accessible extension method 'LastUsedIteration' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0103 // The name 'LastUsedIteration' does not exist in the current context
            if (LastUsedIteration != other.LastUsedIteration)
                return false;
#pragma warning restore CS0103 // The name 'LastUsedIteration' does not exist in the current context
#pragma warning restore CS1061 // 'ApicalDendrite' does not contain a definition for 'LastUsedIteration' and no accessible extension method 'LastUsedIteration' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
            if (m_Ordinal != other.m_Ordinal)
                return false;
#pragma warning disable CS1061 // 'ApicalDendrite' does not contain a definition for 'LastUsedIteration' and no accessible extension method 'LastUsedIteration' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
#pragma warning disable CS0103 // The name 'LastUsedIteration' does not exist in the current context
            if (LastUsedIteration != other.LastUsedIteration)
                return false;
#pragma warning restore CS0103 // The name 'LastUsedIteration' does not exist in the current context
#pragma warning restore CS1061 // 'ApicalDendrite' does not contain a definition for 'LastUsedIteration' and no accessible extension method 'LastUsedIteration' accepting a first argument of type 'ApicalDendrite' could be found (are you missing a using directive or an assembly reference?)
            if (Ordinal != other.Ordinal)
                return false;
            if (SegmentIndex != obj.SegmentIndex)
                return false;
            if (Synapses == null)
            {
                if (obj.Synapses != null)
                    return false;
            }
            else if (!Synapses.ElementsEqual(obj.Synapses))
                return false;

            if (SynapsePermConnected != obj.SynapsePermConnected)
                return false;
            if (NumInputs != obj.NumInputs)
                return false;

            return true;
        }
    }
}
