using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.AIComponents
{
    public class ConnectionHistory
    {
        public int fromNode;
        public int toNode;
        public int innovationNumber;

        public List<int> innovationNumbers = new List<int>();

        public ConnectionHistory(int from, int to, int innovation, List<int> innovationNumbers)
        {
            fromNode = from;
            toNode = to;
            innovationNumber = innovation;
            this.innovationNumbers = new List<int>(innovationNumbers);
        }

        public bool matches(Genome genome, Node from, Node to)
        {
            //TODO: finish this
            return false;
        }
    }
}
