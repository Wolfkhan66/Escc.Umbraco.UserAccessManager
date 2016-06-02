﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace Escc.Umbraco.UserAccessManager.Services
{
    public class CsvFileService : ICsvFileService
    {
        private readonly string _filePath;

        public CsvFileService(string filePath)
        {
            _filePath = filePath;
        }

        public IList<InspyderLinkModel> GetLinksByDestination(string destinationUrl)
        {
            var rtnList = new List<InspyderLinkModel>();

            // Check a file path was supplied
            if (string.IsNullOrEmpty(_filePath)) return rtnList;

            // Check file is present & accessible
            if (!System.IO.File.Exists(_filePath)) return rtnList;

            // Check the destination url is valid
            destinationUrl = HttpUtility.UrlDecode(destinationUrl);
            if (destinationUrl == null) return rtnList;

            // Check that the Url is absolute, not relative
            if (!IsAbsoluteUrl(destinationUrl))
            {
                // url isn't absolute, so add it to the siteUri value
                // This is JUST to make AbsolutePath work
                var siteUri = ConfigurationManager.AppSettings["SiteUri"];
                destinationUrl = siteUri.Replace("/umbraco/", destinationUrl);
            }

            var destUri = new Uri(destinationUrl, UriKind.Absolute);

            using (var csvParser = new TextFieldParser(_filePath))
            {
                csvParser.CommentTokens = new[] { "#" };
                csvParser.SetDelimiters(",");
                csvParser.HasFieldsEnclosedInQuotes = true;

                string d;
                // Read until we find the Report Title line (Verified Links)
                do
                {
                    d = csvParser.ReadLine();
                    if (d == null) return rtnList;
                } while (!d.StartsWith("Verified Links"));

                // Read past the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    var fields = csvParser.ReadFields();

                    // empty row? Move to the next one
                    if (fields == null) continue;

                    // fields[0] is the item path. Check that it is a valid absolute Url.
                    if (!Uri.IsWellFormedUriString(fields[0], UriKind.Absolute)) continue;

                    var itemUri = new Uri(fields[0]);

                    // Make sure neither Uri values have a trailing / to avoid unintentional misses!
                    var itemUriAbsolutePath = itemUri.AbsolutePath;
                    if (itemUriAbsolutePath.EndsWith("/")) itemUriAbsolutePath = itemUriAbsolutePath.Remove(itemUriAbsolutePath.Length - 1);

                    var destUriAbsolutePath = destUri.AbsolutePath;
                    if (destUriAbsolutePath.EndsWith("/")) destUriAbsolutePath = destUriAbsolutePath.Remove(destUriAbsolutePath.Length - 1);

                    // Ignore this line if it isn't the path we are looking for
                    if (itemUriAbsolutePath != destUriAbsolutePath) continue;

                    // Create a record and add it to the return list
                    var rec = new InspyderLinkModel {Item = fields[0], Type = fields[1], Referrer = fields[2]};
                    rtnList.Add(rec);
                }
            }

            return rtnList;
        }

        private static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
    }
}