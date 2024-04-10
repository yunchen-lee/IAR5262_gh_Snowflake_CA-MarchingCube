using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;

namespace IAR5262_MarchingCube
{
    public class PrismGrid
    {
        // property
        // -----------------------------------------------
        private List<Mesh> outMesh;
        private List<TriPrismUnit> TriUnits;
        private int size;
        private int zHeight;
        private double iso;
        public HexGrid grid;
        public bool end;



        // constructor
        // -----------------------------------------------

        public PrismGrid(int _size, int _zh, double _alpha, double _beta, double _gamma, double _iso)
        {
            grid = new HexGrid(_size, _zh, _alpha, _beta, _gamma, _iso);
            this.size = _size;
            this.zHeight = _zh;
            this.iso = _iso;
            this.outMesh = new List<Mesh>();
            this.TriUnits = new List<TriPrismUnit>();
            this.end = false;
        }




        // method
        // -----------------------------------------------
        // create Grid of Triangular prism unit
        public void createUnitGrid()
        {
            for (int k = 0; k < this.zHeight - 1; k++)
            {
                foreach (List<HexNode> row in grid.nodes[k])
                {
                    foreach (HexNode n in row)
                    {
                        if (n != null)
                        {
                            TriPrismUnit leftUnit = new TriPrismUnit(n.Pidx, n.Pidy, n.Pidz, 0, 0.5, this.iso);
                            TriPrismUnit rightUnit = new TriPrismUnit(n.Pidx, n.Pidy, n.Pidz, 1, 0.5, this.iso);
                            if (Math.Abs(n.Pidx - n.Pidy) == this.size)
                            {
                                if (n.Pidx > n.Pidy) rightUnit = null;
                                else leftUnit = null;
                            }
                            if (n.Pidx == this.size * 2 || n.Pidy == this.size * 2)
                            {
                                leftUnit = null;
                                rightUnit = null;
                            }
                            if (leftUnit != null) this.TriUnits.Add(leftUnit);
                            if (rightUnit != null) this.TriUnits.Add(rightUnit);
                        }
                    }
                }
            }
        }

        // update field
        public void updateFeild()
        {
            foreach (TriPrismUnit unit in this.TriUnits)
            {
                double[] newfield = new double[6];
                if (unit.IsRight == 1)
                {
                    newfield[0] = this.grid.nodes[unit.Pidz][unit.Pidx][unit.Pidy].Water;
                    newfield[1] = this.grid.nodes[unit.Pidz][unit.Pidx + 1][unit.Pidy + 1].Water;
                    newfield[2] = this.grid.nodes[unit.Pidz][unit.Pidx + 1][unit.Pidy].Water;
                    newfield[3] = this.grid.nodes[unit.Pidz + 1][unit.Pidx][unit.Pidy].Water;
                    newfield[4] = this.grid.nodes[unit.Pidz + 1][unit.Pidx + 1][unit.Pidy + 1].Water;
                    newfield[5] = this.grid.nodes[unit.Pidz + 1][unit.Pidx + 1][unit.Pidy].Water;
                }
                else
                {
                    newfield[0] = this.grid.nodes[unit.Pidz][unit.Pidx][unit.Pidy].Water;
                    newfield[1] = this.grid.nodes[unit.Pidz][unit.Pidx + 1][unit.Pidy + 1].Water;
                    newfield[2] = this.grid.nodes[unit.Pidz][unit.Pidx][unit.Pidy + 1].Water;
                    newfield[3] = this.grid.nodes[unit.Pidz + 1][unit.Pidx][unit.Pidy].Water;
                    newfield[4] = this.grid.nodes[unit.Pidz + 1][unit.Pidx + 1][unit.Pidy + 1].Water;
                    newfield[5] = this.grid.nodes[unit.Pidz + 1][unit.Pidx][unit.Pidy + 1].Water;
                }
                unit.Field = newfield;
            }
        }

        // update table index
        public void updateTableIndex()
        {
            foreach (TriPrismUnit unit in this.TriUnits)
            {
                unit.setCubeIndex();
            }
        }

        // gather out mesh
        public void getOutMesh()
        {
            List<Mesh> temp = new List<Mesh>();
            foreach (TriPrismUnit unit in this.TriUnits)
            {
                temp.AddRange(unit.getOutMesh());
            }
            this.outMesh = temp;
        }

        // display output mesh
        public List<Mesh> displayOutMesh()
        {
            return this.outMesh;
        }

        // check diffusion end
        public void checkDiffusionEnd() {
            foreach (List<HexNode> row in this.grid.nodes[this.zHeight / 2])
            {
                foreach (HexNode n in row)
                {
                    {
                        if (n != null && n.IsBoundary) if (n.Water >= 1) this.end = true;
                    }
                }
            }
            if (this.end)
            {
                foreach (List<HexNode> row in this.grid.nodes[this.zHeight / 2])
                {
                    foreach (HexNode n in row)
                    {
                        {
                            if (n != null && n.IsBoundary)  n.Water = 0;
                        }
                    }
                }
            }
        }

        // display triangle grid base
        public List<Mesh> displayTrigrid()
        {
            List<Mesh> meshGrid = new List<Mesh>();
            foreach (TriPrismUnit unit in this.TriUnits)
            {
                meshGrid.Add(unit.displayUnitBase());
            }
            return meshGrid;
        }

    }
}
