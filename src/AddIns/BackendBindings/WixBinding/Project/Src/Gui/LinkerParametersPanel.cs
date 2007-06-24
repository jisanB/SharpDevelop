﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.WixBinding
{
	public class LinkerParametersPanel : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.WixBinding.Resources.LinkerParametersPanel.xfrm"));
			InitializeHelper();
			
			// Add the extension picker in manually since the anchoring does not
			// work if we add the picker into the XML of the LinkerParametersPanel.xfrm file.
			WixCompilerExtensionPicker extensionPicker = new WixCompilerExtensionPicker();
			extensionPicker.Dock = DockStyle.Fill;
			ControlDictionary["compilerExtensionsGroupBox"].Controls.Add(extensionPicker);
			
			WixCompilerExtensionBinding b = new WixCompilerExtensionBinding(extensionPicker);
			helper.AddBinding("LinkExtension", b);
			extensionPicker.ExtensionsChanged += CompilerExtensionsChanged;
		
			helper.AddConfigurationSelector(this);
		}
		
		void CompilerExtensionsChanged(object source, EventArgs e)
		{
			IsDirty = true;
		}
	}
}
