using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Collections;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Model.Operations;

namespace Stężenie_pionowe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MyModel = new Model();
            textBox1.Text = "BL10*60";
            textBox2.Text = "150";
            textBox3.Text = "170";
            textBox4.Text = "0";
            textBox5.Text = "0";
        }

        private readonly Model MyModel;

        private void button1_Click(object sender, EventArgs e)
        {
            if (MyModel.GetConnectionStatus())
            {

                double off1 = Convert.ToDouble(textBox4.Text);
                double off2 = Convert.ToDouble(textBox5.Text);
                TransformationPlane currentPlane = MyModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                Picker piku = new Picker();

                ArrayList info = new ArrayList();
                info = new ArrayList { "END_X", "END_Y", "END_Z", "START_X", "START_Y", "START_Z" };
                ArrayList info2 = new ArrayList();
                info2 = new ArrayList { "END_X", "END_Y", "END_Z", "START_X", "START_Y", "START_Z" };
                ModelObject part = new Beam();
                ModelObject part2 = new Beam();
                Hashtable p1 = new Hashtable();
                Hashtable p2 = new Hashtable();
                part = piku.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
                part.GetDoubleReportProperties(info, ref p1);
                double rotpart = (part as Part).Position.RotationOffset;
                part2 = piku.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
                part2.GetDoubleReportProperties(info2, ref p2);
                double rotpart2 = (part2 as Part).Position.RotationOffset;


                Point st1 = new Point(Convert.ToDouble(p1["START_X"]), Convert.ToDouble(p1["START_Y"]), Convert.ToDouble(p1["START_Z"]));
                Point st2 = new Point(Convert.ToDouble(p2["END_X"]), Convert.ToDouble(p2["END_Y"]), Convert.ToDouble(p2["END_Z"]));
                Point st3 = new Point(Convert.ToDouble(p1["END_X"]), Convert.ToDouble(p1["END_Y"]), Convert.ToDouble(p1["END_Z"]));
                Point st4 = new Point(Convert.ToDouble(p2["START_X"]), Convert.ToDouble(p2["START_Y"]), Convert.ToDouble(p2["START_Z"]));

                LineSegment l1 = new LineSegment(st1, st2);
                LineSegment l3 = new LineSegment(st1, st3); Vector vl3 = l3.GetDirectionVector(); vl3.Normalize();
                LineSegment l4 = new LineSegment(st2, st1);
                LineSegment l5 = new LineSegment(st2, st4); Vector vl5 = l5.GetDirectionVector(); vl5.Normalize();

                st1.Translate(vl3.X*(off1/vl3.GetLength()), vl3.Y * (off1 / vl3.GetLength()), vl3.Z * (off1 / vl3.GetLength()));
                st2.Translate(vl5.X * (off2 / vl5.GetLength()), vl5.Y * (off2 / vl5.GetLength()), vl5.Z * (off2 / vl5.GetLength()));


                ControlLine cl1 = new ControlLine(l1, true);
                cl1.Color = ControlLine.ControlLineColorEnum.YELLOW_RED;

                //cl1.Insert();

                CoordinateSystem cs1 = new CoordinateSystem();
                cs1 = part.GetCoordinateSystem();
                CoordinateSystem cs2 = new CoordinateSystem();
                cs2 = part2.GetCoordinateSystem();

                Tekla.Structures.Model.Plane pl1 = new Tekla.Structures.Model.Plane();
                pl1.AxisX = cs1.AxisX;
                pl1.AxisY = cs1.AxisY;
                pl1.Origin = cs1.Origin;

                Tekla.Structures.Model.Plane pl2 = new Tekla.Structures.Model.Plane();
                pl2.AxisX = cs2.AxisX;
                pl2.AxisY = cs2.AxisY;
                pl2.Origin = cs2.Origin;

                GeometricPlane gp1 = new GeometricPlane(cs1);
                GeometricPlane gp2 = new GeometricPlane(cs2);

                ControlPlane cp1 = new ControlPlane(pl1, true);
                //cp1.Insert();
                ControlPlane cp2 = new ControlPlane(pl2, true);
                //cp2.Insert();

                LineSegment l2 = Projection.LineSegmentToPlane(l1, gp1);
                LineSegment l6 = Projection.LineSegmentToPlane(l4, gp2);

                ControlLine cl2 = new ControlLine(l2, true);
                ControlLine cl3 = new ControlLine(l6, true);


                cl2.Color = ControlLine.ControlLineColorEnum.YELLOW_RED;
                //cl2.Insert();
                cl3.Color = ControlLine.ControlLineColorEnum.YELLOW_RED;
                //cl3.Insert();

                Part p3 = part as Part;
                Solid sl1 = p3.GetSolid();
                ArrayList al1 = sl1.Intersect(l2);
                Point px = new Point(al1[1] as Point);
                ControlPoint cpp1 = new ControlPoint(px);
                //cpp1.Insert();

                Part p4 = part2 as Part;
                Solid sl2 = p4.GetSolid();
                ArrayList al2 = sl2.Intersect(l6);
                Point py = new Point(al2[1] as Point);
                ControlPoint cpp4 = new ControlPoint(py);
                //cpp4.Insert();

                double distance = Distance.PointToLineSegment(st3, l2);
                double distance1 = l3.Length();
                double cos = distance/distance1;
                double distance2 = Distance.PointToLineSegment(st4, l6);
                double distance3 = l5.Length();
                double cos1 = distance2 / distance3;

                Point p10 = dlugosc(st1, st3, 4, cos, px);
                ControlPoint cpp2 = new ControlPoint(p10);
                //cpp2.Insert();
                Point p20 = dlugosc(st2, st4, 4, cos1, py);
                ControlPoint cpp5 = new ControlPoint(p20);
                //cpp5.Insert();


                Point p5 = new Point(l2.StartPoint);
                Point p6 = new Point(l2.EndPoint);
                Vector v1 = new Vector((p6.X-p5.X)/10000, (p6.Y-p5.Y)/10000, (p6.Z-p5.Z)/10000);
                double v1d = v1.GetLength();
                double dlbl = Convert.ToDouble(textBox2.Text);
                Point p11 = new Point(px);
                p11.Translate(v1.X*(dlbl/v1d), v1.Y*(dlbl/v1d), v1.Z*(dlbl/v1d));
                Point p12 = dlugosc(st1, st3, 4, cos, p11);
                ControlPoint cpp3 = new ControlPoint(p12);
                //cpp3.Insert();
                Point p7 = new Point(l6.StartPoint);
                Point p8 = new Point(l6.EndPoint);
                Vector v2 = new Vector((p8.X - p7.X) / 10000, (p8.Y - p7.Y) / 10000, (p8.Z - p7.Z) / 10000);
                double v2d = v2.GetLength();
                double dlbl1 = Convert.ToDouble(textBox2.Text);
                Point p21 = new Point(py);
                p21.Translate(v2.X * (dlbl1 / v2d), v2.Y * (dlbl1 / v2d), v2.Z * (dlbl1 / v2d));
                Point p22 = dlugosc(st2, st4, 4, cos1, p21);
                ControlPoint cpp6 = new ControlPoint(p22);
                //cpp6.Insert();


                Beam blachast = CreateBlacha(p10, p12, rotpart, 0, textBox1.Text);
                blachast.Insert();
                int id1 = blachast.Identifier.ID;
                Identifier id11 = new Identifier(id1);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st1, vl3, v1));
                ModelObject blachast3 = MyModel.SelectModelObject(id11);
                double rotblachast = (blachast3 as Beam).Position.RotationOffset;
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
                //textBox1.Text = Convert.ToString(rotblachast);
                blachast.Position.RotationOffset = -rotblachast;
                blachast.Modify();


                Beam blachast2 = CreateBlacha(p20, p22, rotpart2, 1, textBox1.Text);
                blachast2.Insert();
                int id2 = blachast2.Identifier.ID;
                Identifier id12 = new Identifier(id2);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st2, vl5, v2));
                ModelObject blachast4 = MyModel.SelectModelObject(id12);
                double rotblachast2 = (blachast4 as Beam).Position.RotationOffset;
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
                blachast2.Position.RotationOffset = -rotblachast2;
                blachast2.Modify();

                Point p15 = new Point(p12);
                p15.Translate(v1.X * (-30 / v1d), v1.Y * (-30 / v1d), v1.Z * (-30 / v1d));
                ControlPoint cpp7 = new ControlPoint(p15);
                //cpp7.Insert();

                Point p16 = new Point(p22);
                p16.Translate(v2.X * (-30 / v2d), v2.Y * (-30 / v2d), v2.Z * (-30 / v2d));
                ControlPoint cpp8 = new ControlPoint(p16);
                //cpp8.Insert();

                Vector v11516 = new Vector((p16.X - p15.X) / 10000, (p16.Y - p15.Y) / 10000, (p16.Z - p15.Z) / 10000);

                Point p151 = new Point(p15); Point p152 = new Point(p15); Point p153 = new Point(p15);
                Point p161 = new Point(p16); Point p162 = new Point(p16); Point p163 = new Point(p16);

                p151.Translate(v11516.X * (-30 / v11516.GetLength()), v11516.Y * (-30 / v11516.GetLength()), v11516.Z * (-30 / v11516.GetLength()));
                p152.Translate(v11516.X * (120 / v11516.GetLength()), v11516.Y * (120 / v11516.GetLength()), v11516.Z * (120 / v11516.GetLength()));
                p153.Translate(v11516.X * (70 / v11516.GetLength()), v11516.Y * (70 / v11516.GetLength()), v11516.Z * (70 / v11516.GetLength()));

                p161.Translate(v11516.X * (30 / v11516.GetLength()), v11516.Y * (30 / v11516.GetLength()), v11516.Z * (30 / v11516.GetLength()));
                p162.Translate(v11516.X * (-120 / v11516.GetLength()), v11516.Y * (-120 / v11516.GetLength()), v11516.Z * (-120 / v11516.GetLength()));
                p163.Translate(v11516.X * (-70 / v11516.GetLength()), v11516.Y * (-70 / v11516.GetLength()), v11516.Z * (-70 / v11516.GetLength()));

                Beam BlSt1 = BlachaSt(p151, p152, 0);
                BlSt1.Insert();
                int id3 = BlSt1.Identifier.ID;
                Identifier id13 = new Identifier(id3);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st1, vl3, v1));
                ModelObject blachast5 = MyModel.SelectModelObject(id13);
                double rot5 = (blachast5 as Beam).Position.RotationOffset;
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
                //textBox1.Text = Convert.ToString(rotblachast);
                BlSt1.Position.RotationOffset = -rot5;
                BlSt1.Modify();

                Beam BlSt2 = BlachaSt(p161, p162, 1);
                BlSt2.Insert();
                int id4 = BlSt2.Identifier.ID;
                Identifier id14 = new Identifier(id4);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st2, vl5, v2));
                ModelObject blachast6 = MyModel.SelectModelObject(id14);
                double rot6 = (blachast6 as Beam).Position.RotationOffset;
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
                //textBox1.Text = Convert.ToString(rotblachast);
                BlSt2.Position.RotationOffset = -rot6;
                BlSt2.Modify();

                Beam St12 = CreateStezenie12(p153, p163);
                St12.Insert();
                int id5 = St12.Identifier.ID;
                Identifier id15 = new Identifier(id5);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st1, vl3, v1));
                ModelObject stezenie1 = MyModel.SelectModelObject(id15);
                (stezenie1 as Beam).StartPointOffset.Dy = 4;
                stezenie1.Modify();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st2, vl5, v2));
                ModelObject stezenie2 = MyModel.SelectModelObject(id15);
                (stezenie2 as Beam).EndPointOffset.Dy = -6;
                stezenie2.Modify();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);

                double split = Convert.ToDouble(textBox3.Text);
                Point pointsplit = new Point(p163);
                pointsplit.Translate(v11516.X * (-split / v11516.GetLength()), v11516.Y * (-split / v11516.GetLength()), v11516.Z * (-split / v11516.GetLength()));


                BoltArray b1 = sruby(p151, p152, BlSt1, blachast);
                b1.Insert();
                int id6 = b1.Identifier.ID;
                Identifier id16 = new Identifier(id6);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st1, vl3, v1));
                ModelObject b1m = MyModel.SelectModelObject(id16);
                (b1m as BoltArray).Position.RotationOffset = 0;
                b1m.Modify();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);

                BoltArray b2 = sruby(p161, p162, BlSt2, blachast2);
                b2.Insert();
                int id7 = b2.Identifier.ID;
                Identifier id17 = new Identifier(id7);
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(setWorkPlane(st2, vl5, v2));
                ModelObject b2m = MyModel.SelectModelObject(id17);
                (b2m as BoltArray).Position.RotationOffset = 180;
                b2m.Modify();
                MyModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);


                Weld w = new Weld();
                Weld w2 = new Weld();
                Weld w3 = new Weld();
                Weld w4 = new Weld();
                w.MainObject = stezenie2;
                w.SecondaryObject = BlSt1;
                w.ShopWeld = true;
                w.Insert();
                w2.MainObject = stezenie2;
                w2.SecondaryObject = BlSt2;
                w2.ShopWeld = true;
                w2.Insert();
                w3.MainObject = part;
                w3.SecondaryObject = blachast;
                w3.ShopWeld = true;
                w3.Insert();
                w4.MainObject = part2;
                w4.SecondaryObject = blachast2;
                w4.ShopWeld = true;
                w4.Insert();
                Beam st122 = Operation.Split(stezenie2 as Beam, pointsplit);
                Connection sr = new Connection();
                sr.Name = "Połączenie śrubą rzymską";
                sr.Number = 126;
                sr.LoadAttributesFromFile("82269_M12");
                sr.SetPrimaryObject(stezenie2);
                sr.SetSecondaryObject(st122);
                sr.Insert();






            }

            
            MyModel.CommitChanges();
        }

        private static Point dlugosc(Point x, Point y, double l, double kat, Point prz)
        {
            double x1 = x.X;
            double y1 = x.Y;
            double z1 = x.Z;
            double x2 = y.X;
            double y2 = y.Y;
            double z2 = y.Z;
            double dl = l/kat;

            Vector v = new Vector((x2 - x1) / 10000, (y2 - y1) / 10000, (z2 - z1) / 10000);
            double d = v.GetLength();
            double skrocenie = dl;
            double ile = skrocenie / d;
            Vector v2 = ile * v;
            Point p4 = new Point(prz);
            p4.Translate(v2.X, v2.Y, v2.Z);

            return p4;

        }

        private static Beam CreateBlacha(Point X, Point Y, double rot, double c, string jakablacha)
        {
            Beam blacha = new Beam();

            blacha.StartPoint = X;
            blacha.EndPoint = Y;
            blacha.Name = "BLACHA";
            blacha.Profile.ProfileString = jakablacha;
            blacha.Material.MaterialString = "S355JR";
            blacha.Class = "11";
            blacha.Position.Plane = Position.PlaneEnum.MIDDLE;
            if (c == 0)
            {
                blacha.Position.Depth = Position.DepthEnum.BEHIND;
            }
            else
            {
                blacha.Position.Depth = Position.DepthEnum.FRONT;
            }
           
            blacha.Position.Rotation = Position.RotationEnum.BACK;
            blacha.Position.RotationOffset = 0;

            return blacha;
        }

        private static Beam BlachaSt(Point X, Point Y, double c)
        {
            Beam blacha = new Beam();

            blacha.StartPoint = X;
            blacha.EndPoint = Y;
            blacha.Name = "BLACHA";
            blacha.Profile.ProfileString = "BL10*60";
            blacha.Material.MaterialString = "S355JR";
            blacha.Class = "7";
            blacha.Position.Plane = Position.PlaneEnum.MIDDLE;
            if (c == 0)
            {
                blacha.Position.Depth = Position.DepthEnum.BEHIND;
                blacha.Position.DepthOffset = 10;
            }
            else
            {
                blacha.Position.Depth = Position.DepthEnum.FRONT;
                blacha.Position.DepthOffset = -10;
            }

            blacha.Position.Rotation = Position.RotationEnum.BACK;
            blacha.Position.RotationOffset = 0;

            return blacha;
        }

        private static Beam CreateStezenie12(Point x, Point y)
        {
            Beam belka = new Beam();


            belka.StartPoint = x;
            belka.EndPoint = y;
            belka.Name = "PROFIL";
            belka.Profile.ProfileString = "D12";
            belka.Material.MaterialString = "S355JR";
            belka.Class = "7";
            belka.Position.Plane = Position.PlaneEnum.MIDDLE;
            belka.Position.Depth = Position.DepthEnum.MIDDLE;
            belka.StartPointOffset.Dy = 0;
            belka.EndPointOffset.Dy = 0;

            return belka;
        }

        private static BoltArray sruby(Point x, Point y, Part dop, Part dop2)

        {
            BoltArray bg = new BoltArray();

            bg.FirstPosition = x;
            bg.SecondPosition = y;
            bg.BoltSize = 16.0;
            bg.BoltStandard = "931-8.8";
            bg.Bolt = true;
            bg.Nut1 = true;
            bg.Washer1 = true;
            bg.Washer2 = false;
            bg.Washer3 = true;
            bg.Position.Depth = Position.DepthEnum.MIDDLE;
            bg.Position.DepthOffset = 0;
            bg.Position.Plane = Position.PlaneEnum.MIDDLE;
            bg.Position.PlaneOffset = 0;
            bg.Position.Rotation = Position.RotationEnum.BACK;
            bg.Position.RotationOffset = 0;
            bg.BoltType = BoltArray.BoltTypeEnum.BOLT_TYPE_SITE;
            bg.PartToBeBolted = dop;
            bg.PartToBoltTo = dop2;
            bg.AddBoltDistX(0);
            bg.AddBoltDistY(0);
            bg.CutLength = 50;
            bg.StartPointOffset.Dx = 30;


            return bg;
        }
            private static TransformationPlane setWorkPlane(Point x, Vector X, Vector Y)
        {
            CoordinateSystem XZ = new CoordinateSystem();
            //Define origin and two vectors to set the UCS to the XZ plane
            XZ.Origin = x;
            XZ.AxisX = X;
            XZ.AxisY = Y;

            TransformationPlane XZ_Plane = new TransformationPlane(XZ);

            return XZ_Plane;

        }
    

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
