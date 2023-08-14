/// <summary>
/// Author: David Johnson
/// Date: 08/08/2020
///
/// Windows application to translate JSON exported Fusion360 tool library into an HSMAdvisor-Compatible XML format.
/// only supports basic conversion as the systems do not have complete parity in terms of the tool metrics that are defined and exported.
/// Although this tool should work for most standard Fusion360 tool libraries, no specific capability is implied.
/// USE AT YOUR OWN RISK!
/// 
/// ©2020 DIY.Engineering LLC
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using CsvHelper;

namespace _360LibraryConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (ImportFileDialog.ShowDialog() == DialogResult.OK)
            {
                lblImportFileName.Text = "Fusion360 Library to Import: " + Path.GetFileName(ImportFileDialog.FileName);
                labelStatus.Text = "Waiting...";
                btnConvertHSM.Enabled = true;
                btnConvertMill.Enabled = true;
                btnConvertMill.BackColor = SystemColors.Highlight;
                btnImport.BackColor = SystemColors.GradientInactiveCaption;
                btnConvertHSM.BackColor = SystemColors.Highlight;
            }
            else
            {
                lblImportFileName.Text = "Select a File to Import.";
                labelStatus.Text = "Waiting...";
                btnConvertHSM.Enabled = false;
                btnImport.BackColor = SystemColors.Highlight;
                btnConvertHSM.BackColor = SystemColors.GradientInactiveCaption;
                btnConvertMill.Enabled = true;
                btnConvertMill.BackColor = SystemColors.GradientInactiveCaption;
            }
        }

        public string GetResourceTextFile(string filename)
        {
            string result = string.Empty;

            using (Stream stream = this.GetType().Assembly.
                       GetManifestResourceStream("_360LibraryConverter." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void BtnConvertHSM_Click(object sender, EventArgs e)
        {
            // instantiate import object and load from json file
            labelStatus.Text = "Loading Export...";
            this.Refresh();
            var f360ToolLibrary = F360ToolLibrary.FromJson(System.IO.File.ReadAllText(ImportFileDialog.FileName));

            // instantiate XmlDocument and load XML from embeddded resource
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(GetResourceTextFile("HSMAdvisorToolLibrary.xml"));

            labelStatus.Text = "Preparing Template...";
            this.Refresh();

            //create node templates
            XmlNode templateTool = doc.SelectSingleNode("//Tools/Tool[1]").CloneNode(true);
            XmlNode templateLibrary = doc.SelectSingleNode("//Libraries/Library[1]").CloneNode(true);

            //purge template reference nodes
            doc.SelectSingleNode("//Tools").RemoveAll();
            doc.SelectSingleNode("//Libraries").RemoveAll();

            string libraryName = null;
            int count = f360ToolLibrary.Data.Length;
            progressBar1.Maximum = count;
            progressBar1.Value = 0;
            //create xml tool node for each json tool
            foreach (var tool in f360ToolLibrary.Data)
            {
                progressBar1.Value += 1;
                labelStatus.Text = "Converting Tool: " + tool.Description;
                this.progressBar1.Refresh();
                this.Refresh();

                if (tool.Type == "holder")
                {
                    continue;
                }
                else
                {

                    //setup new tool
                    XmlNode newTool = templateTool.CloneNode(true);
                    newTool.SelectSingleNode("/guid").InnerText = tool.Guid.ToString();

                    //set the apropriate library
                    labelStatus.Text = "Creating Library...";
                    this.Refresh();
                    switch (radioButton1.Checked) //user library name selection
                    {
                        case true://use vendor name
                            newTool.SelectSingleNode("/library").InnerText = tool.Vendor.ToString();
                            libraryName = tool.Vendor.ToString();
                            break;
                        case false://use custom library
                            newTool.SelectSingleNode("/library").InnerText = textBoxLibraryName.Text;
                            libraryName = textBoxLibraryName.Text;
                            break;
                    }
                    XmlNode libNode = doc.SelectSingleNode("//Libraries/Library[name='" + libraryName + "']");
                    if (libNode == null)//library doesnt exist yet, add it.
                    {
                        templateLibrary.SelectSingleNode("/name").InnerText = libraryName;
                        doc.SelectSingleNode("//Libraries").AppendChild(templateLibrary.CloneNode(true));
                    }

                    labelStatus.Text = "Updating Tool Attributes...";
                    this.Refresh();
                    newTool.SelectSingleNode("/comment").InnerText = tool.Description.ToString();
                    newTool.SelectSingleNode("/brand_name").InnerText = tool.Vendor.ToString();
                    newTool.SelectSingleNode("/series_name").InnerText = tool.Type.ToString();
                    newTool.SelectSingleNode("/supplier").InnerText = tool.Vendor.ToString();
                    newTool.SelectSingleNode("/supplier_pid").InnerText = tool.ProductId.ToString();
                    newTool.SelectSingleNode("/series_name").InnerText = tool.Type.ToString();
                    newTool.SelectSingleNode("/create_date").InnerText = tool.LastModified == null ? DateTimeOffset.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") : DateTimeOffset.FromUnixTimeMilliseconds((long)tool.LastModified).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    newTool.SelectSingleNode("/update_date").InnerText = DateTimeOffset.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    newTool.SelectSingleNode("/id").InnerText = "0";
                    newTool.SelectSingleNode("/user_id").InnerText = "0";
                    switch (tool.Unit)
                    {
                        case "inches":
                            newTool.SelectSingleNode("/diameter").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Dc.ToString();
                            newTool.SelectSingleNode("/stickout").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Lb.ToString();
                            newTool.SelectSingleNode("/Shank_Dia").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Sfdm.ToString();
                            newTool.SelectSingleNode("/Flute_Len").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Lcf.ToString();
                            newTool.SelectSingleNode("/Shoulder_Len").InnerText = tool.Geometry == null ? "0" : tool.Geometry.ShoulderLength.ToString();
                            newTool.SelectSingleNode("/Shoulder_Dia").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Sfdm.ToString();
                            newTool.SelectSingleNode("/diameter_mm").InnerText = "0";
                            newTool.SelectSingleNode("/stickout_mm").InnerText = "0";
                            newTool.SelectSingleNode("/Shank_Dia_mm").InnerText = "0";
                            newTool.SelectSingleNode("/flute_len_mm").InnerText = "0";
                            newTool.SelectSingleNode("/shoulder_len_mm").InnerText = "0";
                            newTool.SelectSingleNode("/Shoulder_Dia_mm").InnerText = "0";
                            break;
                        case "millimeters":
                            newTool.SelectSingleNode("/diameter").InnerText = "0";
                            newTool.SelectSingleNode("/stickout").InnerText = "0";
                            newTool.SelectSingleNode("/Shank_Dia").InnerText = "0";
                            newTool.SelectSingleNode("/Flute_Len").InnerText = "0";
                            newTool.SelectSingleNode("/Shoulder_Len").InnerText = "0";
                            newTool.SelectSingleNode("/Shoulder_Dia").InnerText = "0";
                            newTool.SelectSingleNode("/diameter_mm").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Dc.ToString();
                            newTool.SelectSingleNode("/stickout_mm").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Lb.ToString();
                            newTool.SelectSingleNode("/Shank_Dia_mm").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Sfdm.ToString();
                            newTool.SelectSingleNode("/flute_len_mm").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Lcf.ToString();
                            newTool.SelectSingleNode("/shoulder_len_mm").InnerText = tool.Geometry == null ? "0" : tool.Geometry.ShoulderLength.ToString();
                            newTool.SelectSingleNode("/Shoulder_Dia_mm").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Sfdm.ToString();
                            break;
                    }
                    switch (tool.Type)
                    {
                        case "flat end mill":
                            newTool.SelectSingleNode("/type").InnerText = "endmill";
                            newTool.SelectSingleNode("/corner_rad").InnerText = "0";
                            break;
                        case "ball end mill":
                            newTool.SelectSingleNode("/type").InnerText = "endmill";
                            double rad = (double)(tool.Geometry.Dc == null ? 0 : tool.Geometry.Dc / 2);
                            newTool.SelectSingleNode("/corner_rad").InnerText = rad.ToString();
                            break;
                        case "chamfer mill":
                            newTool.SelectSingleNode("/type").InnerText = "chamfermill";
                            newTool.SelectSingleNode("/corner_rad").InnerText = "0";
                            break;
                        case "thread mill":
                            newTool.SelectSingleNode("/type").InnerText = "threadmill";
                            newTool.SelectSingleNode("/corner_rad").InnerText = "0";
                            break;
                        default:
                            newTool.SelectSingleNode("/type").InnerText = "endmill";
                            newTool.SelectSingleNode("/corner_rad").InnerText = "0";
                            break;
                    }

                    newTool.SelectSingleNode("/number").InnerText = tool.PostProcess == null ? "0" : tool.PostProcess.Number.ToString();
                    newTool.SelectSingleNode("/Flute_N").InnerText = tool.Geometry == null ? "0" : tool.Geometry.Nof.ToString();
                    switch (tool.Bmc)
                    {
                        case "carbide":
                            newTool.SelectSingleNode("/tool_material_id").InnerText = "5";
                            newTool.SelectSingleNode("/coating_id").InnerText = "1";
                            break;
                        case "ti coated":
                            newTool.SelectSingleNode("/tool_material_id").InnerText = "5";
                            newTool.SelectSingleNode("/coating_id").InnerText = "2";
                            break;
                        case "hss":
                            newTool.SelectSingleNode("/tool_material_id").InnerText = "1";
                            newTool.SelectSingleNode("/coating_id").InnerText = "1";
                            break;
                        case "ceramics":
                            newTool.SelectSingleNode("/tool_material_id").InnerText = "10";
                            newTool.SelectSingleNode("/coating_id").InnerText = "1";
                            break;
                        case "unspecified":
                            newTool.SelectSingleNode("/tool_material_id").InnerText = "1";
                            newTool.SelectSingleNode("/coating_id").InnerText = "1";
                            break;
                    }

                    newTool.SelectSingleNode("/doc").InnerText = "0";
                    newTool.SelectSingleNode("/woc").InnerText = "0";

                    //add tool to document
                    doc.SelectSingleNode("//Tools").AppendChild(newTool);
                }
            }

            //save out file
            labelStatus.Text = "Saving File...";
            this.Refresh();
            doc.Save(Path.ChangeExtension(ImportFileDialog.FileName, ".xml"));
            labelStatus.Text = "HSMAdvisor Library Created: " + Path.GetFileName(Path.ChangeExtension(ImportFileDialog.FileName, ".xml"));

            //done, provide feedback
            btnImport.BackColor = SystemColors.Highlight;
            btnConvertHSM.BackColor = SystemColors.GradientInactiveCaption;
            btnConvertMill.BackColor = SystemColors.GradientInactiveCaption;

            playSound("tada.wav");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLibraryName.Enabled = !radioButton1.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/c/DIYEngineering/videos");
        }

        private void playSound(string resName)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream s = a.GetManifestResourceStream("_360LibraryConverter."+resName);
            SoundPlayer player = new SoundPlayer(s);
            player.Play();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            playSound("FairyDust.wav");
            toolTip1.SetToolTip(btnImport, "Select the tool library that was exported as JSON from Fusion360.");
            toolTip1.SetToolTip(groupBox1, "Select the way that tools should be grouped in HSMAdvisor.");
            toolTip1.SetToolTip(btnConvertHSM, "Click the button to create a new HSMAdvisor tool library file from the Fusion360 JSON information.");
            toolTip1.SetToolTip(radioButton1, "Select this to create libraries using the tools vendor name.");
            toolTip1.SetToolTip(radioButton2, "Select this to put all tools in a single library.");
            toolTip1.SetToolTip(linkLabel1, "Subscribe to the channel if this saved you some time!");
            toolTip1.SetToolTip(linkLabel2, "Download the source code to see how it works!");
            toolTip1.SetToolTip(pictureBox1, "Hi!");
            toolTip1.SetToolTip(textBoxLibraryName, "Enter a useful library name here!");

        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/dgjohnson/F360LibraryConverter");
        }

        private void btnConvertMill_Click(object sender, EventArgs e)
        {
            // instantiate import object and load from json file
            labelStatus.Text = "Loading Export...";
            this.Refresh();
            var f360ToolLibrary = F360ToolLibrary.FromJson(System.IO.File.ReadAllText(ImportFileDialog.FileName));

            int count = f360ToolLibrary.Data.Length;
            progressBar1.Maximum = count;
            progressBar1.Value = 0;

            var MillalyzerRecords = new List<ToolEntry>();

            //create xml tool node for each json tool
            foreach (var tool in f360ToolLibrary.Data)
            {
                progressBar1.Value += 1;
                labelStatus.Text = "Converting Tool: " + tool.Description;
                this.progressBar1.Refresh();
                this.Refresh();

                if (tool.Type == "holder")
                {
                    continue;
                }
                else
                {
                    double factor = tool.Unit == "inches" ? 25.4:1;
                    //setup new tool
                    MillalyzerRecords.Add(new ToolEntry
                    {
                        Name = String.Format("{0} / {1} flute / {2}", tool.Vendor.ToString(),tool.Geometry.Nof.ToString(), tool.Type.ToString()),
                        NOF = tool.Geometry.Nof.ToString(),
                        DC = (tool.Geometry.Dc * factor).ToString(),
                        DN = (tool.Geometry.Sfdm * factor).ToString(),
                        DCON = (tool.Geometry.Sfdm * factor).ToString(),
                        APMX = (tool.Geometry.Lcf * factor).ToString(),
                        LN = (tool.Geometry.ShoulderLength * factor).ToString(),
                        LT = (tool.Geometry.ShoulderLength * factor).ToString(),
                        LXP = (tool.Geometry.Lb * factor).ToString(),
                        LF = (tool.Geometry.Oal * factor).ToString(),
                        FHA = 45.ToString(),
                        RE = (tool.Geometry.Re * factor).ToString(),
                        GAMF = 7.ToString(),
                        GAMP = 5.ToString(),
                        EDRD = 6.ToString(),
                        MaterialType = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tool.Bmc.ToLower()),
                        CobaltPercent = 10.ToString(),
                        HelixVariation = 0.ToString(),
                        FluteSpread = 0.ToString(),
                        OptMaterial = "PMKSN",
                        KAPR = 0.ToString()
                    });
                }
            }
            //save out file
            labelStatus.Text = "Saving File...";
            this.Refresh();

            using (var writer = new StreamWriter(Path.ChangeExtension(ImportFileDialog.FileName, ".csv")))
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(MillalyzerRecords);
            }
            labelStatus.Text = "Millalyzer Library Created: " + Path.GetFileName(Path.ChangeExtension(ImportFileDialog.FileName, ".csv"));

            //done, provide feedback
            btnImport.BackColor = SystemColors.Highlight;
            btnConvertMill.BackColor = SystemColors.GradientInactiveCaption;
            btnConvertHSM.BackColor = SystemColors.GradientInactiveCaption;
            playSound("tada.wav");

        }
    }
}
