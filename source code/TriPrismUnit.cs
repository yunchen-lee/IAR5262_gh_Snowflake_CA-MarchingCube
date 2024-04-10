using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;

namespace IAR5262_MarchingCube
{
    public class TriPrismUnit
    {
        // property
        // -------------------------------------------------
        private int pidx;                                // origin and type
        private int pidy;
        private int pidz;
        private int TypeRight;     // 1:True:Right, 0:False:Left   
        private double threshold;                        // midpoint propertices
        private double iso;
        private double[] field = new double[6];          // field
        private int cubeIndex;
        private int[,,] vc;                              // coner vertices
        private double[,] absvc = new double[6,3];
        private int[,] mp;                               // midpoint vertices
        private double[,] absmp = new double[9, 3];
        private int[,] TriTable;                         // table
        private List<Mesh> outMesh;                            // output mesh



        // constructor
        // -------------------------------------------------
        public TriPrismUnit(int _pidx,int _pidy,int _pidz, int _type,double _threshold,double _iso){

            this.pidx = _pidx;
            this.pidy = _pidy;
            this.pidz = _pidz;
            this.TypeRight = _type;

            this.threshold = _threshold;
            this.iso = _iso;

            //this.setCubeIndex();

            this.vc = new int[,,] { { { 0,0,0},{ 1,1,0},{ 0,1,0},{ 0,0,1},{ 1,1,1},{ 0,1,1} },
                                    { { 0,0,0},{ 1,1,0},{ 1,0,0},{ 0,0,1},{ 1,1,1},{ 1,0,1} } };
            this.absvc = this.setAbsConerPos();

            this.mp = new int[,] {  {0,1},{1,2},{2,0},
                                    {0,3},{1,4},{2,5},
                                    {3,4},{4,5},{5,3}};
            this.absmp = this.setAbsMidPos();

            this.TriTable = new int[,] {
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
                {0,2,3,-1,-1,-1,-1,-1,-1,-1},
                {0,1,4,-1,-1,-1,-1,-1,-1,-1},
                {1,2,3,1,3,4,-1,-1,-1,-1},
                {1,2,5,-1,-1,-1,-1,-1,-1,-1},
                {0,1,3,1,3,5,-1,-1,-1,-1},
                {0,2,5,0,4,5,-1,-1,-1,-1},
                {3,4,5,-1,-1,-1,-1,-1,-1,-1},
                {3,6,8,-1,-1,-1,-1,-1,-1,-1},
                {0,2,6,2,6,8,-1,-1,-1,-1},
                {0,1,4,3,6,8,-1,-1,-1,-1},
                {1,2,8,1,4,8,4,6,8,-1},
                {1,2,5,3,6,8,-1,-1,-1,-1},
                {0,1,5,0,5,6,5,6,8,-1},
                {3,6,8,0,2,5,0,4,5,-1},
                {4,5,6,5,6,8,-1,-1,-1,-1},
                {4,6,7,-1,-1,-1,-1,-1,-1,-1},
                {4,6,7,0,2,3,-1,-1,-1,-1},
                {0,1,6,1,6,7,-1,-1,-1,-1},
                {1,2,7,2,3,7,3,6,7,-1},
                {4,6,7,1,2,5,-1,-1,-1,-1},
                {0,1,5,0,3,5,4,6,7,-1},
                {0,2,5,0,5,6,5,6,7,-1},
                {3,5,6,5,6,7,-1,-1,-1,-1},
                {3,7,8,3,4,7,-1,-1,-1,-1},
                {0,4,7,0,2,7,2,7,8,-1},
                {0,1,3,1,3,8,1,7,8,-1},
                {1,2,7,2,7,8,-1,-1,-1,-1},
                {1,2,5,3,4,8,4,7,8,-1},
                {0,1,4,5,7,8,-1,-1,-1,-1},
                {5,7,8,0,2,3,-1,-1,-1,-1},
                {5,7,8,-1,-1,-1,-1,-1,-1,-1},
                {5,7,8,-1,-1,-1,-1,-1,-1,-1},
                {5,7,8,0,2,3,-1,-1,-1,-1},
                {0,1,4,5,7,8,-1,-1,-1,-1},
                {1,2,5,3,4,8,4,7,8,-1},
                {1,2,7,2,7,8,-1,-1,-1,-1},
                {0,1,3,1,3,8,1,7,8,-1},
                {0,4,7,0,2,7,2,7,8,-1},
                {3,7,8,3,4,7,-1,-1,-1,-1},
                {3,5,6,5,6,7,-1,-1,-1,-1},
                {0,2,5,0,5,6,5,6,7,-1},
                {0,1,5,0,3,5,4,6,7,-1},
                {4,6,7,1,2,5,-1,-1,-1,-1},
                {1,2,7,2,3,7,3,6,7,-1},
                {0,1,6,1,6,7,-1,-1,-1,-1},
                {4,6,7,0,2,3,-1,-1,-1,-1},
                {4,6,7,-1,-1,-1,-1,-1,-1,-1},
                {4,5,6,5,6,8,-1,-1,-1,-1},
                {3,6,8,0,2,5,0,4,5,-1},
                {0,1,5,0,5,6,5,6,8,-1},
                {1,2,5,3,6,8,-1,-1,-1,-1},
                {1,2,8,1,4,8,4,6,8,-1},
                {0,1,4,3,6,8,-1,-1,-1,-1},
                {0,2,6,2,6,8,-1,-1,-1,-1},
                {3,6,8,-1,-1,-1,-1,-1,-1,-1},
                {3,4,5,-1,-1,-1,-1,-1,-1,-1},
                {0,2,5,0,4,5,-1,-1,-1,-1},
                {0,1,3,1,3,5,-1,-1,-1,-1},
                {1,2,5,-1,-1,-1,-1,-1,-1,-1},
                {1,2,3,1,3,4,-1,-1,-1,-1},
                {0,1,4,-1,-1,-1,-1,-1,-1,-1},
                {0,2,3,-1,-1,-1,-1,-1,-1,-1},
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1}
                };

