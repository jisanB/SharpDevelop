﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	public class NuGetPackageViewModel : AddInPackageViewModelBase
	{
		private IPackage _package;
		private IEnumerable<PackageOperation> _packageOperations = new PackageOperation[0];
		
		public NuGetPackageViewModel(IPackage package)
			: base()
		{
			this._package = package;
		}
		
		public NuGetPackageViewModel(IAddInManagerServices services, IPackage package)
			: base(services)
		{
			this._package = package;
		}

		public IPackage Package
		{
			get
			{
				return _package;
			}
		}
		
		public override string Name
		{
			get
			{
				return _package.Id;
			}
		}

		public override Uri LicenseUrl
		{
			get
			{
				return _package.LicenseUrl;
			}
		}

		public override Uri ProjectUrl
		{
			get
			{
				return _package.ProjectUrl;
			}
		}

		public override Uri ReportAbuseUrl
		{
			get
			{
				return _package.ReportAbuseUrl;
			}
		}
		
		public override bool IsOffline
		{
			get
			{
				return false;
			}
		}
		
		public override bool IsPreinstalled
		{
			get
			{
				return false;
			}
		}

		public override bool IsAdded
		{
			get
			{
				AddIn installedAddIn = AddInManager.Setup.GetAddInForNuGetPackage(_package, true);
				
//				if (installedAddIn != null)
//					LoggingService.DebugFormatted("isAdded: installedAddIn.Action = {0}", installedAddIn.Action);
//				else
//					LoggingService.DebugFormatted("isAdded: installedAddIn for {0} is null", _package.Id);
				
				return (installedAddIn != null) && ((installedAddIn.Action == AddInAction.Install) || (installedAddIn.Action == AddInAction.Update));
			}
		}
		
		public override bool IsUpdate
		{
			get
			{
				AddIn installedAddIn = AddInManager.Setup.GetAddInForNuGetPackage(_package);
				
//				if (installedAddIn != null)
//					LoggingService.DebugFormatted("isUpdate: installed {0}, package {1}", installedAddIn.Version.ToString(), _package.Version.Version.ToString());
//				else
//					LoggingService.DebugFormatted("isUpdate: installedAddIn for {0} is null", _package.Id);
				
				return (installedAddIn != null)
					&& AddInManager.Setup.IsAddInInstalled(installedAddIn)
					&& (installedAddIn.Version < _package.Version.Version);
			}
		}
		
		public override bool IsInstalled
		{
			get
			{
				AddIn installedAddIn = AddInManager.Setup.GetAddInForNuGetPackage(_package);
				return (installedAddIn != null) && AddInManager.Setup.IsAddInInstalled(installedAddIn);
			}
		}
		
		public override bool IsInstallable
		{
			get
			{
				return true;
			}
		}
		
		public override bool IsUninstallable
		{
			get
			{
				return true;
			}
		}
		
		public override bool IsDisablingPossible
		{
			get
			{
				return false;
			}
		}
		
		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}
		
		public override bool IsRemoved
		{
			get
			{
				AddIn installedAddIn = AddInManager.Setup.GetAddInForNuGetPackage(_package);
				return (installedAddIn != null) && (installedAddIn.Action == AddInAction.Uninstall);
			}
		}

		private bool IsPackageInstalled()
		{
			return AddInManager.NuGet.Packages.LocalRepository.Exists(_package);
		}

		public override IEnumerable<AddInDependency> Dependencies
		{
			get
			{
				if ((_package.DependencySets != null) && _package.DependencySets.Any())
				{
					PackageDependencySet firstSet = _package.DependencySets.First();
					if ((firstSet != null) && (firstSet.Dependencies != null) && (firstSet.Dependencies.Count > 0))
					{
						return firstSet.Dependencies.Select(d => new AddInDependency(d));
					}
				}
				return null;
			}
		}

		public override IEnumerable<string> Authors
		{
			get
			{
				return _package.Authors;
			}
		}

		public override bool HasDownloadCount
		{
			get
			{
				return _package.DownloadCount >= 0;
			}
		}

		public override string Id
		{
			get
			{
				return _package.Id;
			}
		}

		public override Uri IconUrl
		{
			get
			{
				return _package.IconUrl;
			}
		}

		public override string Summary
		{
			get
			{
				if (IsAdded)
				{
					if (IsUpdate)
					{
						return SD.ResourceService.GetString("AddInManager.AddInUpdated");
					}
					else
					{
						return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInInstalled"));
					}
				}
				else if (IsRemoved)
				{
					return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInRemoved"));
				}
				else if (!IsEnabled)
				{
					return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInDisabled"));
				}
				else
				{
					return _package.Summary;
				}
			}
		}

		public override Version Version
		{
			get
			{
				return _package.Version.Version;
			}
		}

		public override int DownloadCount
		{
			get
			{
				return _package.DownloadCount;
			}
		}

		public override string Description
		{
			get
			{
				return _package.Description;
			}
		}

		public override DateTime? LastUpdated
		{
			get
			{
				// TODO
//				return package.LastUpdated;
				return null;
			}
		}
		
		public override bool HasDependencyConflicts
		{
			get
			{
				return false;
			}
		}

		public override void AddPackage()
		{
			ClearReportedMessages();
			TryInstallingPackage();
		}
		
		public override void UpdatePackage()
		{
			ClearReportedMessages();
			TryInstallingPackage();
		}

		private void ClearReportedMessages()
		{
			//			packageManagementEvents.OnPackageOperationsStarting();
		}

		private void GetPackageOperations()
		{
			var packageOperationResolver = AddInManager.NuGet.CreateInstallPackageOperationResolver(false);
			_packageOperations = packageOperationResolver.ResolveOperations(_package);
		}

		private bool CanInstallPackage()
		{
			// Ask for downloading dependent packages
			if ((_packageOperations != null) && _packageOperations.Any())
			{
				var operationsForDependencies = _packageOperations.Where(p => p.Package.Id != _package.Id);
				if ((operationsForDependencies != null) && operationsForDependencies.Any())
				{
					string addInNames = "";
					foreach (var packageOperation in operationsForDependencies)
					{
						addInNames += "\t " +
							packageOperation.Package.Id + " " + packageOperation.Package.Version.ToString() + Environment.NewLine;
					}
					if (!MessageService.AskQuestionFormatted(
						"${res:AddInManager.Title}", "${res:AddInManager2.InstallDependentMessage}", _package.Id, addInNames))
					{
						return false;
					}
				}
			}
			
			// Ask for license acceptance
			IEnumerable<IPackage> packages = GetPackagesRequiringLicenseAcceptance();
			if (packages.Any())
			{
				AcceptLicensesEventArgs acceptLicenses = new AcceptLicensesEventArgs(packages);
				acceptLicenses.IsAccepted = true;
				AddInManager.Events.OnAcceptLicenses(acceptLicenses);
				return acceptLicenses.IsAccepted;
			}
			return true;
		}

		private IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance()
		{
			IList<IPackage> packagesToBeInstalled = GetPackagesToBeInstalled();
			return GetPackagesRequiringLicenseAcceptance(packagesToBeInstalled);
		}

		private IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IList<IPackage> packagesToBeInstalled)
		{
			return packagesToBeInstalled.Where(package => PackageRequiresLicenseAcceptance(package));
		}

		private IList<IPackage> GetPackagesToBeInstalled()
		{
			List<IPackage> packages = new List<IPackage>();
			foreach (PackageOperation operation in _packageOperations)
			{
				if (operation.Action == PackageAction.Install)
				{
					packages.Add(operation.Package);
				}
			}
			return packages;
		}

		private bool PackageRequiresLicenseAcceptance(IPackage package)
		{
			return package.RequireLicenseAcceptance && !IsPackageInstalled();
		}

		private void TryInstallingPackage()
		{
			try
			{
				if (IsPackageInstalled())
				{
					// Package is already installed, but seems to be not registered as SD AddIn
					string assumedPackageOutputDir =
						Path.Combine(AddInManager.NuGet.PackageOutputDirectory, _package.Id + "." + _package.Version.Version.ToString());
					AddInManager.Setup.InstallAddIn(_package, assumedPackageOutputDir);
				}
				else
				{
					// Perform a normal download and AddIn installation
					GetPackageOperations();
					if (CanInstallPackage())
					{
						InstallPackage(_packageOperations);
					}
				}
			}
			catch (Exception ex)
			{
				ReportError(ex);
				LoggingService.Error("Error when trying to install package.", ex);
			}
		}

		private void InstallPackage(IEnumerable<PackageOperation> packageOperations)
		{
			foreach (PackageOperation operation in packageOperations)
			{
				AddInManager.NuGet.ExecuteOperation(operation);
			}
		}

		private void ReportError(Exception ex)
		{
			AddInManager.Events.OnAddInOperationError(new AddInExceptionEventArgs(ex));
		}

		public override void RemovePackage()
		{
			TryUninstallingPackage();
		}

		private void TryUninstallingPackage()
		{
			try
			{
				UninstallPackage();
			}
			catch (Exception ex)
			{
				ReportError(ex);
				LoggingService.Error("Error when trying to uninstall package.", ex);
			}
		}
		
		public void UninstallPackage()
		{
			ClearReportedMessages();
			
			AddIn installedAddIn = AddInManager.Setup.GetAddInForNuGetPackage(_package);
			if (installedAddIn != null)
			{
				AddInManager.Setup.UninstallAddIn(installedAddIn);
			}
		}
		
		public override void CancelInstallation()
		{
			AddIn addIn = AddInManager.Setup.GetAddInForNuGetPackage(_package, true);
			if (addIn != null)
			{
				AddInManager.Setup.CancelInstallation(addIn);
			}
		}
		
		public override void CancelUpdate()
		{
			AddIn addIn = AddInManager.Setup.GetAddInForNuGetPackage(_package, true);
			if (addIn != null)
			{
				AddInManager.Setup.CancelUpdate(addIn);
			}
		}
		
		public override void CancelUninstallation()
		{
			AddIn addIn = AddInManager.Setup.GetAddInForNuGetPackage(_package);
			if (addIn != null)
			{
				AddInManager.Setup.CancelUninstallation(addIn);
			}
		}
	}
}