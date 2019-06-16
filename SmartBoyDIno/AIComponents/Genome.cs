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
       // public int nextConnectionNo = 1000;

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

        //empty genome constructor
        public Genome(int input, int output, bool cross)
        {
            inputs = input;
            outputs = output;
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

        public void ConnectNodes()
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
            ConnectNodes();
            network = new List<Node>();

            for (int i = 0; i < layers; i++) //each layer
            {
                for (int j = 0; j < nodes.Count; j++) //each node
                {
                    if (nodes[j].layer == i) //if node is in correct layer then add it
                    {
                        network.Add(nodes[j]);
                    }
                }
            }
        }

        public void AddConnection(List<ConnectionHistory> innovationHistory)
        {
            if (FullyConnected())
            {
                Console.WriteLine("Cannot add connection to fully connected network");
                return;
            }

            Random random = new Random();

            //getting randoms
            int randomNode1 = random.Next(0, nodes.Count); //this might be buggy
            int randomNode2 = random.Next(0, nodes.Count);
            while (BadRandomConnectionNodes(randomNode1, randomNode2))
            {
                randomNode1 = random.Next(0, nodes.Count);
                randomNode2 = random.Next(0, nodes.Count);
            }

            int switchTmp;
            if (nodes[randomNode1].layer > nodes[randomNode2].layer) // we want to keep direction  of nodes right, so 1 -> 2
            {
                switchTmp = randomNode2;
                randomNode2 = randomNode1;
                randomNode1 = switchTmp;
            }

            int connectionInnovationNumber = GetInnovationNumber(innovationHistory, nodes[randomNode1], nodes[randomNode2]);

            genes.Add(new ConnectionGene(nodes[randomNode1], nodes[randomNode2], random.NextDouble(-1, 1), connectionInnovationNumber)); //this also might be buggy
            ConnectNodes();
        }

        /// <summary>
        /// Checks if there were this mutations in history, and if true, it will give this unique innovation numbers, otherwise, gets the old one
        /// </summary>
        /// <param name="innovationHistory"></param>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns>Innovation number for the new mutation</returns>
        private int GetInnovationNumber(List<ConnectionHistory> innovationHistory, Node from, Node to)
        {
            bool isNew = true;
            int connectionInnovationNumber = Game1.nextConnectionNo; // come back to this, make it global

            foreach (var innovation in innovationHistory)
            {
                if (innovation.Matches(this, from, to))
                {
                    isNew = false;
                    connectionInnovationNumber = innovation.innovationNumber;
                    break;
                }
            }

            if (isNew)
            {
                List<int> innovNumbers = new List<int>();
                foreach (var gene in genes)
                {
                    innovNumbers.Add(gene.innovationNo); //set the number, then add it to history
                }

                innovationHistory.Add(new ConnectionHistory(from.number, to.number, connectionInnovationNumber, innovNumbers));
                Game1.nextConnectionNo++;
            }

            return connectionInnovationNumber;
        }

        private bool BadRandomConnectionNodes(int randomNode1, int randomNode2)
        {
            if (nodes[randomNode1].layer == nodes[randomNode2].layer) //same layer
            {
                return true;
            }
            if (nodes[randomNode1].isConnectedTo(nodes[randomNode2])) //already connected, as method name says
            {
                return true;
            }
            return false;
        }

        public void AddNode(List<ConnectionHistory> innovationHistory)
        {
            if (genes.Count == 0)
            {
                AddConnection(innovationHistory);
                return;
            }

            Random random = new Random();

            int randomConnection = random.Next(0, genes.Count);

            while (genes[randomConnection].fromNode == nodes[biasNode] && genes.Count != 1)
            {
                randomConnection = random.Next(0, genes.Count);
            }

            genes[randomConnection].isEnabled = false;

            int newNodeNo = nextNode;
            nodes.Add(new Node(newNodeNo));
            nextNode++;

            //new connection with weight 1
            int connectionInnovationNumber = GetInnovationNumber(innovationHistory, genes[randomConnection].fromNode, GetNode(newNodeNo));
            genes.Add(new ConnectionGene(genes[randomConnection].fromNode, GetNode(newNodeNo), 1, connectionInnovationNumber));

            connectionInnovationNumber = GetInnovationNumber(innovationHistory, GetNode(newNodeNo), genes[randomConnection].toNode);
            // add new connection from new node with a weight same as the disabled connection
            genes.Add(new ConnectionGene(GetNode(newNodeNo), genes[randomConnection].toNode, genes[randomConnection].weight, connectionInnovationNumber));
            GetNode(newNodeNo).layer = genes[randomConnection].fromNode.layer + 1;

            connectionInnovationNumber = GetInnovationNumber(innovationHistory, nodes[biasNode], GetNode(newNodeNo));
            genes.Add(new ConnectionGene(nodes[biasNode], GetNode(newNodeNo), 0, connectionInnovationNumber));

            if (GetNode(newNodeNo).layer == genes[randomConnection].toNode.layer)
            {
                for (int i = 0; i < nodes.Count - 1; i++) //miss the new one
                {
                    if (nodes[i].layer >= GetNode(newNodeNo).layer)
                    {
                        nodes[i].layer++;
                    }
                }
                layers++;
            }

            ConnectNodes();
        }

        /// <summary>
        /// Ensures that network is fully connected
        /// </summary>
        public bool FullyConnected()
        {
            int maxConnections = 0;
            int[] nodesInLayers = new int[layers];

            foreach (var node in nodes)
            {
                nodesInLayers[node.layer] += 1;
            }

            for (int i = 0; i < layers - 1; i++)
            {
                int nodesInFront = 0;
                for (int j = i + 1; j < layers; j++)
                {
                    nodesInFront += nodesInLayers[j];
                }
                maxConnections += nodesInLayers[i] * nodesInFront; // each node in layer need to have connections to all next layers nodes
            }

            if (maxConnections == genes.Count) //checking if no of conns is equal to max number of possible connections
            {
                return true;
            }
            return false;
        }

        public void Mutate(List<ConnectionHistory> innovationHistory)
        {
            if (genes.Count == 0)
            {
                AddConnection(innovationHistory);
            }

            Random random = new Random();

            double random1 = random.NextDouble();
            if (random1 < 0.8)
            {
                foreach (var gene in genes)
                {
                    gene.MutateWeight();
                }
            }

            double random2 = random.NextDouble();
            if (random2 < 0.08)
            {
                AddConnection(innovationHistory);
            }

            double random3 = random.NextDouble();
            if (random3 < 0.02)
            {
                AddNode(innovationHistory);
            }
        }

        public Genome Crossover(Genome parent2)
        {
            Genome child = new Genome(inputs, outputs, true);
            child.genes.Clear();
            child.nodes.Clear();
            child.layers = layers;
            child.nextNode = nextNode;
            child.biasNode = biasNode;
            List<ConnectionGene> childGenes = new List<ConnectionGene>(); //inherited from partens
            List<bool> isEnabled = new List<bool>();

            Random random = new Random();

            for (int i = 0; i < genes.Count; i++)
            {
                bool setEnabled = true;

                int parent2Gene = matchingGene(parent2, genes[i].innovationNo);
                if (parent2Gene != -1)
                {
                    if (!genes[i].isEnabled || !parent2.genes[parent2Gene].isEnabled)
                    {
                        if (random.NextDouble() < 0.75)
                        {
                            setEnabled = false;
                        }
                    }

                    if (random.NextDouble() < 0.5)
                    {
                        childGenes.Add(genes[i]);
                    }
                    else
                    {
                        childGenes.Add(parent2.genes[parent2Gene]);
                    }
                }
                else
                {
                    childGenes.Add(genes[i]);
                    setEnabled = genes[i].isEnabled;
                }
                isEnabled.Add(setEnabled);
            }

            foreach (var node in nodes)
            {
                child.nodes.Add(node.clone());
            }

            for (int i = 0; i < childGenes.Count; i++)
            {
                child.genes.Add(childGenes[i].clone(child.GetNode(childGenes[i].fromNode.number), child.GetNode(childGenes[i].toNode.number)));
                child.genes[i].isEnabled = isEnabled[i];
            }

            child.ConnectNodes();
            return child;
        }

        private int matchingGene(Genome parent2, int innovationNo)
        {
            for (int i = 0; i < parent2.genes.Count; i++)
            {
                if (parent2.genes[i].innovationNo == innovationNo)
                {
                    return i;
                }
            }
            return -1;
        }

        public Genome Clone()
        {
            Genome clone = new Genome(inputs, outputs, true);

            foreach (var node in nodes)
            {
                clone.nodes.Add(node.clone()); //cloning nodes
            }

            foreach(var gene in genes)
            {
                clone.genes.Add(gene.clone(clone.GetNode(gene.fromNode.number), clone.GetNode(gene.toNode.number))); //cloning all connections
            }

            clone.layers = layers;
            clone.nextNode = nextNode;
            clone.biasNode = biasNode;
            clone.ConnectNodes();

            return clone;
        }
    }
}
