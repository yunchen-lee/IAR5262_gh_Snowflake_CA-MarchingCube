using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;

namespace IAR5262_MarchingCube
{
    public class HexGrid
    {
        // property
        // -----------------------------------------------
        public List<List<List<HexNode>>> nodes = new List<List<List<HexNode>>>();
        private int size;
        private int zHeight;
        private double alpha;
        private double beta;
        private double gamma;
        private double iso;

        // constructor
        // -----------------------------------------------
        public HexGrid(int _size, int _zh, double _alpha, double _beta, double _gamma, double _iso)
        {
            this.size = _size;
            this.zHeight = _zh;
            this.alpha = _alpha;
            this.beta = _beta;
            this.gamma = _gamma;
            this.iso = _iso;

            for (int k = 0; k < this.zHeight; k++)
            {
                nodes.Add(new List<List<HexNode>>());
                for (int i = 0; i < 2 * this.size + 1; i++)
                {
                    nodes[k].Add(new List<HexNode>());
                    for (int j = 0; j < 2 * this.size + 1; j++)
                    {
                        HexNode node = new HexNode(i, j, k, this.size, this.beta);
                        if (i == this.size && j == this.size && k == this.zHeight / 2) node.U = 1;     // center

                        if (i < this.size) { if (j > this.size + i) node = null; }
                        else if (i > this.size) { if (j < i - this.size) node = null; }

                        nodes[k][i].Add(node);

                    }
                }
            }

        }


