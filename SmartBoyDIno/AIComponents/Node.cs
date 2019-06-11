using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.AIComponents
{
    public class Node
    {
        public int number;
        public double inputSum = 0;
        public double outputValue = 0;
        public List<ConnectionGene> outputConnections = new List<ConnectionGene>();
        public int layer = 0;

        public Node(int number)
        {
            this.number = number;
        }

        public void engage()
        {
            if (layer != 0)
            {
                outputValue = sigmoid(inputSum);
            }
            foreach (var connection in outputConnections)
            {
                if (connection.isEnabled)
                {
                    connection.toNode.inputSum += connection.weight * outputValue;
                }
            }

        }

        public bool isConnectedTo(Node node)
        {
            if (node.layer == this.layer) //you cant have connection between node in the same layer
                return false;

            if (node.layer < this.layer) // checking left side
            {
                foreach (var el in node.outputConnections)
                {
                    if (el.toNode == node)
                    {
                        return true;
                    }
                }
            }
            else // checking right side
            {
                foreach (var el in outputConnections)
                {
                    if (el.toNode == node)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private double sigmoid(double inputSum)
        {
            double res = 1 / (1 + Math.Pow(Math.E, -4.9 * inputSum));
            return res;
        }

        public Node clone()
        {
            var clone = new Node(number);
            clone.layer = this.layer;
            return clone;
        }
    }
}
