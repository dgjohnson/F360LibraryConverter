﻿/// <summary>
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
                btnConvert.Enabled = true;
                btnImport.BackColor = SystemColors.GradientInactiveCaption;
                btnConvert.BackColor = SystemColors.Highlight;
            }
            else
            {
                lblImportFileName.Text = "Select a File to Import.";
                labelStatus.Text = "Waiting...";
                btnConvert.Enabled = false;
                btnImport.BackColor = SystemColors.Highlight;
                btnConvert.BackColor = SystemColors.GradientInactiveCaption;
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

        private void btnConvert_Click(object sender, EventArgs e)
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
                this.Refresh();

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

            //save out file
            labelStatus.Text = "Saving File...";
            this.Refresh();
            doc.Save(Path.ChangeExtension(ImportFileDialog.FileName, ".xml"));
            labelStatus.Text = "HSMAdvisor Library Created: " + Path.GetFileName(Path.ChangeExtension(ImportFileDialog.FileName, ".xml"));

            //done, provide feedback
            btnImport.BackColor = SystemColors.Highlight;
            btnConvert.BackColor = SystemColors.GradientInactiveCaption;

            playSound();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLibraryName.Enabled = !radioButton1.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/c/DIYEngineering/videos");
        }

        private void playSound()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream s = a.GetManifestResourceStream("_360LibraryConverter.tada.wav");
            SoundPlayer player = new SoundPlayer(s);
            player.Play();
        }
    }
}