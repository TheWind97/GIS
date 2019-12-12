using AxMapWinGIS;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GISApp
{
    public partial class Form1 : Form
    {
        public List<int> indexLayers;
        public List<int> bandIndex= new List<int>();
        public bool iRasterCheck;
        private Shapefile shp_tmp= new Shapefile();
        private Shape shpDraw = new Shape();
        int indexTxt;
        // Context Menu of Tree Layers
        ContextMenu toolTipMenuLayer = new ContextMenu();
        // Current Layer Index Choiced
        int currentLayerIndex;
        // Pre Selected Shape
        int preSelectedShape=-1;
        public Form1()
        {
            iRasterCheck = false;
            InitializeComponent();
            InitControl();

        }

        private void InitControl()
        {
            treeLayers.CheckBoxes = true;
            MenuItem layerOpenAtt = new MenuItem("Open Attribute");
            layerOpenAtt.Click += layerOpenAtt_Click;
            MenuItem layerRemove = new MenuItem("Remove Layer");
            layerRemove.Click += layerRemove_Click;

            toolTipMenuLayer.MenuItems.Add(layerOpenAtt);
            toolTipMenuLayer.MenuItems.Add(layerRemove);
            //treeLayers.AfterCheck += treeLayer_AfterCheck;
            treeLayers.NodeMouseClick += treeLayer_NodeMouseClick;

            // Map Control
            mapControl.Identifier.IdentifierMode = tkIdentifierMode.imAllLayersStopOnFirst;
            

        }
        /// <summary>
        /// Open Attributes Table of Layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layerOpenAtt_Click(object sender, EventArgs e)
        {
            MessageBox.Show(treeLayers.SelectedNode.ToString());
            var shp = mapControl.get_Shapefile(0);

        }

        private void layerRemove_Click(object sender, EventArgs e)
        {
            mapControl.RemoveLayer(currentLayerIndex);
            treeLayers.Nodes[0].Nodes.RemoveAt(currentLayerIndex);
            treeLayers.Nodes[0].Checked = (treeLayers.Nodes[0].Nodes.Count == 0) ? false : true;

            listData.Clear();
            splitContainer1.Panel2Collapsed = true;
            listData.Visible = false;
            
        }


        private void treeLayer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            TreeNode currentNode = treeLayers.GetNodeAt(e.X, e.Y);
            treeLayers.SelectedNode = currentNode;
            if (currentNode == treeLayers.Nodes[0] || currentNode == null ) return;
            toolTipMenuLayer.Show(treeLayers, new System.Drawing.Point(e.X, e.Y));
            currentLayerIndex = currentNode.Index;

        }
        private void treeLayer_AfterCheck(object sender, TreeViewEventArgs e)
        {
            mapControl.set_LayerVisible(e.Node.Index, e.Node.Checked);

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            this.openDialog.Title ="Open data";
            MapWinGIS.Image img = new MapWinGIS.Image();
            

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                this.AddLayers(openDialog.FileName);
                //foreach (String name in openDialog.FileNames)
                //{
                //    
                //}
            }


        }
        public bool AddLayers(string name)
        {
            //this.mapControl.RemoveAllLayers();
            
            //this.mapControl.LockWindow(tkLockMode.lmLock);
            if (name.ToLower().EndsWith(".shp"))
            {
                Shapefile shp = new Shapefile();
                shp.Open(name, null);
                int layerID = mapControl.AddLayer(shp, true);
                //indexLayers.Insert(0, layerID);
            }
            else if (openDialog.SafeFileName.ToLower().EndsWith(".tif") ||
                openDialog.SafeFileName.ToLower().EndsWith(".png") ||
                openDialog.SafeFileName.ToLower().EndsWith(".jpg"))
            {
                MapWinGIS.Image img = new MapWinGIS.Image();
                img.Open(name, ImageType.TIFF_FILE, false, null);
                img.UseRgbBandMapping = true;
                for (int i = 0; i < img.NoBands; i++)
                {
                    bandIndex.Add(i);
                }
                //MessageBox.Show(img.UseRgbBandMapping.ToString());
                int layer = mapControl.AddLayer(img, true);

                //indexLayers.Insert(0, layer);

            }
            

            // List on Tree Layers
            var splited = openDialog.FileName.Split('\\');
            treeLayers.CheckBoxes = true;
            treeLayers.AfterCheck += treeLayer_AfterCheck;
            TreeNode layerNode = new TreeNode(splited[splited.Length - 1].Split('.')[0]);
            layerNode.Checked = true;
            treeLayers.Nodes[0].Nodes.Add(layerNode);
            layerNode.Parent.Checked = true;

            // Draw Marker
            shp_tmp = new Shapefile();
            if (!shp_tmp.CreateNewWithShapeID("", ShpfileType.SHP_POINT))
            {
                MessageBox.Show("Failed to create shapefile: " + shp_tmp.ErrorMsg[shp_tmp.LastErrorCode]);
                return false;
            }
            int layer_tmp = mapControl.AddLayer(shp_tmp, true);

            mapControl.CursorMode = tkCursorMode.cmNone;
            mapControl.MouseDownEvent += mapControl_MouseDownEvent;
            return this.mapControl.NumLayers>0;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.toolOpen.PerformClick();
        }

        private void toolZoomIn_Click(object sender, EventArgs e)
        {
            mapControl.CursorMode = MapWinGIS.tkCursorMode.cmZoomIn;
        }

        private void toolZoomOut_Click(object sender, EventArgs e)
        {
            mapControl.CursorMode = MapWinGIS.tkCursorMode.cmZoomOut;
        }

        private void toolZoomExtent_Click(object sender, EventArgs e)
        {
            mapControl.ZoomToMaxExtents();
        }

        private void toolPan_Click(object sender, EventArgs e)
        {
            mapControl.CursorMode = MapWinGIS.tkCursorMode.cmPan;
        }

        private void toolNone_Click(object sender, EventArgs e)
        {
            mapControl.CursorMode = MapWinGIS.tkCursorMode.cmNone;
        }

        private void attributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }
        public void InitValuesAttributes(String[] columnNames, String[,] cellValues)
        {
            int numberColumns = columnNames.Length;
            // Add Header Columns:
            for (int i = 0; i < numberColumns; i++)
            {
                listData.Columns.Add(columnNames[i], 80, HorizontalAlignment.Center);
            }


            // Load Data
            for (int i = 0; i < cellValues.GetLength(0); i++)
            {
                String[] rowData = new String[numberColumns];
                for (int j = 0; j < cellValues.GetLength(1); j++)
                {
                    rowData[j] = cellValues[i, j];
                }
                ListViewItem item = new ListViewItem(rowData);
                listData.Items.Add(item);
            }

        }

        private void identifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shapefile sf = new Shapefile();
            sf = mapControl.get_Shapefile(0);
            mapControl.CursorMode = tkCursorMode.cmIdentify;
            mapControl.ShapeIdentified += MapControl_ShapeIdentified;
            //mapControl.SendMouseUp = true;
            //mapControl.MouseUpEvent += mapControl_MouseUpEvent;


        }

        

        private void MapControl_ShapeHighlighted(object sender, _DMapEvents_ShapeHighlightedEvent e)
        {
            Shapefile sf = mapControl.get_Shapefile(e.layerHandle);
            if( sf!=null)
            {
                int idFeature = e.shapeIndex;
                // Unselected
                if( listData.SelectedIndices.Count>0)
                {
                    for (int i = 0; i < listData.SelectedIndices.Count; i++)
                    {
                        listData.Items[listData.SelectedIndices[i]].Selected = false;
                    }
                }
                if (listData.Items.Count>0)
                {
                    listData.Items[idFeature].Selected = true;
                }
                
            }

        }

        private void MapControl_ShapeIdentified(object sender, _DMapEvents_ShapeIdentifiedEvent e)
        {
            
            Shapefile sf = mapControl.get_Shapefile(e.layerHandle);
            
            if (sf != null)
            {
                int idFeature = e.shapeIndex;
                // Unselected
                if (listData.SelectedIndices.Count > 0)
                {
                    for (int i = 0; i < listData.SelectedIndices.Count; i++)
                    {
                        listData.Items[listData.SelectedIndices[i]].Selected = false;
                    }
                }
                if (listData.Items.Count > 0)
                {
                    listData.Items[idFeature].Selected = true;
                }

            }

        }

        private void MapControl_ShapeLabeling(object sender, _DMapEvents_ShapeHighlightedEvent e)
        {
            
            Shapefile sf = mapControl.get_Shapefile(e.layerHandle);
            if (sf != null)
            {
                sf.SelectionAppearance = tkSelectionAppearance.saDrawingOptions;
                int idFeature = e.shapeIndex;
                if (preSelectedShape!=-1)
                {
                    sf.ShapeSelected[preSelectedShape] = false;
                }
                sf.ShapeSelected[idFeature] = true;
                preSelectedShape = idFeature;


                if (listLabel.SelectedIndices.Count > 0)
                {
                    for (int i = 0; i < listLabel.SelectedIndices.Count; i++)
                    {
                        listLabel.Items[listLabel.SelectedIndices[i]].Selected = false;
                    }
                }
                if (listLabel.Items.Count > 0)
                {
                    listLabel.Items[idFeature].Selected = true;
                }

                
            }
            mapControl.Redraw();
        }


        private void listData_SelectedIndexChanged(object sender, EventArgs e)
        {

            if(listData.SelectedIndices.Count>0)
            {
                Shapefile sf = mapControl.get_Shapefile(0);
                sf.SelectNone();
                string error = "";
                object result = null;
                string index= (listData.SelectedIndices[0]+1).ToString();
                string query = "[ID]=" + index;
                if(sf.Table.Query(query,ref result, ref error))
                {
                    int[] shapes = result as int[];
                    if( shapes!= null)
                    {
                        for (int i = 0; i < shapes.Length; i++)
                        {
                            sf.set_ShapeSelected(shapes[i], true);
                        }
                    }
                    //mapControl.Update();
                    mapControl.ZoomToSelected(0);
                    //MessageBox.Show("Object Selected:" + sf.NumSelected);

                }
            }
            
        }

        private void toolGetValues_Click(object sender, EventArgs e)
        {
            if(toolGetValues.Checked)
            {
                toolGetValues.Checked = false;
                mapControl.RemoveLayer(mapControl.NumLayers - 1);
            }
            else
            {
                toolGetValues.Checked = true;
                
                ShapeDrawingOptions options = shp_tmp.DefaultDrawingOptions;
                options.PointType = tkPointSymbolType.ptSymbolStandard;
                shp_tmp.CollisionMode = tkCollisionMode.AllowCollisions;

            }

        }


        private void mapControl_MouseDownEvent(object sender, _DMapEvents_MouseDownEvent e)
        {
            
            
            

            if (e.button==2 && listLabel.Visible)
            {
                ListViewItem x = listLabel.Items[listLabel.SelectedIndices[0]];
                string type = x.SubItems[1].Text;
                FormEnterLabel enter = new FormEnterLabel(preSelectedShape.ToString(), type);
                enter.ShowDialog();
                string str_ = enter.label;
                String[] rowData = new String[3];
                rowData[0] = preSelectedShape.ToString();
                rowData[1] = type;
                rowData[2] = str_;

                ListViewItem item = new ListViewItem(rowData);
                listLabel.Items.RemoveAt(preSelectedShape);
                listLabel.Items.Insert(preSelectedShape, item);
            }


            if (!toolGetValues.Checked)
            {
                return;
            }
            Shape shp = new Shape();
            if (e.button == 1)          // left button
            {
                Shapefile sf = mapControl.get_Shapefile(mapControl.NumLayers-1);
                
                shp.Create(ShpfileType.SHP_POINT);
                MapWinGIS.Point pnt = new MapWinGIS.Point();
                double x = 0.0;
                double y = 0.0;
                mapControl.PixelToProj(e.x, e.y, ref x, ref y);
                Console.WriteLine(x.ToString() + "," + y.ToString());
                pnt.x = x;
                pnt.y = y;
                int index = shp.numPoints;
                shp.InsertPoint(pnt, ref index);
                index = sf.NumShapes;
                if (!sf.EditInsertShape(shp, ref index))
                {
                    MessageBox.Show("Failed to insert shape: " + sf.ErrorMsg[sf.LastErrorCode]);
                    return;
                }
                mapControl.Redraw();
            }
            else
            {
                return;
            }
            
            
            int row;
            int column;

            double X = 0;
            double Y = 0;
            mapControl.PixelToProj(e.x, e.y, ref X, ref Y);
            MapWinGIS.Image img = mapControl.get_Image(0);
            img.ProjectionToImage(X, Y, out column, out row);
            double[] vals = new double[img.NoBands];
            for (int i = 1; i <= img.NoBands; i++)
            {
                GdalRasterBand rst = img.get_Band(i);
                double pVal;
                rst.get_Value(row, column, out pVal);
                vals[i - 1] = pVal;
            }

            FormRasterValue uiRasterValues = new FormRasterValue(vals);
            uiRasterValues.ShowDialog();
            shp.Clear();
            
            
        }

        private void getValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolGetValues.PerformClick();
        }

        private void openAttributesTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!listData.Visible)
            {
                splitContainer1.Panel2Collapsed = false;
                listData.Visible = true;
                splitContainer3.Panel2Collapsed = true;
                splitContainer3.Panel1Collapsed = false;
            }
            else
            {
                listData.Clear();
                listData.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                return;

            }

            listData.Width = 400;
            MapWinGIS.Table table = new Table();
            String dataName = this.openDialog.FileName.Replace(".shp", ".dbf");
            table.Open(dataName, null);

            int numberRows = table.NumRows;
            int numberFields = table.NumFields;
            String[,] cellValues = new String[numberRows, numberFields];
            String[] columnNames = new String[numberFields];
            for (int i = 0; i < numberFields; i++)
            {
                columnNames[i] = table.Field[i].Name;
            }

            for (int i = 0; i < numberRows; i++)
            {
                for (int j = 0; j < numberFields; j++)
                {
                    cellValues[i, j] = System.Convert.ToString(table.get_CellValue(j, i));
                }
            }

            //FormTableAttributes uiAttributes = new FormTableAttributes(columnNames,cellValues);
            //uiAttributes.ShowDialog();
            InitValuesAttributes(columnNames, cellValues);

        }

        private void mapControl_MouseUpEvent(object sender, _DMapEvents_MouseUpEvent e)
        {
            
        }

        private void layersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (splitContainer2.Panel1Collapsed)
            {
                splitContainer2.Panel1Collapsed = false;
            }
            else
            {
                splitContainer2.Panel1Collapsed = true;
            }
        }

        private void loadPolygonFromTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
                
            }
            List<MapWinGIS.Point> coords = new List<MapWinGIS.Point>();
            string file_txt = openDialog.FileName;
            string[] lines = System.IO.File.ReadAllLines(file_txt);
            for (int i = 0; i < lines.Length; i++)
            {
                MapWinGIS.Point pTmp = new MapWinGIS.Point();
                string line = lines[i];
                if (line!="")
                {
                    string[] arr_line = line.Split(' ');
                    double latt = Convert.ToDouble(arr_line[1]);
                    double longt = Convert.ToDouble(arr_line[2]);
                    pTmp.x = latt;
                    pTmp.y = longt;
                }
                else
                {
                    pTmp.x = 0.0;
                    pTmp.y = 0.0;
                }

                coords.Add(pTmp);
            }

            // Create Shaefile load Polygon
            var sfPoly = new Shapefile();
            bool result = sfPoly.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON);
            if (!result)
            {
                MessageBox.Show(sfPoly.ErrorMsg[sfPoly.LastErrorCode]);
            }
            else
            {
                Shape shpPoly= new Shape();
                shpPoly.Create(ShpfileType.SHP_POLYGON);
                int numberPoly = 0;
                int numPoint = 0;
                for (int i = 0; i < coords.Count; i++)
                {
                    if(coords[i].x==0.0)
                    {
                        if (i != 0)
                        {
                            sfPoly.EditInsertShape(shpPoly, ref numberPoly);
                            numberPoly += 1;
                        }
                        numPoint = 0 ;
                        
                        shpPoly = new Shape();
                        shpPoly.Create(ShpfileType.SHP_POLYGON);
                        
                    }
                    else
                    {
                        shpPoly.InsertPoint(coords[i], ref numPoint);
                        numPoint += 1;
                    }
                    
                }
                sfPoly.EditInsertShape(shpPoly, ref numberPoly);

            }


            indexTxt= mapControl.AddLayer(sfPoly,true);
            // Add to Tree
            treeLayers.CheckBoxes = true;
            treeLayers.AfterCheck += treeLayer_AfterCheck;
            TreeNode layerNode = new TreeNode("Polygon from Text");
            layerNode.Checked = true;
            treeLayers.Nodes[0].Nodes.Add(layerNode);
            layerNode.Parent.Checked = true;

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Collapsed
            if (!listLabel.Visible)
            {
                splitContainer1.Panel2Collapsed = false;
                
                listLabel.Visible = true;
                listLabel.Columns.Add("ID");
                listLabel.Columns[0].Width = 100;
                listLabel.Columns.Add("Type");
                listLabel.Columns[1].Width = 100;
                listLabel.Columns.Add("Label");
                listLabel.Columns[2].Width = 100;
                splitContainer3.Panel2Collapsed = false;
                splitContainer3.Panel1Collapsed = true;
            }
            else
            {
                listLabel.Clear();
                listLabel.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                return;

            }

            Shapefile sf = mapControl.get_Shapefile(indexTxt);
            string type = sf.ShapefileType.ToString();
            for (int i = 0; i < sf.NumShapes; i++)
            {
                String[] rowData = new String[listLabel.Columns.Count];
                rowData[0] = i.ToString();
                
                rowData[1] = type;
                rowData[2] = "";
                ListViewItem item = new ListViewItem(rowData);
                listLabel.Items.Add(item);
            }

            

            mapControl.CursorMode = tkCursorMode.cmIdentify;
            
            mapControl.ShapeHighlighted+= MapControl_ShapeLabeling;
            
        }

        private void selectBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            if(!listLabel.Visible)
            {
                MessageBox.Show("No Data!");
                return;
            }
            Shapefile sfTxt = mapControl.get_Shapefile(indexTxt);
            String str = "";
            for (int i = 0; i < sfTxt.NumShapes; i++)
            {
                ListViewItem item = listLabel.Items[i];
                string id = item.SubItems[0].Text;
                string label = item.SubItems[2].Text;
                str += id + "\n";
                str += label + "\n";
                Shape shpTmp = sfTxt.Shape[i];
                for (int j = 0; j < shpTmp.numPoints; j++)
                {
                    MapWinGIS.Point pnt = shpTmp.Point[j];
                    str += (j + 1).ToString() + " " + pnt.x.ToString() + " " + pnt.y.ToString() + "\n";

                }
                str += "\n";
            }

            saveDialog.ShowDialog();
            System.IO.File.WriteAllText(saveDialog.FileName, str);
            MessageBox.Show("Completed!");
        }

        private void combinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBandCombination fCombine = new FormBandCombination(bandIndex);
            fCombine.ShowDialog();
            MapWinGIS.Image img = mapControl.get_Image(0);
            img.RedBandIndex= fCombine.redIndex;
            img.GreenBandIndex = fCombine.greenIndex;
            img.BlueBandIndex = fCombine.blueIndex;
            MessageBox.Show(img.UseRgbBandMapping.ToString());
            MessageBox.Show(img.RedBandIndex.ToString()+ img.GreenBandIndex.ToString() + img.BlueBandIndex.ToString());
            mapControl.Redraw();
        }

        private void polygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygonToolStripMenuItem.Checked = !polygonToolStripMenuItem.Checked;
            if (polygonToolStripMenuItem.Checked)
            {
                mapControl.CursorMode = tkCursorMode.cmMeasure;
                mapControl.Measuring.MeasuringType = tkMeasuringType.MeasureArea;
                mapControl.Measuring.ShowLength = false;
                mapControl.Measuring.ShowBearing = false;
                mapControl.Measuring.ShowTotalLength = false;
                mapControl.Measuring.FillTransparency = 50;
                mapControl.MeasuringChanged += mapControl_MeasuringChanged;
                shp_tmp = new Shapefile();
                if (!shp_tmp.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON))
                {
                    MessageBox.Show("Failed to create shapefile: " + shp_tmp.ErrorMsg[shp_tmp.LastErrorCode]);
                    return;
                }
                currentLayerIndex = mapControl.AddLayer(shp_tmp, true);

            }
            else
            {
                mapControl.CursorMode = tkCursorMode.cmPan;
                if (currentLayerIndex >= 0)
                {
                    mapControl.RemoveLayer(currentLayerIndex);
                }
            }
        }

        private void mapControl_MeasuringChanged(object sender, _DMapEvents_MeasuringChangedEvent e)
        {

            if (e.action == tkMeasuringAction.MesuringStopped)
            {
                Shape shp = new Shape();
                shp.Create(ShpfileType.SHP_POLYGON);
                //shp_tmp.EditAddShape()
                double x, y;
                for (int i = 0; i < mapControl.Measuring.PointCount; i++)
                {
                    if (mapControl.Measuring.get_PointXY(i, out x, out y))
                    {
                        MapWinGIS.Point ptn = new MapWinGIS.Point();
                        ptn.Set(x, y);
                        shp.InsertPoint(ptn, ref i);
                    }
                }
                Shapefile sfTmp = mapControl.get_Shapefile(currentLayerIndex);
                sfTmp.EditAddShape(shp);
                MessageBox.Show(sfTmp.NumShapes.ToString());
                mapControl.Redraw();
            }

        }

        private void saveTool_Click(object sender, EventArgs e)
        {
            Shapefile sfTmp = mapControl.get_Shapefile(currentLayerIndex);
            saveDialog.Filter = "Shape file (*.shp)|*.shp";
            if (saveDialog.ShowDialog()== DialogResult.OK)
            {
                sfTmp.SaveAs(saveDialog.FileName);

                string nameTxt = saveDialog.FileName.Replace(".shp", ".txt");
                string str = "";
                for (int i = 0; i < sfTmp.NumShapes; i++)
                {
                    Shape shpTmp = sfTmp.Shape[i];
                    for (int j = 0; j < shpTmp.numPoints; j++)
                    {
                        string x = shpTmp.Point[j].x.ToString();
                        string y = shpTmp.Point[j].y.ToString();
                        str += j.ToString() + " " + x + " " + y + "\n";
                    }
                    str += "\n";
                }
                System.IO.File.WriteAllText(nameTxt,str);
                MessageBox.Show("Completed!");
            }
        }
    }
}
