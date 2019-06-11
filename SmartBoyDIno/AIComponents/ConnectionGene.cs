using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBoyDIno.Helpers;

namespace SmartBoyDIno.AIComponents
{
    /// <summary>
    /// Represents connection between 2 NN nodes
    /// </summary>
    public class ConnectionGene
    {
        public Node fromNode;
        public Node toNode;
        public double weight;
        public bool isEnabled;
        public int innovationNo;

        public ConnectionGene(Node from, Node to, double w, int innovation)
        {
            fromNode = from;
            toNode = to;
            weight = w;
            innovationNo = innovation;
        }

        public void mutateWeight()
        {
            Random random = new Random();
            double rand = random.NextDouble();
            if (rand < 0.1)
            {
                weight = random.NextDouble(-1, 1); // extended
            }
            else
            {
                weight += random.GaussianRandom() / 50; //extended

                if (weight > 1) // range must be between -1 & 1
                {
                    weight = 1;
                }
                if (weight < -1)
                {
                    weight = 1;
                }
            }
        }

        public ConnectionGene clone(Node from, Node to)
        {
            ConnectionGene clone = new ConnectionGene(from, to, weight, innovationNo);
            clone.isEnabled = this.isEnabled;

            return clone;
        }
    }
}
