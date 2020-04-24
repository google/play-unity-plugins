// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Google.Android.AppBundle.Editor.Internal.AndroidManifest;
using Google.Play.Common;
using Google.Play.Common.Internal;

namespace Google.Play.Instant.Editor.Internal.AndroidManifest
{
    /// <summary>
    /// A helper class for updating the AndroidManifest to target installed vs instant apps.
    /// </summary>
    public static class AndroidManifestHelper
    {
        public const string PreconditionOneManifestElement = "expect 1 manifest element";
        public const string PreconditionMissingXmlnsAndroid = "missing manifest attribute xmlns:android";
        public const string PreconditionInvalidXmlnsAndroid = "invalid value for xmlns:android";
        public const string PreconditionInvalidXmlnsDistribution = "invalid value for xmlns:dist";
        public const string PreconditionOneApplicationElement = "expect 1 application element";
        public const string PreconditionOneModuleInstant = "more than one dist:module element with dist:instant";

        private const string PreconditionInvalidXmlnsTools = "invalid value for xmlns:tools";

        private const string PreconditionOneAssetModule =
            "more than one dist:module element with dist:type asset-pack";

        private const string PreconditionOneMetaDataFlavor =
            "more than one meta-data element for com.google.android.gms.instant.flavor";

        private delegate IEnumerable<XElement> ElementFinder(XElement element);

        /// <summary>
        /// Returns the <see cref="IAndroidManifestUpdater"/> appropriate for the version of Unity.
        /// </summary>
        public static IAndroidManifestUpdater GetAndroidManifestUpdater()
        {
#if UNITY_2018_1_OR_NEWER
            return new PostGenerateGradleProjectAndroidManifestUpdater();
#else
            return new LegacyAndroidManifestUpdater();
#endif
        }

        /// <summary>
        /// Creates a new XDocument representing a basic Unity AndroidManifest.xml file.
        /// </summary>
        public static XDocument CreateManifestXDocument()
        {
            return new XDocument(new XElement(
                    ManifestConstants.Manifest,
                    new XAttribute(ManifestConstants.AndroidXmlns,
                        XNamespace.Get(ManifestConstants.AndroidNamespaceUrl)),
                    new XElement(ManifestConstants.Application,
                        new XAttribute(ManifestConstants.AndroidIconXName, "@drawable/app_icon"),
                        new XAttribute(ManifestConstants.AndroidLabelXName, "@string/app_name"),
                        new XElement(ManifestConstants.Activity,
                            new XAttribute(
                                ManifestConstants.AndroidNameXName,
                                "com.unity3d.player.UnityPlayerActivity"),
                            new XAttribute(ManifestConstants.AndroidLabelXName, "@string/app_name"),
                            new XElement(ManifestConstants.IntentFilter,
                                new XElement(ManifestConstants.Action,
                                    new XAttribute(ManifestConstants.AndroidNameXName,
                                        AndroidConstants.IntentActionMain)),
                                new XElement(ManifestConstants.Category,
                                    new XAttribute(ManifestConstants.AndroidNameXName,
                                        AndroidConstants.IntentCategoryLauncher))
                            )
                        )
                    )
                )
            );
        }

        /// <summary>
        /// Converts the specified XDocument representing an AndroidManifest to support an installed app build.
        /// </summary>
        public static void ConvertManifestToInstalled(XDocument doc)
        {
            foreach (var manifestElement in doc.Elements(ManifestConstants.Manifest))
            {
                manifestElement.Attributes(ManifestConstants.AndroidTargetSandboxVersionXName).Remove();
                FindDistributionModuleInstantElements(manifestElement).Remove();
                // TODO: it may not always be safe to remove the "dist" or "tools" namespaces.
                manifestElement.Attributes(ManifestConstants.DistXmlns).Remove();
                manifestElement.Attributes(ManifestConstants.ToolsXmlns).Remove();

                var applicationElement = GetExactlyOne(manifestElement.Elements(ManifestConstants.Application));
                if (applicationElement != null)
                {
                    FindFlavorMetaDataElements(applicationElement).Remove();
                }
            }
        }