            this.outMesh = new List<Mesh>();
        }

        // method
        // -------------------------------------------------
        // get table index
        public void setCubeIndex()
        {
            int i = 0;
            for (int n = 0; n < 6; n++)
            {
                if (this.field[n] >= iso) { i = i + (int)(Math.Pow(2, n)); }
            }
            this.cubeIndex = i;
        }

        // set absolute position of coner
        public double[,] setAbsConerPos()
        {
            double[,] abspos = new double[6, 3];
            for (int n = 0; n < 6; n++)
            {
                int idx = this.pidx + this.vc[this.TypeRight,n, 0];
                int idy = this.pidy + this.vc[this.TypeRight,n, 1];
                int idz = this.pidz + this.vc[this.TypeRight,n, 2];

                abspos[n, 0] = idx + idy * Math.Cos(Math.PI / 3 * 2);
                abspos[n, 1] = idy * Math.Sin(Math.PI / 3 * 2);
                abspos[n, 2] = idz * 0.5;
            }
            return abspos;
        }

        // set absolute position of midpoint
        public double[,] setAbsMidPos()
        {
            double[,] abspos = new double[9, 3];
            for (int n = 0; n < 9; n++)
            {
                double x1 = this.absvc[this.mp[n, 0], 0]; double x2 = this.absvc[this.mp[n, 1], 0];
                double y1 = this.absvc[this.mp[n, 0], 1]; double y2 = this.absvc[this.mp[n, 1], 1];
                double z1 = this.absvc[this.mp[n, 0], 2]; double z2 = this.absvc[this.mp[n, 1], 2];

                abspos[n, 0] = x2 + (x1 - x2) * this.threshold;
                abspos[n, 1] = y2 + (y1 - y2) * this.threshold;
                abspos[n, 2] = z2 + (z1 - z2) * this.threshold;
            }
            return abspos;

        }

        // get output mesh
        public List<Mesh> getOutMesh() {
           List<Mesh> mesh = new List<Mesh>();
            for (int i = 0; this.TriTable[this.cubeIndex, i] != -1; i = i + 3)
            {
                Mesh m = this.addTriangle(i);
                mesh.Add(m);
            }

            this.outMesh = mesh;
            return this.outMesh;
        }

        // add an triangle to mesh
        public Mesh addTriangle(int i) {
            Point3d v1 = new Point3d(this.absmp[this.TriTable[this.cubeIndex, i], 0],
                                     this.absmp[this.TriTable[this.cubeIndex, i], 1], 
                                     this.absmp[this.TriTable[this.cubeIndex, i], 2]);
            Point3d v2 = new Point3d(this.absmp[this.TriTable[this.cubeIndex, i + 1], 0], 
                                     this.absmp[this.TriTable[this.cubeIndex, i + 1], 1], 
                                     this.absmp[this.TriTable[this.cubeIndex, i + 1], 2]);
            Point3d v3 = new Point3d(this.absmp[this.TriTable[this.cubeIndex, i + 2], 0],
                                     this.absmp[this.TriTable[this.cubeIndex, i + 2], 1], 
                                     this.absmp[this.TriTable[this.cubeIndex, i + 2], 2]);
            return this.createFace(v1, v2, v3);
        }

        // create an triangle mesh
        public Mesh createFace(Point3d v1,Point3d v2,Point3d v3) {
            Mesh eachMesh = new Mesh();
            eachMesh.Vertices.Add(v1);
            eachMesh.Vertices.Add(v2);
            eachMesh.Vertices.Add(v3);
            eachMesh.Faces.AddFace(0, 1, 2);
            eachMesh.Normals.ComputeNormals();
            return eachMesh;
        }

        // display unit
        public Mesh displayUnitBase() { 
            Mesh unitbase = new Mesh();
            unitbase.Vertices.Add(this.absvc[0, 0], this.absvc[0, 1], this.absvc[0, 2]);
            unitbase.Vertices.Add(this.absvc[1, 0], this.absvc[1, 1], this.absvc[1, 2]);
            unitbase.Vertices.Add(this.absvc[2, 0], this.absvc[2, 1], this.absvc[2, 2]);
            unitbase.Faces.AddFace(0, 1, 2);
            unitbase.Normals.ComputeNormals();
            return unitbase;
        }

        // get set
        public double[] Field { 
            get { return this.field; }
            set { this.field = value; }
        }

        public double Threshold { 
            get { return this.threshold; }
            set { this.threshold = value; }
        }

        public double Iso {
            get { return this.iso; }
            set { this.iso = value; }
        }

        public int IsRight
        {
            get { return this.TypeRight; }
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




    }
}
