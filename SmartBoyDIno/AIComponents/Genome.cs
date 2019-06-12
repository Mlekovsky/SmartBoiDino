using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.AIComponents
{
    public class Genome
    {
        public List<ConnectionGene> genes = new List<ConnectionGene>();
        public List<Node> nodes = new List<Node>();
        public int inputs;
        public int outputs;
        public int layers = 2;
        public int nextNode = 0;
        public int biasNode;

        public List<Node> network = new List<Node>();

        public Genome(int input, int output)
        {
            inputs = input;
            outputs = output;

            //creating input nodes
            for (int i = 0; i < inputs; i++)
            {
                nodes.Add(new Node(i));
                nextNode++;
                nodes[i].layer = 0;
            }
            //creating output nodes
            for (int i = 0; i < outputs; i++)
            {
                nodes.Add(new Node(i + inputs));
                nodes[i + inputs].layer = 1;
                nextNode++;
            }
            //creating bias node
            nodes.Add(new Node(nextNode));
            biasNode = nextNode;
            nextNode++;
            nodes[biasNode].layer = 0;
        }

        /// <summary>
        /// returns node with matching number
        /// </summary>
        /// <param name="nodeNumber">number to match</param>
        /// <returns>Node</returns>
        public Node GetNode(int nodeNumber)
        {
            foreach (var node in nodes)
            {
                if (node.number == nodeNumber)
                {
                    return node;
                }

            }
            return null;
        }

        public void connectNodes()
        {
            foreach (var node in nodes)
            {
                node.outputConnections.Clear();
            }

            foreach (var gene in genes)
            {
                gene.fromNode.outputConnections.Add(gene);
            }
        }

        public double[] feedForward(double[] inputValues)
        {
            //setting outputs of the inputnodes
            for (int i = 0; i < inputs; i++)
            {
                nodes[i].outputValue = inputValues[i];
            }

            nodes[biasNode].outputValue = 1;

            foreach (var net in network)
            {
                net.engage();
            }

            double[] outs = new double[outputs];
            for (int i = 0; i < outputs; i++)
            {
                outs[i] = nodes[inputs + i].outputValue;
            }

            foreach (var node in nodes)
            {
                node.inputSum = 0;
            }

            return outs;
        }

        public void GenerateNetwork()
        {
            connectNodes();
            network = new List<Node>();

            for (int i = 0; i < layers; i++) //each layer
            {
                for (int j = 0; j < nodes.Count; j++) //each node
                {
                    if(nodes[j].layer == i) //if node is in correct layer then add it
                    {
                        network.Add(nodes[j]);
                    }
                }
            }
        }

    }
}
