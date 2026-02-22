using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Xceed.Zip;

namespace BeeSys.Utilities
{
    public class CDownloadArcFile
    {

        private readonly ILogger<CDownloadArcFile> _logger;
        public CDownloadArcFile(ILogger<CDownloadArcFile> logger)
        {
            _logger = logger;
        }

        public const string NEWARCPASSWORD = "";
        public string AssignLicense(Xceed.Zip.ZipArchive zaZipFile, string lincenseId, string licensee, string trace)
        {
            //string licenseId = "{007976A1-FC69-4CFD-93C8-A60E8801FD27}";
            //string licensee = "wasp3d";

            string funcResponse = string.Empty;
            string comment = zaZipFile.Comment;

            var files = zaZipFile.GetFiles(true, "*metadata*");

            if (trace == "4")
                return files.Count().ToString();

            foreach (Xceed.FileSystem.AbstractFile item in files)
            {
                if (item != null && item.Exists)
                {
                    string sWslXml = "";
                    XmlDocument xdMetaData = new XmlDocument();
                    Stream strZipXml = item.OpenRead();
                    using (StreamReader srReader = new StreamReader(strZipXml))
                    {
                        sWslXml = srReader.ReadToEnd();
                    }
                    xdMetaData.LoadXml(sWslXml);
                    strZipXml.Close();


                    if (trace == "5")
                        return sWslXml;


                    string convertedXML = ConvertTemplate(comment, sWslXml, lincenseId, licensee);
                    funcResponse = convertedXML;

                    if(trace == "6")
                        return funcResponse;
                    

                    if (!string.IsNullOrEmpty(convertedXML))
                    {
                        var absTempFile = (ZippedFile)item;
                        var strFile = absTempFile.OpenWrite(true, Xceed.Compression.CompressionMethod.Deflated,
                                        Xceed.Compression.CompressionLevel.Normal, NEWARCPASSWORD);

                        XmlDocument xdMetadata = new XmlDocument();
                        xdMetadata.LoadXml(convertedXML);
                        var bytarrData = GetSGBuffer(xdMetadata);
                        if (bytarrData != null)
                            strFile.Write(bytarrData, 0, bytarrData.Length);

                        if (strFile != null)
                        {
                            strFile.Flush();
                            strFile.Close();
                            strFile = null;
                        }
                    }
                }//end(if (objarrAbstractLocalSG.GetUpperBound(0) > 0))

            }

            return funcResponse;
        }

