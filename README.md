# Jayhuekim Utilities

This repository contains utilities and samples for Autodesk Revit.

## ExportFamilyDWG Add-in

`ExportFamilyDWG` is a Revit 2024 add-in that lets you export loaded families as 3D DWG files. For each family, a folder selection window allows you to choose where the DWG will be saved. The file is saved with the family name as the filename (e.g. `FamilyName.dwg`).

To build the add-in, open `ExportFamilyDWG.csproj` in Visual Studio with the Revit 2024 API assemblies referenced in the provided paths. Copy `ExportFamilyDWG.addin` and the compiled DLL to the Revit 2024 add-ins folder.
