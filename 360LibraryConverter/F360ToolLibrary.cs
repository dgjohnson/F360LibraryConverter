/// <summary>
/// Author: David Johnson
/// Date: 08/08/2020
///
/// Windows application to translate JSON exported Fusion360 tool library into an HSMAdvisor-Compatible XML format.
/// only supports basic conversion as the systems do not have complete parity in terms of the tool metrics that are defined and exported.
/// *although this tool should work for most standard libraries, use it at your own risk!
/// 
/// ©2020 DIY.Engineering LLC
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _360LibraryConverter
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class F360ToolLibrary
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Datum[] Data { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public long? Version { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("BMC", NullValueHandling = NullValueHandling.Ignore)]
        public string Bmc { get; set; }

        [JsonProperty("GRADE", NullValueHandling = NullValueHandling.Ignore)]
        public string Grade { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("geometry", NullValueHandling = NullValueHandling.Ignore)]
        public Geometry Geometry { get; set; }

        [JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Guid { get; set; }

        [JsonProperty("holder", NullValueHandling = NullValueHandling.Ignore)]
        public Holder Holder { get; set; }

        [JsonProperty("last_modified", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastModified { get; set; }

        [JsonProperty("post-process", NullValueHandling = NullValueHandling.Ignore)]
        public PostProcess PostProcess { get; set; }

        [JsonProperty("product-id", NullValueHandling = NullValueHandling.Ignore)]
        public string ProductId { get; set; }

        [JsonProperty("start-values", NullValueHandling = NullValueHandling.Ignore)]
        public StartValues StartValues { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }

        [JsonProperty("vendor", NullValueHandling = NullValueHandling.Ignore)]
        public string Vendor { get; set; }

        [JsonProperty("product-link", NullValueHandling = NullValueHandling.Ignore)]
        public string ProductLink { get; set; }

        [JsonProperty("segments", NullValueHandling = NullValueHandling.Ignore)]
        public Segment[] Segments { get; set; }
    }

    public partial class Geometry
    {
        [JsonProperty("CSP", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Csp { get; set; }

        [JsonProperty("DC", NullValueHandling = NullValueHandling.Ignore)]
        public double? Dc { get; set; }

        [JsonProperty("HAND", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Hand { get; set; }

        [JsonProperty("LB", NullValueHandling = NullValueHandling.Ignore)]
        public double? Lb { get; set; }

        [JsonProperty("LCF", NullValueHandling = NullValueHandling.Ignore)]
        public double? Lcf { get; set; }

        [JsonProperty("NOF", NullValueHandling = NullValueHandling.Ignore)]
        public long? Nof { get; set; }

        [JsonProperty("NT", NullValueHandling = NullValueHandling.Ignore)]
        public long? Nt { get; set; }

        [JsonProperty("OAL", NullValueHandling = NullValueHandling.Ignore)]
        public double? Oal { get; set; }

        [JsonProperty("RE", NullValueHandling = NullValueHandling.Ignore)]
        public double? Re { get; set; }

        [JsonProperty("SFDM", NullValueHandling = NullValueHandling.Ignore)]
        public double? Sfdm { get; set; }

        [JsonProperty("SIG", NullValueHandling = NullValueHandling.Ignore)]
        public long? Sig { get; set; }

        [JsonProperty("TA", NullValueHandling = NullValueHandling.Ignore)]
        public long? Ta { get; set; }

        [JsonProperty("TP", NullValueHandling = NullValueHandling.Ignore)]
        public long? Tp { get; set; }

        [JsonProperty("shoulder-length", NullValueHandling = NullValueHandling.Ignore)]
        public double? ShoulderLength { get; set; }

        [JsonProperty("thread-profile-angle", NullValueHandling = NullValueHandling.Ignore)]
        public long? ThreadProfileAngle { get; set; }

        [JsonProperty("tip-diameter", NullValueHandling = NullValueHandling.Ignore)]
        public double? TipDiameter { get; set; }

        [JsonProperty("tip-length", NullValueHandling = NullValueHandling.Ignore)]
        public long? TipLength { get; set; }

        [JsonProperty("tip-offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? TipOffset { get; set; }
    }

    public partial class Holder
    {
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Guid { get; set; }

        [JsonProperty("last_modified", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastModified { get; set; }

        [JsonProperty("product-id", NullValueHandling = NullValueHandling.Ignore)]
        public string ProductId { get; set; }

        [JsonProperty("segments", NullValueHandling = NullValueHandling.Ignore)]
        public Segment[] Segments { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }

        [JsonProperty("vendor", NullValueHandling = NullValueHandling.Ignore)]
        public String Vendor { get; set; }
    }

    public partial class Segment
    {
        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public long? Height { get; set; }

        [JsonProperty("lower-diameter", NullValueHandling = NullValueHandling.Ignore)]
        public long? LowerDiameter { get; set; }

        [JsonProperty("upper-diameter", NullValueHandling = NullValueHandling.Ignore)]
        public long? UpperDiameter { get; set; }
    }

    public partial class PostProcess
    {
        [JsonProperty("break-control", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BreakControl { get; set; }

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty("diameter-offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? DiameterOffset { get; set; }

        [JsonProperty("length-offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? LengthOffset { get; set; }

        [JsonProperty("live", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Live { get; set; }

        [JsonProperty("manual-tool-change", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ManualToolChange { get; set; }

        [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
        public long? Number { get; set; }

        [JsonProperty("turret", NullValueHandling = NullValueHandling.Ignore)]
        public long? Turret { get; set; }
    }

    public partial class StartValues
    {
        [JsonProperty("presets", NullValueHandling = NullValueHandling.Ignore)]
        public Preset[] Presets { get; set; }
    }

    public partial class Preset
    {
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("f_n", NullValueHandling = NullValueHandling.Ignore)]
        public double? FN { get; set; }

        [JsonProperty("f_z", NullValueHandling = NullValueHandling.Ignore)]
        public double? FZ { get; set; }

        [JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Guid { get; set; }

        [JsonProperty("n", NullValueHandling = NullValueHandling.Ignore)]
        public long? N { get; set; }

        [JsonProperty("n_ramp", NullValueHandling = NullValueHandling.Ignore)]
        public long? NRamp { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("tool-coolant", NullValueHandling = NullValueHandling.Ignore)]
        public string ToolCoolant { get; set; }

        [JsonProperty("use-stepdown", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UseStepdown { get; set; }

        [JsonProperty("use-stepover", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UseStepover { get; set; }

        [JsonProperty("v_c", NullValueHandling = NullValueHandling.Ignore)]
        public double? VC { get; set; }

        [JsonProperty("v_f", NullValueHandling = NullValueHandling.Ignore)]
        public long? VF { get; set; }

        [JsonProperty("v_f_leadIn", NullValueHandling = NullValueHandling.Ignore)]
        public long? VFLeadIn { get; set; }

        [JsonProperty("v_f_leadOut", NullValueHandling = NullValueHandling.Ignore)]
        public long? VFLeadOut { get; set; }

        [JsonProperty("v_f_plunge", NullValueHandling = NullValueHandling.Ignore)]
        public long? VFPlunge { get; set; }

        [JsonProperty("v_f_ramp", NullValueHandling = NullValueHandling.Ignore)]
        public long? VFRamp { get; set; }

        [JsonProperty("v_f_retract", NullValueHandling = NullValueHandling.Ignore)]
        public long? VFRetract { get; set; }
    }

    public partial class F360ToolLibrary
    {
        public static F360ToolLibrary FromJson(string json) => JsonConvert.DeserializeObject<F360ToolLibrary>(json, _360LibraryConverter.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this F360ToolLibrary self) => JsonConvert.SerializeObject(self, _360LibraryConverter.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}

