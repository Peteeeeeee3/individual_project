using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class NFCScripts : IPreprocessBuildWithReport
{

	public int callbackOrder { get { return 0; } }

	public void OnPreprocessBuild(BuildReport report)
	{
	}


    [PostProcessBuild]
    public static void OnPostProcess(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS && buildTarget != BuildTarget.tvOS)
        {
            return;
        }
#if UNITY_IOS

		var projectPath = PBXProject.GetPBXProjectPath(buildPath);
		var project = new PBXProject();
		project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
		project = new PBXProject();
		project.ReadFromFile(projectPath);

		CustomCapabilityManager manager = new CustomCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
#else
		CustomCapabilityManager manager = new CustomCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
#endif

        if (NFCConfig.associatedAppDomains != null && NFCConfig.associatedAppDomains.Length > 0)
        {
            manager.AddAssociatedDomains(NFCConfig.associatedAppDomains);
        }

        manager.AddNearFieldCommunications();
        manager.WriteToFile();


        try
        {
            // Add a default NFCReaderUsageDescription
            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElementDict rootDict = plist.root;
            if (rootDict["NFCReaderUsageDescription"] == null)
            {
                rootDict.SetString("NFCReaderUsageDescription", "Allow to scan for tags");
                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
        catch (Exception e)
        {
        }
#endif

    }



}


#if UNITY_IOS
class CustomCapabilityManager : ProjectCapabilityManager
{
    //	private const BindingFlags NonPublicInstanceBinding = BindingFlags.NonPublic | BindingFlags.Instance;

    //    private static readonly string NearFieldCommunicationsEntitlementsKey = "com.apple.developer.nfc.readersession.formats";


    private const string EntitlementsArrayKey = "com.apple.developer.nfc.readersession.formats";
    private const string DefaultFormat = "NDEF";
    private const string CoreNFCFramework = "CoreNFC.framework";
    private const BindingFlags NonPublicInstanceBinding = BindingFlags.NonPublic | BindingFlags.Instance;




    public CustomCapabilityManager(string buildPath, string entitlementFileName, string targetName = null, string targetGuid = null) : base(buildPath, entitlementFileName, targetName, targetGuid)
    {

    }


    public void AddNearFieldCommunications(string unityFrameworkTargetGuid=null)
    {
        var managerType = typeof(ProjectCapabilityManager);
        var capabilityTypeType = typeof(PBXCapabilityType);

        var projectField = managerType.GetField("project", NonPublicInstanceBinding);
        var targetGuidField = managerType.GetField("m_TargetGuid", NonPublicInstanceBinding);
        var entitlementFilePathField = managerType.GetField("m_EntitlementFilePath", NonPublicInstanceBinding);
        var getOrCreateEntitlementDocMethod = managerType.GetMethod("GetOrCreateEntitlementDoc", NonPublicInstanceBinding);
        var constructorInfo = capabilityTypeType.GetConstructor(
            NonPublicInstanceBinding,
            null,
            new[] { typeof(string), typeof(bool), typeof(string), typeof(bool) },
            null);

        if (projectField == null || targetGuidField == null || entitlementFilePathField == null ||
            getOrCreateEntitlementDocMethod == null || constructorInfo == null)
            throw new Exception("Can't Add Near Field Communications programatically in this Unity version");

        var entitlementFilePath = entitlementFilePathField.GetValue(this) as string;
        var entitlementDoc = getOrCreateEntitlementDocMethod.Invoke(this, new object[] { }) as PlistDocument;
        if (entitlementDoc != null)
        {
            var plistArray = new PlistElementArray();
            plistArray.AddString(DefaultFormat);
            entitlementDoc.root[EntitlementsArrayKey] = plistArray;
        }

        var project = projectField.GetValue(this) as PBXProject;
        if (project != null)
        {
            var mainTargetGuid = targetGuidField.GetValue(this) as string;
            var capabilityType = constructorInfo.Invoke(new object[] { "com.apple.developer.nfc.custom", true, string.Empty, true }) as PBXCapabilityType;

            var targetGuidToAddFramework = unityFrameworkTargetGuid;
            if (targetGuidToAddFramework == null)
            {
                targetGuidToAddFramework = mainTargetGuid;
            }

            project.AddFrameworkToProject(targetGuidToAddFramework, CoreNFCFramework, true);
            project.AddCapability(mainTargetGuid, capabilityType, entitlementFilePath, false);
        }

    }

}
#endif



