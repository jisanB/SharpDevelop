﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class WixCompilerExtensionBinding : ConfigurationGuiBinding
	{
		WixCompilerExtensionPicker extensionPicker;
		
		enum WixExtensionType {
			Compiler,
			Linker,
			Library
		}
		
		public WixCompilerExtensionBinding(WixCompilerExtensionPicker extensionPicker)
		{
			this.extensionPicker = extensionPicker;
		}
		
		public override bool Save()
		{
			RemoveExistingProjectExtensions();
			
			IProject project = Project;
			WixCompilerExtensionName[] extensions = extensionPicker.GetExtensions();
			foreach (WixCompilerExtensionName extension in extensions) {
				if (extension.AssemblyName.Length > 0) {
					ProjectService.AddProjectItem(project, CreateProjectItem(extension));
				} else {
					MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.WixBinding.ExtensionBinding.InvalidExtension}"));
					return false;
				}
			}
			return true;
		}
		
		public override void Load()
		{
			ReadOnlyCollection<WixExtensionProjectItem> extensions = GetExtensions();
			foreach (WixExtensionProjectItem extension in extensions) {
				extensionPicker.AddExtension(extension.QualifiedName);
			}
		}
		
		WixExtensionProjectItem CreateProjectItem(WixCompilerExtensionName extension)
		{
			WixExtensionProjectItem projectItem;
			switch (ExtensionType) {
				case WixExtensionType.Compiler:
					projectItem = new WixCompilerExtensionProjectItem(Project);
					break;
				case WixExtensionType.Library:
					projectItem = new WixLibraryExtensionProjectItem(Project);
					break;
				default:
					projectItem = new WixLinkerExtensionProjectItem(Project);
					break;
			}
			projectItem.Include = extension.AssemblyName;
			projectItem.ClassName = extension.ClassName;
			return projectItem;
		}
		
		ReadOnlyCollection<WixExtensionProjectItem> GetExtensions()
		{
			switch (ExtensionType) {
				case WixExtensionType.Compiler:
					return WixProject.WixCompilerExtensions;
				case WixExtensionType.Library:
					return WixProject.WixLibraryExtensions;
				default:
					return WixProject.WixLinkerExtensions;
			}
		}
		
		WixProject WixProject {
			get {
				return (WixProject)Project;
			}
		}
		
		WixExtensionType ExtensionType {
			get {
				switch (Property) {
					case "CompileExtension":
						return WixExtensionType.Compiler;
					case "LibExtension":
						return WixExtensionType.Library;
					case "LinkExtension":
						return WixExtensionType.Linker;
					default:
						throw new ApplicationException("Unknown WiX extension type: " + Property);
				}
			}
		}
		
		void RemoveExistingProjectExtensions()
		{
			IProject project = Project;
			foreach (ProjectItem item in GetExtensions()) {
				ProjectService.RemoveProjectItem(project, item);
			}
		}
	}
}