        // method
        // -----------------------------------------------
        // check receptive
        public void checkReceptive()
        {
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    if (n != null)
                    {
                        if (n.Water >= 1) n.Receptive = true;
                        if (this.isReceptive(n)) n.Receptive = true;
                    }
                }
            }


        }

        public void run()
        {
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    {
                        if (n != null) n.runPhase1(this.gamma);
                    }
                }
            }
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    if (n != null)
                    {
                        double avg = this.getBoundatyAvg(n);
                        n.runPhase2(this.alpha, avg);
                        n.runPhase3(this.beta);
                    }
                }

            }
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    if (n != null)
                    {
                        n.update();
                    }
                }

            }
        }

        // get avg
        public double getBoundatyAvg(HexNode center)
        {
            double sum = 0;
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    if (n != null)
                    {
                        if (this.isNeighbor(center, n)) sum += n.U;
                    }
                }

            }

            return sum / 6;
        }

        // is neighbor
        public bool isNeighbor(HexNode center, HexNode other)
        {
            //if (other.Pidz == center.Pidz)
            //{
            if (other.Pidx == center.Pidx - 1 && other.Pidy == center.Pidy - 1) return true;
            else if (other.Pidx == center.Pidx - 1 && other.Pidy == center.Pidy) return true;
            else if (other.Pidx == center.Pidx && other.Pidy == center.Pidy - 1) return true;
            else if (other.Pidx == center.Pidx && other.Pidy == center.Pidy + 1) return true;
            else if (other.Pidx == center.Pidx + 1 && other.Pidy == center.Pidy) return true;
            else if (other.Pidx == center.Pidx + 1 && other.Pidy == center.Pidy + 1) return true;
            //}
            //else if ((other.Pidz == center.Pidz + 1||other.Pidz==center.Pidz-1) && other.Pidx == center.Pidx && other.Pidy == center.Pidy) return true;
            return false;
        }

        // is receptive
        public bool isReceptive(HexNode center)
        {
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    if (n != null)
                    {
                        if (this.isNeighbor(center, n))
                        {
                            if (n.Water >= 1) return true;
                        }
                    }
                }

            }
            return false;
        }

        // set constant
        public void setConstant(double a, double b, double g)
        {
            this.alpha = a;
            this.beta = b;
            this.gamma = g;
        }

        // display grid
        public List<Point3d> displayGrid()
        {
            List<Point3d> listPts = new List<Point3d>();
            foreach (List<List<HexNode>> zlayer in this.nodes)
            {
                foreach (List<HexNode> row in zlayer)
                {
                    foreach (HexNode n in row)
                    {
                        if (n != null) listPts.Add(n.Pos);
                    }
                }
            }
            return listPts;
        }

        // display frozen
        public List<Point3d> displayFrozen()
        {
            List<Point3d> listPts = new List<Point3d>();
            foreach (List<List<HexNode>> zlayer in this.nodes)
            {
                foreach (List<HexNode> row in zlayer)
                {
                    foreach (HexNode n in row)
                    {
                        if (n != null) if (n.Water > this.iso && n.IsBoundary == false) listPts.Add(n.Pos);
                    }
                }
            }
            return listPts;
        }

        // display zh/2 layer
        public List<Point3d> displayCenterLayer()
        {
            List<Point3d> listPts = new List<Point3d>();
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    if (n != null)
                    {
                        listPts.Add(n.Pos);

                    }
                }
            }
            return listPts;
        }

        // display receptive
        public List<Point3d> displayRece()
        {
            List<Point3d> listPts = new List<Point3d>();
            foreach (List<List<HexNode>> zlayer in this.nodes)
            {
                foreach (List<HexNode> row in zlayer)
                {
                    foreach (HexNode n in row)
                    {
                        if (n != null && n.Receptive) listPts.Add(n.Pos);
                    }
                }
            }
            return listPts;
        }

    }
    /*
    public class HexGrid
    {
        // property
        // -----------------------------------------------
        private List<List<List<HexNode>>> nodes = new List<List<List<HexNode>>>();
        private int size;
        private int zHeight;
        private double alpha;
        private double beta;
        private double gamma;
        private double iso;

        // constructor
        // -----------------------------------------------
        public HexGrid(int _size, int _zh,double _alpha, double _beta,double _gamma,double _iso) { 
            this.size = _size;
            this.zHeight = _zh;
            this.alpha = _alpha;
            this.beta = _beta;
            this.gamma = _gamma;
            this.iso = _iso;

            for (int k = 0; k < this.zHeight; k++)
            {
                nodes.Add(new List<List<HexNode>>());
                for (int i = 0; i < 2 * this.size+1; i++) { 
                    nodes[k].Add(new List<HexNode>());
                    for(int j=0;j<2* this.size+1; j++)
                    {
                            HexNode node = new HexNode(i,j,k,this.size,this.beta);
                            if (i == this.size && j == this.size && k == this.zHeight/2) node.U = 1;     // center

                            if (i < this.size) { if (j > this.size + i) node = null; }
                            else if(i>this.size) { if (j < i - this.size) node = null; }

                            nodes[k][i].Add(node);

                        }
                    }
            }
        
        }


        // method
        // -----------------------------------------------
        // check receptive
        public void checkReceptive() {
                foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
                {
                    foreach (HexNode n in row){
                        if (n != null){
                            if (n.Water >= 1) n.Receptive = true;
                            if (this.isReceptive(n)) n.Receptive = true;
                        }
                    }
                }
            
        
        }

        public void run()
        {
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row) { 
                    {
                        if (n != null) n.runPhase1(this.gamma);
                    }
                }
            }
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row){
                    if (n != null){
                        double avg = this.getBoundatyAvg(n);
                        n.runPhase2(this.alpha, avg);
                        n.runPhase3(this.beta);
                    }
                }
                
            }
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row){
                    if (n != null)
                    {
                        n.update();
                    }
                }
                
            }
        }

        // get avg
        public double getBoundatyAvg(HexNode center)
        {
            double sum = 0;
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row){
                    if (n != null){
                        if (this.isNeighbor(center, n)) sum += n.U;
                    }
                }
                
            }

            return sum / 6;
        }


        // is neighbor
        public bool isNeighbor(HexNode center,HexNode other)
        {
            //if (other.Pidz == center.Pidz)
            //{
                if (other.Pidx == center.Pidx - 1 && other.Pidy == center.Pidy - 1) return true;
                else if (other.Pidx == center.Pidx - 1 && other.Pidy == center.Pidy) return true;
                else if (other.Pidx == center.Pidx && other.Pidy == center.Pidy - 1) return true;
                else if (other.Pidx == center.Pidx && other.Pidy == center.Pidy + 1) return true;
                else if (other.Pidx == center.Pidx + 1 && other.Pidy == center.Pidy) return true;
                else if (other.Pidx == center.Pidx + 1 && other.Pidy == center.Pidy + 1) return true;
            //}
            //else if ((other.Pidz == center.Pidz + 1||other.Pidz==center.Pidz-1) && other.Pidx == center.Pidx && other.Pidy == center.Pidy) return true;
            return false;
        }

        // is receptive
        public bool isReceptive(HexNode center)
        {
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row) {
                    if (n != null) {
                        if (this.isNeighbor(center, n)) {
                            if (n.Water >= 1) return true;
                        }
                    }
                }
                
            }
            return false;
        }

        // set constant
        public void setConstant(double a,double b,double g)
        {
            this.alpha = a;
            this.beta = b;
            this.gamma = g;
        }

        // display grid
        public List<Point3d> displayGrid()
        {
            List<Point3d> listPts = new List<Point3d>();
            foreach (List<List<HexNode>> zlayer in this.nodes){
                foreach (List<HexNode> row in zlayer)
                {
                    foreach (HexNode n in row)
                    {
                        if (n != null) listPts.Add(n.Pos);
                    }
                }
            }
            return listPts;
        }

        // display frozen
        public List<Point3d> displayFrozen()
        {
            List<Point3d> listPts = new List<Point3d>();
            foreach (List<List<HexNode>> zlayer in this.nodes){
                foreach (List<HexNode> row in zlayer){
                    foreach (HexNode n in row){
                        if (n != null) if(n.Water>this.iso&&n.IsBoundary==false) listPts.Add(n.Pos);
                    }      
                }
            }
            return listPts;
        }

        // display zh/2 layer
        public List<Point3d> displayCenterLayer()
        {
            List<Point3d> listPts = new List<Point3d>();
            foreach (List<HexNode> row in this.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    if (n != null)
                    {
                        listPts.Add(n.Pos);

                    }
                }
            }
            return listPts;
        }
        
    }
    */
}
