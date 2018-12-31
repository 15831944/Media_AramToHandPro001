

using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDXFViewer
{
   public class DXF_Operator_Fun
    {
        public static DxfDocument Test(string file, string output = null)
        {
            // optionally you can save the information to a text file
            bool outputLog = !string.IsNullOrEmpty(output);
            TextWriter writer = null;
            if (outputLog)
            {
                writer = new StreamWriter(File.Create(output));
                Console.SetOut(writer);
            }

            // check if the dxf actually exists
            FileInfo fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
            {
                Console.WriteLine("THE FILE {0} DOES NOT EXIST", file);
                Console.WriteLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }
            bool isBinary;
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);

            // check if the file is a dxf
            if (dxfVersion == DxfVersion.Unknown)
            {
                Console.WriteLine("THE FILE {0} IS NOT A VALID DXF OR THE DXF DOES NOT INCLUDE VERSION INFORMATION IN THE HEADER SECTION", file);
                Console.WriteLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }

            // check if the dxf file version is supported
            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                Console.WriteLine("THE FILE {0} IS NOT A SUPPORTED DXF", file);
                Console.WriteLine();

                Console.WriteLine("FILE VERSION: {0}", dxfVersion);
                Console.WriteLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }

            // DxfDocument dxf = DxfDocument.Load(file, new List<string> { @".\Support" });
            DxfDocument dxf= DxfDocument.Load(file);
            // check if there has been any problems loading the file,
            // this might be the case of a corrupt file or a problem in the library
            if (dxf == null)
            {
                Console.WriteLine("ERROR LOADING {0}", file);
                Console.WriteLine();

                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();

                if (outputLog)
                {
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadLine();
                }
                return null;
            }

            // the dxf has been properly loaded, let's show some information about it
            Console.WriteLine("FILE NAME: {0}", file);
            Console.WriteLine("\tbinary dxf: {0}", isBinary);
            Console.WriteLine();
            Console.WriteLine("FILE VERSION: {0}", dxf.DrawingVariables.AcadVer);
            Console.WriteLine();
            Console.WriteLine("FILE COMMENTS: {0}", dxf.Comments.Count);
            foreach (var o in dxf.Comments)
            {
                Console.WriteLine("\t{0}", o);
            }
            Console.WriteLine();
            Console.WriteLine("FILE TIME:");
            Console.WriteLine("\tdrawing created (UTC): {0}.{1}", dxf.DrawingVariables.TduCreate, dxf.DrawingVariables.TduCreate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing last update (UTC): {0}.{1}", dxf.DrawingVariables.TduUpdate, dxf.DrawingVariables.TduUpdate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing edition time: {0}", dxf.DrawingVariables.TdinDwg);
            Console.WriteLine();
            Console.WriteLine("APPLICATION REGISTRIES: {0}", dxf.ApplicationRegistries.Count);
            foreach (var o in dxf.ApplicationRegistries)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.ApplicationRegistries.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("LAYERS: {0}", dxf.Layers.Count);
            foreach (var o in dxf.Layers)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layers.GetReferences(o).Count);
                Debug.Assert(ReferenceEquals(o.Linetype, dxf.Linetypes[o.Linetype.Name]), "Object reference not equal.");
            }
            Console.WriteLine();

            Console.WriteLine("LINE TYPES: {0}", dxf.Linetypes.Count);
            foreach (var o in dxf.Linetypes)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Linetypes.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("TEXT STYLES: {0}", dxf.TextStyles.Count);
            foreach (var o in dxf.TextStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.TextStyles.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            //Console.WriteLine("SHAPE STYLES: {0}", dxf.ShapeStyles.Count);
            //foreach (var o in dxf.ShapeStyles)
            //{
            //    Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.ShapeStyles.GetReferences(o.Name).Count);
            //}
            //Console.WriteLine();

            Console.WriteLine("DIMENSION STYLES: {0}", dxf.DimensionStyles.Count);
            foreach (var o in dxf.DimensionStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.DimensionStyles.GetReferences(o.Name).Count);
                Debug.Assert(ReferenceEquals(o.TextStyle, dxf.TextStyles[o.TextStyle.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.DimLineLinetype, dxf.Linetypes[o.DimLineLinetype.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.ExtLine1Linetype, dxf.Linetypes[o.ExtLine1Linetype.Name]), "Object reference not equal.");
                Debug.Assert(ReferenceEquals(o.ExtLine2Linetype, dxf.Linetypes[o.ExtLine2Linetype.Name]), "Object reference not equal.");
                if (o.DimArrow1 != null) Debug.Assert(ReferenceEquals(o.DimArrow1, dxf.Blocks[o.DimArrow1.Name]), "Object reference not equal.");
                if (o.DimArrow2 != null) Debug.Assert(ReferenceEquals(o.DimArrow2, dxf.Blocks[o.DimArrow2.Name]), "Object reference not equal.");
            }
            Console.WriteLine();

            Console.WriteLine("MLINE STYLES: {0}", dxf.MlineStyles.Count);
            foreach (var o in dxf.MlineStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.MlineStyles.GetReferences(o.Name).Count);
                foreach (var e in o.Elements)
                {
                    Debug.Assert(ReferenceEquals(e.Linetype, dxf.Linetypes[e.Linetype.Name]), "Object reference not equal.");
                }
            }
            Console.WriteLine();

            Console.WriteLine("UCSs: {0}", dxf.UCSs.Count);
            foreach (var o in dxf.UCSs)
            {
                Console.WriteLine("\t{0}", o.Name);
            }
            Console.WriteLine();

            Console.WriteLine("BLOCKS: {0}", dxf.Blocks.Count);
            foreach (var o in dxf.Blocks)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Blocks.GetReferences(o.Name).Count);
                Debug.Assert(ReferenceEquals(o.Layer, dxf.Layers[o.Layer.Name]), "Object reference not equal.");

                foreach (var e in o.Entities)
                {
                    Debug.Assert(ReferenceEquals(e.Layer, dxf.Layers[e.Layer.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(e.Linetype, dxf.Linetypes[e.Linetype.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(e.Owner, dxf.Blocks[o.Name]), "Object reference not equal.");
                    foreach (var x in e.XData.Values)
                    {
                        Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                    }

                    Text txt = e as Text;
                    if (txt != null) Debug.Assert(ReferenceEquals(txt.Style, dxf.TextStyles[txt.Style.Name]), "Object reference not equal.");

                    MText mtxt = e as MText;
                    if (mtxt != null) Debug.Assert(ReferenceEquals(mtxt.Style, dxf.TextStyles[mtxt.Style.Name]), "Object reference not equal.");

                    Dimension dim = e as Dimension;
                    if (dim != null)
                    {
                        Debug.Assert(ReferenceEquals(dim.Style, dxf.DimensionStyles[dim.Style.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(dim.Block, dxf.Blocks[dim.Block.Name]), "Object reference not equal.");
                    }

                    MLine mline = e as MLine;
                    if (mline != null) Debug.Assert(ReferenceEquals(mline.Style, dxf.MlineStyles[mline.Style.Name]), "Object reference not equal.");

                    Image img = e as Image;
                    if (img != null) Debug.Assert(ReferenceEquals(img.Definition, dxf.ImageDefinitions[img.Definition.Name]), "Object reference not equal.");

                    Insert ins = e as Insert;
                    if (ins != null)
                    {
                        Debug.Assert(ReferenceEquals(ins.Block, dxf.Blocks[ins.Block.Name]), "Object reference not equal.");
                        foreach (var a in ins.Attributes)
                        {
                            Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Linetype, dxf.Linetypes[a.Linetype.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Style, dxf.TextStyles[a.Style.Name]), "Object reference not equal.");
                        }
                    }
                }

                foreach (var a in o.AttributeDefinitions.Values)
                {
                    Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                    Debug.Assert(ReferenceEquals(a.Linetype, dxf.Linetypes[a.Linetype.Name]), "Object reference not equal.");
                    foreach (var x in a.XData.Values)
                    {
                        Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("LAYOUTS: {0}", dxf.Layouts.Count);
            foreach (var o in dxf.Layouts)
            {
                Debug.Assert(ReferenceEquals(o.AssociatedBlock, dxf.Blocks[o.AssociatedBlock.Name]), "Object reference not equal.");

                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layouts.GetReferences(o.Name).Count);
                List<DxfObject> entities = dxf.Layouts.GetReferences(o.Name);
                foreach (var e in entities)
                {
                    EntityObject entity = e as EntityObject;
                    if (entity != null)
                    {
                        Debug.Assert(ReferenceEquals(entity.Layer, dxf.Layers[entity.Layer.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(entity.Linetype, dxf.Linetypes[entity.Linetype.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(entity.Owner, dxf.Blocks[o.AssociatedBlock.Name]), "Object reference not equal.");
                        foreach (var x in entity.XData.Values)
                        {
                            Debug.Assert(ReferenceEquals(x.ApplicationRegistry, dxf.ApplicationRegistries[x.ApplicationRegistry.Name]), "Object reference not equal.");
                        }
                    }

                    Text txt = e as Text;
                    if (txt != null) Debug.Assert(ReferenceEquals(txt.Style, dxf.TextStyles[txt.Style.Name]), "Object reference not equal.");

                    MText mtxt = e as MText;
                    if (mtxt != null) Debug.Assert(ReferenceEquals(mtxt.Style, dxf.TextStyles[mtxt.Style.Name]), "Object reference not equal.");

                    Dimension dim = e as Dimension;
                    if (dim != null)
                    {
                        Debug.Assert(ReferenceEquals(dim.Style, dxf.DimensionStyles[dim.Style.Name]), "Object reference not equal.");
                        Debug.Assert(ReferenceEquals(dim.Block, dxf.Blocks[dim.Block.Name]), "Object reference not equal.");
                    }

                    MLine mline = e as MLine;
                    if (mline != null) Debug.Assert(ReferenceEquals(mline.Style, dxf.MlineStyles[mline.Style.Name]), "Object reference not equal.");

                    Image img = e as Image;
                    if (img != null) Debug.Assert(ReferenceEquals(img.Definition, dxf.ImageDefinitions[img.Definition.Name]), "Object reference not equal.");

                    Insert ins = e as Insert;
                    if (ins != null)
                    {
                        Debug.Assert(ReferenceEquals(ins.Block, dxf.Blocks[ins.Block.Name]), "Object reference not equal.");
                        foreach (var a in ins.Attributes)
                        {
                            Debug.Assert(ReferenceEquals(a.Layer, dxf.Layers[a.Layer.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Linetype, dxf.Linetypes[a.Linetype.Name]), "Object reference not equal.");
                            Debug.Assert(ReferenceEquals(a.Style, dxf.TextStyles[a.Style.Name]), "Object reference not equal.");
                        }
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("IMAGE DEFINITIONS: {0}", dxf.ImageDefinitions.Count);
            foreach (var o in dxf.ImageDefinitions)
            {
             
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.ImageDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DGN UNDERLAY DEFINITIONS: {0}", dxf.UnderlayDgnDefinitions.Count);
            foreach (var o in dxf.UnderlayDgnDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayDgnDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DWF UNDERLAY DEFINITIONS: {0}", dxf.UnderlayDwfDefinitions.Count);
            foreach (var o in dxf.UnderlayDwfDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayDwfDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("PDF UNDERLAY DEFINITIONS: {0}", dxf.UnderlayPdfDefinitions.Count);
            foreach (var o in dxf.UnderlayPdfDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.UnderlayPdfDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("GROUPS: {0}", dxf.Groups.Count);
            foreach (var o in dxf.Groups)
            {
                Console.WriteLine("\t{0}; Entities count: {1}", o.Name, o.Entities.Count);
            }
            Console.WriteLine();

            Console.WriteLine("ATTRIBUTE DEFINITIONS for the \"Model\" Layout: {0}", dxf.Layouts[Layout.ModelSpaceName].AssociatedBlock.AttributeDefinitions.Count);
            foreach (var o in dxf.Layouts[Layout.ModelSpaceName].AssociatedBlock.AttributeDefinitions)
            {
                Console.WriteLine("\tTag: {0}", o.Value.Tag);
            }
            Console.WriteLine();

            // the entities lists contain the geometry that has a graphical representation in the drawing across all layouts,
            // to get the entities that belongs to a specific layout you can get the references through the Layouts.GetReferences(name)
            // or check the EntityObject.Owner.Record.Layout property
            Console.WriteLine("ENTITIES for the Active Layout = {0}:", dxf.ActiveLayout);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Arc, dxf.Arcs.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Circle, dxf.Circles.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Dimension, dxf.Dimensions.Count());
            foreach (var a in dxf.Dimensions)
            {
                foreach (var styleOverride in a.StyleOverrides.Values)
                {
                    switch (styleOverride.Type)
                    {
                        case DimensionStyleOverrideType.DimLineLinetype:
                            Debug.Assert(ReferenceEquals((Linetype)styleOverride.Value, dxf.Linetypes[((Linetype)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.ExtLine1Linetype:
                            Debug.Assert(ReferenceEquals((Linetype)styleOverride.Value, dxf.Linetypes[((Linetype)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.ExtLine2Linetype:
                            Debug.Assert(ReferenceEquals((Linetype)styleOverride.Value, dxf.Linetypes[((Linetype)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.TextStyle:
                            Debug.Assert(ReferenceEquals((TextStyle)styleOverride.Value, dxf.TextStyles[((TextStyle)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.LeaderArrow:
                            Debug.Assert(ReferenceEquals((Block)styleOverride.Value, dxf.Blocks[((Block)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.DimArrow1:
                            if (styleOverride.Value == null) break;
                            Debug.Assert(ReferenceEquals((Block)styleOverride.Value, dxf.Blocks[((Block)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.DimArrow2:
                            if (styleOverride.Value == null) break;
                            Debug.Assert(ReferenceEquals((Block)styleOverride.Value, dxf.Blocks[((Block)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                    }
                }
            }
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ellipse, dxf.Ellipses.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Face3D, dxf.Faces3d.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Hatch, dxf.Hatches.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Image, dxf.Images.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Insert, dxf.Inserts.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Leader, dxf.Leaders.Count());
            foreach (var a in dxf.Leaders)
            {
                foreach (var styleOverride in a.StyleOverrides.Values)
                {
                    switch (styleOverride.Type)
                    {
                        case DimensionStyleOverrideType.DimLineLinetype:
                            Debug.Assert(ReferenceEquals((Linetype)styleOverride.Value, dxf.Linetypes[((Linetype)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.ExtLine1Linetype:
                            Debug.Assert(ReferenceEquals((Linetype)styleOverride.Value, dxf.Linetypes[((Linetype)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.ExtLine2Linetype:
                            Debug.Assert(ReferenceEquals((Linetype)styleOverride.Value, dxf.Linetypes[((Linetype)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.TextStyle:
                            Debug.Assert(ReferenceEquals((TextStyle)styleOverride.Value, dxf.TextStyles[((TextStyle)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.LeaderArrow:
                            Debug.Assert(ReferenceEquals((Block)styleOverride.Value, dxf.Blocks[((Block)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.DimArrow1:
                            Debug.Assert(ReferenceEquals((Block)styleOverride.Value, dxf.Blocks[((Block)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                        case DimensionStyleOverrideType.DimArrow2:
                            Debug.Assert(ReferenceEquals((Block)styleOverride.Value, dxf.Blocks[((Block)styleOverride.Value).Name]), "Object reference not equal.");
                            break;
                    }
                }
            }
            Console.WriteLine("\t{0}; count: {1}", EntityType.LightWeightPolyline, dxf.LwPolylines.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Line, dxf.Lines.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Mesh, dxf.Meshes.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.MLine, dxf.MLines.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.MText, dxf.MTexts.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Point, dxf.Points.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.PolyfaceMesh, dxf.PolyfaceMeshes.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Polyline, dxf.Polylines.Count());
           // Console.WriteLine("\t{0}; count: {1}", EntityType.Shape, dxf.Shapes.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Solid, dxf.Solids.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Spline, dxf.Splines.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Text, dxf.Texts.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ray, dxf.Rays.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Underlay, dxf.Underlays.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Viewport, dxf.Viewports.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.Wipeout, dxf.Wipeouts.Count());
            Console.WriteLine("\t{0}; count: {1}", EntityType.XLine, dxf.XLines.Count());
            Console.WriteLine();

            // the dxf version is controlled by the DrawingVariables property of the dxf document,
            // also a HeaderVariables instance or a DxfVersion can be passed to the constructor to initialize a new DxfDocument.
          //  dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2018;
          //  dxf.Save("sample 2018.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2013;
            dxf.Save("sample 2013.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("sample 2010.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("sample 2007.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("sample 2004.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("sample 2000.dxf");

            // saving to binary dxf
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2013;
            dxf.Save("binary test.dxf", true);
            // DxfDocument test = DxfDocument.Load("binary test.dxf", new List<string> { @".\Support" });
            DxfDocument test = DxfDocument.Load("binary test.dxf");
            if (outputLog)
            {
                writer.Flush();
                writer.Close();
            }
            else
            {
                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();
            }
            return dxf;
        }
    }
}