        private string ConvertTemplate(string edition, string metadata, string licenseId, string licensee, bool isauthor = false)
        {
            _logger.LogInformation($"ConvertTemplate => executed");

            string license = string.Empty;
            string sg = string.Empty;
            try
            {
                if (edition != null)
                {
                    //Pro
                    string str1 = "45435332200000006d7701faa64dd64e0ab0c98d0124ae756d27bb926eb0f8ac731a8c0f028e46700df6c0e14c3c72ed0aa8a5b1e495bf40cae6e99d1be36ec6cb58065e7658c0386173546eca42b6dc3d235aeaa7730c538b5ca5454aab4304e18deddaa4c130ec";
                    string str2 = "45435331200000006d7701faa64dd64e0ab0c98d0124ae756d27bb926eb0f8ac731a8c0f028e46700df6c0e14c3c72ed0aa8a5b1e495bf40cae6e99d1be36ec6cb58065e7658c038";

                    //Xpress
                    string str3 = "454353322000000065876ca4993f06098545896004ee5f152d0b2780d8c8fc9c5b21472313600a6e740c3fd6120bd17949ae9698775ac0884847b215a73bc1ac590fbc4f6be9f6a5e8a1d2685f5dffd8ddec6b86de1fb03a34afe5516f1955a31ddc0f1c42af664d";
                    string str4 = "454353312000000065876ca4993f06098545896004ee5f152d0b2780d8c8fc9c5b21472313600a6e740c3fd6120bd17949ae9698775ac0884847b215a73bc1ac590fbc4f6be9f6a5";

                    //enterprise
                    string str5 = "4543533220000000f54dbd55ca1e7c8bc0d8eab92f9cda575d9a013b7ca641ee10620c8b8cb39e07426a3df65a945f8bca07825a09312ba7b0da2d8e562a1aa0655afc309ed2f183999edce3ed20cafa5332a8c597031b373d8e3d1bf42d062cc74a44112c4e4025";
                    string str6 = "4543533120000000f54dbd55ca1e7c8bc0d8eab92f9cda575d9a013b7ca641ee10620c8b8cb39e07426a3df65a945f8bca07825a09312ba7b0da2d8e562a1aa0655afc309ed2f183";


                    //Community
                    string str7 = "454353322000000061dc7ff11567637d356c365429396f4ec77e5ce15d78c01fd33e673d44cc1121c5d1662e224156f050e264f5723be1b303aef24af4ac8a7adf36846708b3de4de1a0633fad12dbfb71103ce31295e9112aed0e505e1d969598978261932d3fc4";
                    string str8 = "454353312000000061dc7ff11567637d356c365429396f4ec77e5ce15d78c01fd33e673d44cc1121c5d1662e224156f050e264f5723be1b303aef24af4ac8a7adf36846708b3de4d";


                    //Dve
                    string str9 = "4543533220000000ccd524b358b33e6db73dcbb03d510e801fad381ef0e961f7c5f087d7a24492878f9b8e557c68658cf1afb1027567dbc072a1e648cfac4b36614168460997b98ccfb1056f4b14f35aa9c3c16ef93808a83eb096013dbb3783f6a604161eec2de2";
                    string str10 = "4543533120000000ccd524b358b33e6db73dcbb03d510e801fad381ef0e961f7c5f087d7a24492878f9b8e557c68658cf1afb1027567dbc072a1e648cfac4b36614168460997b98c";


                    string bstrPrivate = (string)null;
                    string bstrPublic = (string)null;

                    string metadataXML = metadata;
                    if (!string.IsNullOrEmpty(metadataXML))
                    {
                        XmlDocument xdocmetadata = new XmlDocument();
                        xdocmetadata.LoadXml(metadataXML);
                        XmlNode xnNodeLicense = xdocmetadata.SelectSingleNode("//metadata/scenelicense");
                        if (xnNodeLicense != null)
                        {
                            license = xnNodeLicense.InnerText;

                            XmlDocument licenseDocument = new XmlDocument();
                            licenseDocument.LoadXml(license);
                            XmlElement xMetadata = licenseDocument.DocumentElement;

                            if (xMetadata != null)
                            {
                                XmlNode xinfo = xMetadata.SelectSingleNode(".//info");
                                if (xinfo != null && xinfo is XmlElement)
                                {
                                    XmlAttribute editionAttribute = ((XmlElement)xinfo).GetAttributeNode("edition");



                                    switch (editionAttribute.Value)
                                    {
                                        case "2":
                                            bstrPrivate = str3;
                                            bstrPublic = str4;
                                            edition = "2";
                                            break;
                                        case "3":
                                            bstrPrivate = str1;
                                            bstrPublic = str2;
                                            edition = "3";
                                            break;
                                        case "1":
                                            bstrPrivate = str5;
                                            bstrPublic = str6;
                                            edition = "1";
                                            break;
                                        case "0":
                                            bstrPrivate = str7;
                                            bstrPublic = str8;
                                            edition = "0";
                                            break;
                                        case "4":
                                            bstrPrivate = str9;
                                            bstrPublic = str10;
                                            edition = "4";
                                            break;
                                    }


                                    if (!string.IsNullOrEmpty(licenseId))
                                    {
                                        XmlAttribute xlicensee = ((XmlElement)xinfo).GetAttributeNode("licensee");
                                        if (xlicensee != null)
                                            xlicensee.Value = licensee;

                                        XmlAttribute xlicenseeid = ((XmlElement)xinfo).GetAttributeNode("licenseeid");
                                        if (xlicenseeid != null)
                                            xlicenseeid.Value = licenseId;

                                        if (isauthor)
                                        {
                                            XmlAttribute xauthor = ((XmlElement)xinfo).GetAttributeNode("authororg");
                                            if (xauthor != null)
                                                xauthor.Value = licensee;

                                            XmlAttribute xAuthorid = ((XmlElement)xinfo).GetAttributeNode("authororgid");
                                            if (xAuthorid != null)
                                                xAuthorid.Value = licenseId;
                                        }
                                    }


                                    try
                                    {
                                        BeeSys.WaspHandShake waspHandShake = new BeeSys.WaspHandShake();
                                        waspHandShake.SetTokens(bstrPrivate, bstrPublic);

                                        _logger.LogInformation($"ConvertTemplate => Handshake Done for edition {edition} with licensee {licensee} and licenseId {licenseId}");

                                        _logger.LogTrace($"ConvertTemplate => Handshake Done for edition {edition} with licensee {licensee} and licenseId {licenseId}");

                                        int retCode = 0;
                                        string metadatasignature = string.Empty;


                                        waspHandShake.SignString(xinfo.OuterXml, ref metadatasignature, ref retCode);

                                        XmlAttribute xsignature = ((XmlElement)xMetadata).GetAttributeNode("signature");
                                        if (xsignature != null)
                                            xsignature.Value = metadatasignature;
                                    }
                                    catch(Exception ex)
                                    {
                                        _logger.LogError($"ConvertTemplate => Exception => {ex}");

                                        throw new Exception("Error in waspHandShake.SetTokens(bstrPrivate, bstrPublic); " + ex.Message);
                                    }

                                }
                                //https://waspsource.beesys.com/Products/licensepublisher/-/issues/19
                                //Insert license string in CData section in publisher
                                XmlCDataSection cdata = xdocmetadata.CreateCDataSection(xMetadata.OuterXml);
                                xnNodeLicense.RemoveAll();
                                xnNodeLicense.AppendChild(cdata);
                                return xdocmetadata.OuterXml;
                            }
                        }
                    }

                }

            }
            finally
            {

            }
            return null;
        }

        private static byte[] GetSGBuffer(XmlDocument xdSG)
        {
            MemoryStream memstrSG = null;
            XmlWriter xw;
            try
            {
                if (xdSG != null)
                {

                    memstrSG = new MemoryStream();
                    xw = XmlWriter.Create(memstrSG);
                    xdSG.Save(xw);
                    xw.Close();
                    memstrSG.Position = 0;
                    return memstrSG.ToArray();
                }//end(if (xdSG != null))
                return null;
            }//end(try)

            finally
            {
                memstrSG = null;
            }//end(finally)
        }//end(private void GetSGBuffer(XmlDocument xdSG))
    }
}