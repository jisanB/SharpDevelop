﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass2 : CodeClass
	{
		public CodeClass2(IProjectContent projectContent, IClass c)
			: base(projectContent, c)
		{
		}
		
		public CodeElements PartialClasses {
			get { return new PartialClasses(this); }
		}
		
		public static CodeClass2 CreateFromBaseType(IProjectContent projectContent, IReturnType baseType)
		{
			IClass baseTypeClass = baseType.GetUnderlyingClass();
			return new CodeClass2(projectContent, baseTypeClass);
		}
		
		public bool IsGeneric {
			get { throw new NotImplementedException(); }
		}
		
		public vsCMClassKind ClassKind {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		public bool IsAbstract {
			get { throw new NotImplementedException(); }
		}
	}
}