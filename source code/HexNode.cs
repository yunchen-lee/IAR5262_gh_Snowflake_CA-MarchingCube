using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;

namespace IAR5262_MarchingCube
{
    public class HexNode
    {
        // property
        // ----------------------------------------------------

        private int pidx;
        private int pidy;
        private int pidz;
        private Point3d absPos;
        private bool isboundary;
        private int size;
        private double water;
        private double u;
        private double v;
        private double newu;
        private double newv;
        private bool frozen;
        private bool receptive;

        // constructor
        // ----------------------------------------------------

        public HexNode(int _pidx,int _pidy,int _pidz,int _size,double _initu) { 
            this.pidx = _pidx;
            this.pidy = _pidy;
            this.pidz = _pidz;
            this.size = _size;
            this.u = _initu;
            this.v = 0;
            this.newu = 0;
            this.newv = 0;
            this.frozen = false;
            this.receptive = false;

            double x = this.pidx + this.pidy* Math.Cos(Math.PI / 3 * 2);
            double y = this.pidy* Math.Sin(Math.PI / 3 * 2);
            double z = this.pidz * 0.5;
            this.absPos = new Point3d(x, y, z);

            if (this.pidx == 0 || this.pidx == this.size * 2 ||
                this.pidy == 0 || this.pidy == this.size * 2 ||
                Math.Abs(this.pidx - this.pidy)==this.size) this.isboundary = true;

            this.water = this.u + this.v;
        }

        // method
        // ----------------------------------------------------
        // gamma
        public void runPhase1(double gamma) {
            this.water = this.u + this.v;


            if (this.receptive) {
                this.v = this.water;
                this.u = 0;
                this.addBGConstant(gamma);
                if (this.water >= 1) this.frozen = true;
            }
            else
            {
                this.v = 0;
                this.u = this.water;
            }
        
        }

        // alpha
        public void runPhase2(double alpha,double avg)
        {
            if (!this.frozen) this.diffusion(alpha,avg);

        }

        // beta
        public void runPhase3(double beta)
        {
            if (this.isboundary) this.newu = beta;


        }

        // update
        public void update() {
            this.u = this.newu;
            this.v = this.newv;
        }


        // diffusion
        public void diffusion(double alpha, double avg) {
            this.newu = this.u + alpha * (avg - this.u) / 2;
        }
        // background constant
        public void addBGConstant(double gamma) {
            this.newv = this.v + gamma;
        }

        // get set
        public double U { 
            get { return this.u; }
            set { this.u = value; }
        }

        public double Water {
            get { return this.water; }
            set { this.water = value; }
        }

        public bool Receptive
        {
            get { return this.receptive; }
            set { this.receptive = value; }
        }

        public int Pidx
        {
            get { return this.pidx; }
        }
        public int Pidy
        {
            get { return this.pidy; }
        }
        public int Pidz
        {
            get { return this.pidz; }
        }

        public Point3d Pos
        {
            get { return this.absPos; }
        }

        public bool IsBoundary
        {
            get { return this.isboundary; }
        }





    }
}
