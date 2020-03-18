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

using System;
using System.Xml.Linq;
using Google.Android.AppBundle.Editor.AssetPacks;

namespace Google.Android.AppBundle.Editor.Internal.AndroidManifest
{
    /// <summary>
    /// A helper class for creating asset pack AndroidManifest xml documents.
    /// </summary>
    public static class AssetPackManifestHelper
    {
        /// <summary>
        /// Creates a new XDocument representing the AndroidManifest.xml for an asset pack.
        /// </summary>
        /// <param name="packageName">Package name of this application.</param>
        /// <param name="assetPackName">The name of the asset pack.</param>
        /// <param name="deliveryMode">The <see cref="AssetPackDeliveryMode"/> of this asset pack.</param>
        public static XDocument CreateAssetPackManifestXDocument(
            string packageName, string assetPackName, AssetPackDeliveryMode deliveryMode)
        {
            // TODO: Add support for <dist:instant-delivery>
            var deliveryTypeXName = ManifestConstants.DistDeliveryXName;
            XName deliveryModeXName;
            switch (deliveryMode)
            {
                case AssetPackDeliveryMode.OnDemand:
                    deliveryModeXName = ManifestConstants.DistOnDemandXName;
                    break;
                case AssetPackDeliveryMode.InstallTime:
                    deliveryModeXName = ManifestConstants.DistInstallTimeName;
                    break;
                case AssetPackDeliveryMode.FastFollow:
                    deliveryModeXName = ManifestConstants.DistFastFollowName;
                    break;
                default:
                    throw new ArgumentException("Unexpected delivery mode: " + deliveryMode, "deliveryMode");
            }

            var moduleElement = new XElement(
                ManifestConstants.DistModuleXName,
                new XAttribute(ManifestConstants.DistTypeXName, ManifestConstants.AssetPack),
                new XElement(deliveryTypeXName,
                    new XElement(deliveryModeXName)),
                new XElement(ManifestConstants.DistFusingXName,
                    new XAttribute(ManifestConstants.DistIncludeXName, ManifestConstants.ValueTrue))
            );

            return new XDocument(new XElement(
                ManifestConstants.Manifest,
                new XAttribute(ManifestConstants.AndroidXmlns, XNamespace.Get(ManifestConstants.AndroidNamespaceUrl)),
                new XAttribute(ManifestConstants.DistXmlns, XNamespace.Get(ManifestConstants.DistNamespaceUrl)),
                new XAttribute(ManifestConstants.Package, packageName),
                new XAttribute(ManifestConstants.Split, assetPackName),
                moduleElement));
        }
    }
}