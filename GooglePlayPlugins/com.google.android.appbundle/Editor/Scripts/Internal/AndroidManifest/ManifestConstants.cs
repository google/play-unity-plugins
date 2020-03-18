// Copyright 2019 Google LLC
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

using System.Xml.Linq;

namespace Google.Android.AppBundle.Editor.Internal.AndroidManifest
{
    /// <summary>
    /// String constants used by AndroidManifest.xml files.
    /// </summary>
    public static class ManifestConstants
    {
        public const string Action = "action";
        public const string Activity = "activity";
        public const string Application = "application";
        public const string AssetPack = "asset-pack";
        public const string Category = "category";
        public const string Data = "data";
        public const string IntentFilter = "intent-filter";
        public const string Manifest = "manifest";
        public const string Package = "package";
        public const string Service = "service";
        public const string Split = "split";
        public const string ValueTrue = "true";

        public const string AndroidNamespaceAlias = "android";
        public const string AndroidNamespaceUrl = "http://schemas.android.com/apk/res/android";
        public static readonly XName AndroidXmlns = XNamespace.Xmlns + AndroidNamespaceAlias;
        public static readonly XName AndroidEnabledXName = XName.Get("enabled", AndroidNamespaceUrl);
        public static readonly XName AndroidIconXName = XName.Get("icon", AndroidNamespaceUrl);
        public static readonly XName AndroidLabelXName = XName.Get("label", AndroidNamespaceUrl);
        public static readonly XName AndroidNameXName = XName.Get("name", AndroidNamespaceUrl);

        public static readonly XName AndroidTargetSandboxVersionXName =
            XName.Get("targetSandboxVersion", AndroidNamespaceUrl);

        public const string DistNamespaceAlias = "dist";
        public const string DistNamespaceUrl = "http://schemas.android.com/apk/distribution";
        public static readonly XName DistXmlns = XNamespace.Xmlns + DistNamespaceAlias;
        public static readonly XName DistDeliveryXName = XName.Get("delivery", DistNamespaceUrl);
        public static readonly XName DistFastFollowName = XName.Get("fast-follow", DistNamespaceUrl);
        public static readonly XName DistFusingXName = XName.Get("fusing", DistNamespaceUrl);
        public static readonly XName DistIncludeXName = XName.Get("include", DistNamespaceUrl);
        public static readonly XName DistInstallTimeName = XName.Get("install-time", DistNamespaceUrl);
        public static readonly XName DistInstantXName = XName.Get("instant", DistNamespaceUrl);
        public static readonly XName DistModuleXName = XName.Get("module", DistNamespaceUrl);
        public static readonly XName DistOnDemandXName = XName.Get("on-demand", DistNamespaceUrl);
        public static readonly XName DistTypeXName = XName.Get("type", DistNamespaceUrl);

        public const string ToolsNamespaceAlias = "tools";
        public const string ToolsNamespaceUrl = "http://schemas.android.com/tools";
        public static readonly XName ToolsXmlns = XNamespace.Xmlns + ToolsNamespaceAlias;
        public static readonly XName ToolsReplaceXName = XName.Get("replace", ToolsNamespaceUrl);
    }
}