        /// <summary>
        /// Converts the specified XDocument representing an AndroidManifest to support an instant app build.
        /// </summary>
        /// <param name="doc">An XDocument representing an AndroidManifest.</param>
        /// <param  name="supportPlayGamesApp">
        /// Whether or not to configure the manifest to support being launched by the Play Games App.
        /// </param>
        /// <returns>An error message if there was a problem updating the manifest, or null if successful.</returns>
        public static string ConvertManifestToInstant(XDocument doc, bool supportPlayGamesApp)
        {
            var manifestElement = GetExactlyOne(doc.Elements(ManifestConstants.Manifest));
            if (manifestElement == null)
            {
                return PreconditionOneManifestElement;
            }

            // Verify that "xmlns:android" is already present and correct.
            var androidAttribute = manifestElement.Attribute(ManifestConstants.AndroidXmlns);
            if (androidAttribute == null)
            {
                return PreconditionMissingXmlnsAndroid;
            }

            if (androidAttribute.Value != ManifestConstants.AndroidNamespaceUrl)
            {
                return PreconditionInvalidXmlnsAndroid;
            }

            // Don't assume that "xmlns:dist" is already present. If it is present, verify that it's correct.
            var distAttribute = manifestElement.Attribute(ManifestConstants.DistXmlns);
            if (distAttribute == null)
            {
                manifestElement.SetAttributeValue(ManifestConstants.DistXmlns, ManifestConstants.DistNamespaceUrl);
            }
            else if (distAttribute.Value != ManifestConstants.DistNamespaceUrl)
            {
                return PreconditionInvalidXmlnsDistribution;
            }

            // Don't assume that "xmlns:tools" is already present. If it is present, verify that it's correct.
            var toolsAttribute = manifestElement.Attribute(ManifestConstants.ToolsXmlns);
            if (toolsAttribute == null)
            {
                manifestElement.SetAttributeValue(ManifestConstants.ToolsXmlns, ManifestConstants.ToolsNamespaceUrl);
            }
            else if (toolsAttribute.Value != ManifestConstants.ToolsNamespaceUrl)
            {
                return PreconditionInvalidXmlnsTools;
            }

            // The manifest element <dist:module dist:instant="true" /> is required for AppBundles.
            var moduleInstantResult = UpdateDistributionModuleInstantElement(manifestElement);
            if (moduleInstantResult != null)
            {
                return moduleInstantResult;
            }

            // TSV2 is required for instant apps starting with Android Oreo.
            manifestElement.SetAttributeValue(ManifestConstants.AndroidTargetSandboxVersionXName, "2");

            var applicationElement = GetExactlyOne(manifestElement.Elements(ManifestConstants.Application));
            if (applicationElement == null)
            {
                return PreconditionOneApplicationElement;
            }

            if (supportPlayGamesApp)
            {
                var result = UpdateMetaDataElement(FindFlavorMetaDataElements, applicationElement,
                    PreconditionOneMetaDataFlavor, PlayInstantManifestConstants.Flavor,
                    PlayInstantManifestConstants.PlayGamesInstantFlavor);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the specified XDocument representing an asset pack AndroidManifest to support an instant app build.
        /// </summary>
        /// <param name="doc">An XDocument representing an asset pack AndroidManifest.</param>
        /// <returns>An error message if there was a problem updating the manifest, or null if successful.</returns>
        public static string ConvertAssetPackManifestToInstant(XDocument doc)
        {
            var manifestElement = GetExactlyOne(doc.Elements(ManifestConstants.Manifest));
            if (manifestElement == null)
            {
                return PreconditionOneManifestElement;
            }

            var moduleElement = GetExactlyOne(manifestElement.Elements(ManifestConstants.DistModuleXName));
            if (moduleElement == null)
            {
                return PreconditionOneAssetModule;
            }

            moduleElement.SetAttributeValue(ManifestConstants.DistInstantXName, ManifestConstants.ValueTrue);
            return null;
        }

        /// <summary>
        /// Searches the specified XDocument for an application element.
        /// </summary>
        /// <param name="doc">An XDocument representing an AndroidManifest.</param>
        /// <returns>The first application element in the XDocument, or null if one cannot be found.</returns>
        public static XElement GetApplicationXElement(XDocument doc)
        {
            var manifestElement = GetExactlyOne(doc.Elements(ManifestConstants.Manifest));
            if (manifestElement == null)
            {
                return null;
            }

            return GetExactlyOne(manifestElement.Elements(ManifestConstants.Application));
        }

        /// <summary>
        /// Creates an XElement configured to set the component's enabled attribute to true after a
        /// manifest merge. This allows us to enable components that are set to false in an aar manifest.
        /// </summary>
        /// <param name="androidName">The android:name of the component to be enabled.</param>
        /// <param name="componentType">The component tag to apply for (e.g. activity, service).</param>
        public static XElement GetReplaceComponentElement(string androidName, string componentType)
        {
            return new XElement(componentType,
                new XAttribute(ManifestConstants.AndroidNameXName, androidName),
                new XAttribute(ManifestConstants.AndroidEnabledXName, ManifestConstants.ValueTrue),
                new XAttribute(ManifestConstants.ToolsReplaceXName, ManifestConstants.AndroidEnabledXName.LocalName));
        }

        /// <summary>
        /// Updates the specified manifest to indicate that it is an instant module, necessary for building app bundles.
        /// </summary>
        /// <returns>An error message if there was a problem updating the manifest, or null if successful.</returns>
        private static string UpdateDistributionModuleInstantElement(XElement manifestElement)
        {
            var moduleInstant =
                GetElement(FindDistributionModuleInstantElements, manifestElement, ManifestConstants.DistModuleXName);
            if (moduleInstant == null)
            {
                return PreconditionOneModuleInstant;
            }

            moduleInstant.SetAttributeValue(ManifestConstants.DistInstantXName, ManifestConstants.ValueTrue);

            return null;
        }

        private static IEnumerable<XElement> FindDistributionModuleInstantElements(XContainer manifestElement)
        {
            // Find all elements of the form <dist:module dist:instant="..." />
            return from moduleElement in manifestElement.Elements(ManifestConstants.DistModuleXName)
                where moduleElement.Attribute(ManifestConstants.DistInstantXName) != null
                select moduleElement;
        }

        private static IEnumerable<XElement> FindFlavorMetaDataElements(XContainer application)
        {
            // Find all elements of the form <meta-data android:name="com.google.android.gms.instant.flavor" />
            return from metaData in application.Elements(PlayInstantManifestConstants.MetaData)
                where (string) metaData.Attribute(ManifestConstants.AndroidNameXName) ==
                      PlayInstantManifestConstants.Flavor
                select metaData;
        }

        /// <summary>
        /// Updates the specified meta-data element to the specified value.
        /// </summary>
        /// <returns>An error message if there was a problem updating the manifest, or null if successful.</returns>
        private static string UpdateMetaDataElement(
            ElementFinder finder, XElement parentElement, string errorMessage, string name, object attributeValue)
        {
            var metaDataElement = GetElement(finder, parentElement, PlayInstantManifestConstants.MetaData);
            if (metaDataElement == null)
            {
                return errorMessage;
            }

            metaDataElement.SetAttributeValue(ManifestConstants.AndroidNameXName, name);
            metaDataElement.SetAttributeValue(PlayInstantManifestConstants.AndroidValueXName, attributeValue);

            return null;
        }

        private static XElement GetExactlyOne(IEnumerable<XElement> elements)
        {
            // If the IEnumerable has exactly 1 element, return it. If the IEnumerable has 0 or 2+ elements, return
            // null. Cannot use FirstOrDefault() here since that will return the first element if 2+ elements.
            return elements.Count() == 1 ? elements.First() : null;
        }

        /// <summary>
        /// Uses the specified delegate to find all matching elements attached to the specified parent element. If
        /// no matches are found, create a new element and return it. If one match is found, remove any existing
        /// elements/attributes and return it. If more than one match is found, return null to indicate an error.
        /// </summary>
        private static XElement GetElement(ElementFinder finder, XElement parentElement, XName elementName)
        {
            var elements = finder(parentElement);
            switch (elements.Count())
            {
                case 0:
                    var createdElement = new XElement(elementName);
                    parentElement.Add(createdElement);
                    return createdElement;
                case 1:
                    var existingElement = elements.First();
                    existingElement.RemoveAll();
                    return existingElement;
                default:
                    // This is unexpected. The caller is responsible for logging an error.
                    return null;
            }
        }
    }
}