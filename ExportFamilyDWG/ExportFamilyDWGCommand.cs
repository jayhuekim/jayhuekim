using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;

namespace ExportFamilyDWG
{
    [Transaction(TransactionMode.Manual)]
    public class ExportFamilyDWGCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Collect all loaded families in the document
            IList<Family> families = new FilteredElementCollector(doc)
                                        .OfClass(typeof(Family))
                                        .Cast<Family>()
                                        .ToList();

            if(families.Count == 0)
            {
                TaskDialog.Show("Export", "No families found in the document.");
                return Result.Cancelled;
            }

            foreach (Family fam in families)
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.Description = $"Select folder to save {fam.Name}.dwg";
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        continue;
                    }

                    string folderPath = dialog.SelectedPath;
                    string dwgPath = Path.Combine(folderPath, fam.Name + ".dwg");

                    // Open family document for editing
                    Document famDoc = doc.EditFamily(fam);

                    // Find a 3D view to export
                    View3D view3D = new FilteredElementCollector(famDoc)
                                        .OfClass(typeof(View3D))
                                        .Cast<View3D>()
                                        .FirstOrDefault(v => !v.IsTemplate);
                    if (view3D == null)
                    {
                        TaskDialog.Show("Export", $"No 3D view found for family {fam.Name}.");
                        famDoc.Close(false);
                        continue;
                    }

                    IList<ElementId> views = new List<ElementId> { view3D.Id };

                    DWGExportOptions options = new DWGExportOptions
                    {
                        FileVersion = ACADVersion.R2018, // Revit 2024 default dwg version
                        ExportOfSolids = SolidGeometry.ACIS
                    };

                    bool success = famDoc.Export(folderPath, fam.Name + ".dwg", views, options);
                    famDoc.Close(false);

                    if(!success)
                    {
                        TaskDialog.Show("Export", $"Failed to export {fam.Name}.");
                    }
                }
            }

            return Result.Succeeded;
        }
    }
}
