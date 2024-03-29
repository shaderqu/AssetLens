﻿using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetLens.UI
{
	using Reference;
	
	[CustomEditor(typeof(Setting))]
	internal sealed class AssetLensSettingInspector : AssetLensInspector
	{
		private VisualElement options;
		private VisualElement buttons;
		
		private Toggle enabled;
#if UNITY_2021_1_OR_NEWER
		private DropdownField localization;
#else
		private TextField localization;
#endif

		#region Options Field

		private Label globalOptionHeader;
		private Label indexOptionHeader;
		private Label viewOptionHeader;
		private Label inspectorOptionHeader;
		private Label LogOptionHeader;

		private VisualElement IndexGroup;
		private Toggle IndexByGuidRegEx;
		private Toggle IndexSceneObject;
		private Toggle IndexPackageSubDir;
		
		private VisualElement ViewGroup;
		private Toggle ViewSceneObject;
		private Toggle ViewInPlayMode;
		private Toggle ViewIndexerVersion;
		private EnumField ViewObjectFocusMethod;
		private Toggle ViewRefreshOnUpdate;
		// private EnumField ViewRefreshRate;
		
		private VisualElement InspectorGroup;
		private Toggle InspectorLensEnable;
		private Toggle InspectorHideWithNoLink;
		private Toggle InspectorDrawObjectInstanceId;
		
		private VisualElement LogGroup;
		private Toggle LogReferenceAdd;
		private Toggle LogReferenceRemove;
		
		#endregion

		#region Buttons Field

		private Button openIndexWizard;
		private Button resetSetting;
		private Button cleanupCache;
		private Button uninstallPackage;

		#endregion

		#region Help Field
		
		private Button openReadme;
		private Button documentation;
		private Button reportIssue;
		private Button changelog;
		private Button license;
		private Button credit;

		#endregion
		
		/// <summary>
		/// 뭔가 하고 있다!
		/// </summary>
		private bool working = false;
		private bool init = false;
		
		protected override void Constructor()
		{
			LoadLayout("SettingInspector");
			
			LoadStylesheet("SettingInspector");
			
			root.AddSquareRoundButton();
			root.AddHeader();
			root.AddSwitchToggle();
			
			options = root.Q<VisualElement>("options");
			buttons = root.Q<VisualElement>("buttons");

			IndexGroup = new VisualElement();
			ViewGroup = new VisualElement();
			InspectorGroup = new VisualElement();
			LogGroup = new VisualElement();
			
			IndexGroup.AddToClassList("group");
			ViewGroup.AddToClassList("group");
			InspectorGroup.AddToClassList("group");
			LogGroup.AddToClassList("group");

			enabled = new Toggle();
#if UNITY_2021_1_OR_NEWER
			localization = new DropdownField();
#else
			localization = new TextField();
#endif

			InitOptions();
			InitButtons();
			InitHelp();
			
			AddCallbacks();

			init = true;
			
			/*
			* localized text
			*/

			RefreshLocalizedText();

			// setting은 인스펙터에서만 조절가능하므로,
			Setting.CanSetDirty = true;
			Setting.SetSettingDirty();

			AssetLensConsole.Log(R.D("We can execute Setting.onSettingChange from now on."));
		}

		private void InitOptions()
		{
			globalOptionHeader = new Label();
			indexOptionHeader = new Label();
			viewOptionHeader = new Label();
			inspectorOptionHeader = new Label();
			LogOptionHeader = new Label();

			// indexOptionHeader.value = true;
			// viewOptionHeader.value = true;
			// inspectorOptionHeader.value = true;

			IndexByGuidRegEx = new Toggle();
			IndexSceneObject = new Toggle();
			IndexPackageSubDir = new Toggle();
			
			ViewSceneObject = new Toggle();
			ViewInPlayMode = new Toggle();
			ViewIndexerVersion = new Toggle();
			ViewObjectFocusMethod = new EnumField();
			ViewRefreshOnUpdate = new Toggle();
			// ViewRefreshRate = new EnumField();
			
			InspectorLensEnable = new Toggle();
			InspectorHideWithNoLink = new Toggle();
			InspectorDrawObjectInstanceId = new Toggle();

			LogReferenceAdd = new Toggle();
			LogReferenceRemove = new Toggle();
			
			/*
 * Binding Path
 */

			enabled.bindingPath = nameof(enabled);
			localization.bindingPath = nameof(localization);
			
			IndexByGuidRegEx.bindingPath = nameof(IndexByGuidRegEx);
			IndexSceneObject.bindingPath = nameof(IndexSceneObject);
			IndexPackageSubDir.bindingPath = nameof(IndexPackageSubDir);
			
			ViewSceneObject.bindingPath = nameof(ViewSceneObject);
			ViewInPlayMode.bindingPath = nameof(ViewInPlayMode);
			ViewIndexerVersion.bindingPath = nameof(ViewIndexerVersion);
			ViewObjectFocusMethod.bindingPath = nameof(ViewObjectFocusMethod);
			ViewRefreshOnUpdate.bindingPath = nameof(ViewRefreshOnUpdate);
			// ViewRefreshRate.bindingPath = nameof(ViewRefreshRate);
			
			InspectorLensEnable.bindingPath = nameof(InspectorLensEnable);
			InspectorHideWithNoLink.bindingPath = nameof(InspectorHideWithNoLink);
			InspectorDrawObjectInstanceId.bindingPath = nameof(InspectorDrawObjectInstanceId);
			
			LogReferenceAdd.bindingPath = nameof(LogReferenceAdd);
			LogReferenceRemove.bindingPath = nameof(LogReferenceRemove);
			
			/*
			 * Bind
			 */
			enabled.Bind(serializedObject);
			localization.Bind(serializedObject);
			
			IndexByGuidRegEx.Bind(serializedObject);
			IndexSceneObject.Bind(serializedObject);
			IndexPackageSubDir.Bind(serializedObject);
			
			ViewSceneObject.Bind(serializedObject);
			ViewInPlayMode.Bind(serializedObject);
			ViewIndexerVersion.Bind(serializedObject);
			ViewObjectFocusMethod.Bind(serializedObject);
			ViewRefreshOnUpdate.Bind(serializedObject);
			// ViewRefreshRate.Bind(serializedObject);
			
			InspectorLensEnable.Bind(serializedObject);
			InspectorHideWithNoLink.Bind(serializedObject);
			InspectorDrawObjectInstanceId.Bind(serializedObject);
			
			LogReferenceAdd.Bind(serializedObject);
			LogReferenceRemove.Bind(serializedObject);
			

			
			/*
			 * Add
			 */
			options.Add(globalOptionHeader);
			options.Add(enabled);
			options.Add(localization);
			
			options.Add(indexOptionHeader);
			options.Add(IndexGroup);
			
			IndexGroup.Add(IndexByGuidRegEx);
			IndexGroup.Add(IndexSceneObject);
			IndexGroup.Add(IndexPackageSubDir);
			
			options.Add(viewOptionHeader);
			options.Add(ViewGroup);
			
			ViewGroup.Add(ViewSceneObject);
			ViewGroup.Add(ViewInPlayMode);
			ViewGroup.Add(ViewIndexerVersion);
			ViewGroup.Add(ViewObjectFocusMethod);
			ViewGroup.Add(ViewRefreshOnUpdate);
			// ViewGroup.Add(ViewRefreshRate);
			
			options.Add(inspectorOptionHeader);
			options.Add(InspectorGroup);
			
			InspectorGroup.Add(InspectorLensEnable);
			InspectorGroup.Add(InspectorHideWithNoLink);
			InspectorGroup.Add(InspectorDrawObjectInstanceId);
			
			options.Add(LogOptionHeader);
			options.Add(LogGroup);
			
			LogGroup.Add(LogReferenceAdd);
			LogGroup.Add(LogReferenceRemove);
			
			globalOptionHeader.AddToClassList("header-1");
			indexOptionHeader.AddToClassList("header-2");
			viewOptionHeader.AddToClassList("header-2");
			inspectorOptionHeader.AddToClassList("header-2");
			LogOptionHeader.AddToClassList("header-2");

			/*
			 * Init
			 */
			// localization.choices = Setting.GetLanguageChoices();
			// localization.SetChoices(Setting.GetLanguageChoices());

#if UNITY_2021_1_OR_NEWER
			localization.SetChoices(Setting.GetLanguageChoices());
#endif

			/*
			 * Callbacks
			 */

			localization.RegisterValueChangedCallback(OnLanguageChange);

			// indexOptionHeader.RegisterValueChangedCallback(OnIndexOptionHeaderChange);
			// viewOptionHeader.RegisterValueChangedCallback(OnViewOptionHeaderChange);
			// inspectorOptionHeader.RegisterValueChangedCallback(OnInspectorOptionHeaderChange);
		}

		private void AddCallbacks()
		{
			enabled.RegisterValueChangedCallback(SetSettingDirty);
			
			IndexByGuidRegEx.RegisterValueChangedCallback(SetSettingDirty);
			IndexSceneObject.RegisterValueChangedCallback(SetSettingDirty);
			IndexPackageSubDir.RegisterValueChangedCallback(SetSettingDirty);
			
			ViewSceneObject.RegisterValueChangedCallback(SetSettingDirty);
			ViewInPlayMode.RegisterValueChangedCallback(SetSettingDirty);
			ViewIndexerVersion.RegisterValueChangedCallback(SetSettingDirty);
			ViewObjectFocusMethod.RegisterValueChangedCallback(SetSettingDirty);
			ViewRefreshOnUpdate.RegisterValueChangedCallback(SetSettingDirty);
			// ViewRefreshRate.RegisterValueChangedCallback(SetSettingDirty);
			
			InspectorLensEnable.RegisterValueChangedCallback(SetSettingDirty);
			InspectorHideWithNoLink.RegisterValueChangedCallback(SetSettingDirty);
			InspectorDrawObjectInstanceId.RegisterValueChangedCallback(SetSettingDirty);
			
			LogReferenceAdd.RegisterValueChangedCallback(SetSettingDirty);
			LogReferenceRemove.RegisterValueChangedCallback(SetSettingDirty);
		}

		private void RefreshLocalizedText()
		{
			if (!init) return;
			
			enabled.label = L.Inst.setting_enabled;
			localization.label = L.Inst.setting_language;
			
			globalOptionHeader.text = "Setting";
			indexOptionHeader.text = L.Inst.IndexOptionLabel;
			viewOptionHeader.text = L.Inst.ViewOptionLabel;
			inspectorOptionHeader.text = L.Inst.InspectorOptionLabel;
			LogOptionHeader.text = L.Inst.LogOptionLabel;

			/*
			 * Options
			 */
			IndexByGuidRegEx.label = L.Inst.IndexByGuidRegExLabel;
			IndexSceneObject.label = L.Inst.IndexSceneObjectLabel;
			IndexPackageSubDir.label = L.Inst.IndexPackageSubDirLabel;
			
			ViewSceneObject.label = L.Inst.ViewSceneObjectLabel;
			ViewInPlayMode.label = L.Inst.ViewInPlayModeLabel;
			ViewIndexerVersion.label = L.Inst.ViewIndexerVersionLabel;
			ViewObjectFocusMethod.label = L.Inst.ViewObjectFocusMethodLabel;
			ViewRefreshOnUpdate.label = L.Inst.ViewRefreshOnUpdate;
			// ViewRefreshRate.label = L.Inst.ViewRefreshRate;
			
			InspectorLensEnable.label = L.Inst.InspectorLensEnableLabel;
			InspectorHideWithNoLink.label = L.Inst.InspectorHideWithNoLinkLabel;
			InspectorDrawObjectInstanceId.label = L.Inst.InspectorDrawObjectInstanceIdLabel;

			LogReferenceAdd.label = L.Inst.LogReferenceAdd;
			LogReferenceRemove.label = L.Inst.LogReferenceRemove;
			
			/*
			 * Buttons
			 */
			openIndexWizard.text = L.Inst.OpenIndexWizard;
			resetSetting.text = L.Inst.ResetSetting;
			cleanupCache.text = L.Inst.CleanCachedIndices;
			uninstallPackage.text = L.Inst.UninstallPackage;
			
			/*
			 * Help
			 */
			openReadme.text = L.Inst.OpenReadme;
			documentation.text = L.Inst.Documentation;
			reportIssue.text = L.Inst.ReportIssue;
			changelog.text = L.Inst.ChangeLog;
			license.text = L.Inst.License;
			credit.text = L.Inst.Credit;
		}
		
		private void OnLanguageChange(ChangeEvent<string> evt)
		{
#if UNITY_2021_1_OR_NEWER
			if (localization.GetChoices().Contains(evt.newValue))
#else
			if (Setting.GetLanguageChoices().Contains(localization.value))
#endif
			{
				Setting.Localization = evt.newValue;
				L.Inst = Setting.LoadLocalization;

				RefreshLocalizedText();
				AssetLensConsole.Log(R.D($"OnLanguageChanged : {evt.previousValue} -> {evt.newValue}"));
			}
			else
			{
				AssetLensConsole.Log(R.D($"Skip Change Event : Invalid Language ({evt.newValue})"));
			}
		}

		private void OnViewOptionHeaderChange(ChangeEvent<bool> evt)
		{
			ViewGroup.SetEnabled(evt.newValue);
		}

		private void OnInspectorOptionHeaderChange(ChangeEvent<bool> evt)
		{
			InspectorGroup.SetEnabled(evt.newValue);
		}

		private void OnIndexOptionHeaderChange(ChangeEvent<bool> evt)
		{
			IndexGroup.SetEnabled(evt.newValue);
		}

		private void SetSettingDirty<T>(ChangeEvent<T> changeEvent)
		{
			SetSettingDirty();
		}
		
		// private void SetSettingDirty(ChangeEvent<int> changeEvent)
		// {
		// 	SetSettingDirty();
		// }
		//
		// private void SetSettingDirty(ChangeEvent<bool> changeEvent)
		// {
		// 	SetSettingDirty();
		// }
		//
		// private void SetSettingDirty(ChangeEvent<Enum> evt)
		// {
		// 	SetSettingDirty();
		// }

		private void SetSettingDirty()
		{
			Setting.SetSettingDirty();
		}

		private void InitButtons()
		{
			openIndexWizard = root.Q<Button>("open-index-wizard");
			resetSetting = root.Q<Button>("reset-setting");
			cleanupCache = root.Q<Button>("clean-cached-indecies");
			uninstallPackage = root.Q<Button>("uninstall-package");

			openIndexWizard.clickable.clicked += OnOpenIndexWizard;
			resetSetting.clickable.clicked += OnResetSetting;
			cleanupCache.clickable.clicked += CleanupCacheData;
			uninstallPackage.clickable.clicked += OnUninstallPackage;
			
			resetSetting.SetEnabled(false);
		}

		private void InitHelp()
		{
			openReadme = root.Q<Button>("open-readme");
			documentation = root.Q<Button>("documentation");
			reportIssue = root.Q<Button>("report-issue");
			changelog = root.Q<Button>("changelog");
			license = root.Q<Button>("license");
			credit = root.Q<Button>("credit");

			openReadme.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/blob/main/README.md#quickstart");
			documentation.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens");
			reportIssue.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/issues");
			changelog.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/blob/main/CHANGELOG.md");
			license.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/blob/main/LICENSE");
			credit.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev");
			
			openReadme.tooltip = "Open Web Page - https://github.com/seonghwan-dev/AssetLens/blob/main/README.md#quickstart";
			documentation.tooltip = "Open Web Page - https://github.com/seonghwan-dev/AssetLens";
			reportIssue.tooltip = "Open Web Page - https://github.com/seonghwan-dev/AssetLens/issues";
			changelog.tooltip = "Open Web Page - https://github.com/seonghwan-dev/AssetLens/blob/main/CHANGELOG.md";
			license.tooltip = "Open Web Page - https://github.com/seonghwan-dev/AssetLens/blob/main/LICENSE";
			credit.tooltip = "Open Web Page - https://github.com/seonghwan-dev";
			
			documentation.SetEnabled(false);
		}

		private async void OnUninstallPackage()
		{
			if (working) return;
			SetPanelInteractable(false);
			
			working = true;
			Setting.IsEnabled = false;
			
			int processedAssetCount = await AssetLensCache.CleanUpAssetsAsync();
			AssetLensConsole.Log(R.D($"{processedAssetCount} asset caches removed!"));
			
			Directory.Delete(FileSystem.ReferenceCacheDirectory);

#if DEBUG_ASSETLENS
			
			string projectManifest = await File.ReadAllTextAsync(FileSystem.Manifest);
			if (!projectManifest.Contains(Constants.PackageName))
			{
				AssetLensConsole.Log(R.D("Cannot be uninstalled under development."));
				return;
			}
#endif

			string resultMessage = await PackageSystem.Uninstall();
			working = false;
			
			AssetLensConsole.Log(R.D(resultMessage));
			SetPanelInteractable(true);
		}

		private async void CleanupCacheData()
		{
			if (working) return;
			SetPanelInteractable(false);
			
			working = true;
			
			int processedAssetCount = await AssetLensCache.CleanUpAssetsAsync();
			AssetLensConsole.Verbose(R.L($"{processedAssetCount} asset caches removed!"));

			await Task.Delay(1000);

			working = false;
			SetPanelInteractable(true);
		}

		private void OnResetSetting()
		{
			// throw new NotImplementedException();
		}

		private void OnOpenIndexWizard()
		{
			AssetLensIndexWizard.Open();
		}

		private void SetPanelInteractable(bool enable)
		{
			root.SetEnabled(enable);
		}

		private void Awake()
		{
			L.onUpdate += OnLocalizationChange;

			// AssetLensConsole.Log(R.D("Setting Inspector Awake"));
		}

		private void OnDestroy()
		{
			L.onUpdate -= OnLocalizationChange;
			
			// AssetLensConsole.Log(R.D("Setting Inspector Destroy"));
		}

		private void OnLocalizationChange(L l)
		{
			RefreshLocalizedText();
		}
	}
}