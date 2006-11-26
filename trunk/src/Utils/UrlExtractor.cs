/* [ Utils/UrlExtractor.cs ] 
 * Author: Matteo Bertozzi
 * ============================================================================
 * Niry Sharp
 * Copyright (C) 2006 Matteo Bertozzi.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301 USA
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Niry.Utils {
	/// Extract Images and Links Url From (Html) Web Page
	public class UrlExtractor {
		// ============================================
		// PRIVATE Members
		// ============================================
		private string baseUrl = null;
		private string html = null;
		private string url = null;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Url Extractor
		public UrlExtractor (string url) : this(url, (WebProxy) null) {
		}

		/// Create New Url Extractor using your source
		public UrlExtractor (string url, string html) {
			this.url = url;
			this.html = html.Replace(">", ">\n");
			this.baseUrl = GetBaseUrl();
		}

		/// Create New Url Extractor using Proxy
		public UrlExtractor (string url, WebProxy proxy) {
			this.url = url;
			this.html = FetchPage(proxy).Replace(">", ">\n");
			this.baseUrl = GetBaseUrl();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Get All The Images in The Page
		public string[] GetImages() {
			return(Extractor("img", "src"));
		}

		/// Get All The Links in The Page
		public string[] GetLinks() {
			return(Extractor("a", "href"));
		}

		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		/// Get All The Images in The Page
		public static string[] GetImages (string url) {
			UrlExtractor extractor = new UrlExtractor(url);
			string[] images = extractor.GetImages();
			return(images);
		}

		/// Get All The Links in The Page
		public static string[] GetLinks (string url) {
			UrlExtractor extractor = new UrlExtractor(url);
			string[] links = extractor.GetLinks();
			return(links);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		/// Get The Source of The Page
		private string FetchPage (WebProxy proxy) {
			// Make Http Request
			WebRequest request = WebRequest.Create(this.url);
			request.Timeout = 5000;

			// Set Proxy
			if (proxy != null) request.Proxy = proxy;

			// Wait Http Response
			WebResponse response = request.GetResponse();
			StreamReader stream = new StreamReader(response.GetResponseStream());
			string page = stream.ReadToEnd();
			stream.Close();
			response.Close();
			return(page);
		}

		/// Get Web Page Base URL or default url page if is not specified
		private string GetBaseUrl() {
			string[] baseUrls = Extractor("base", "href");
			if (baseUrls != null) {
				foreach (string url in baseUrls) Console.WriteLine(url);
				return(baseUrls[0]);
			}
			return(this.url);
		}

		/// Get Page Complete Url
		private string GetCompleteUrl (string url) {
			string regex = @"^(http://|https://|ftp://|ftps://|mailto:)";

			if (Regex.IsMatch(url, regex) == false) {
				if (url[0] == '/') url = url.Substring(1);

				// Fix Me: if url is ../ path
				url = Path.Combine(this.baseUrl, url);
			}

			return(url);
		}

		/// Extract Attribute Value of the Specified Tag
		private string[] Extractor (string tag, string attribute) {
			ArrayList urlList = new ArrayList();

			string regex1 = "<" + tag + @"[^>]*?" + attribute + @"\s*=\s*[""']?([^'"" >]+?)[ '""]?.*>";
			string regex2 = "(" + attribute + @"\s*=\s*[""'](.+?)[""'])";
			MatchCollection matches = Regex.Matches(this.html, regex1, RegexOptions.IgnoreCase);
			for (int i=0; i < matches.Count; i++) {
				try {
					Match m;

					// Extract href='url'
					m = Regex.Match(matches[i].Value, regex2, RegexOptions.IgnoreCase);

					// Extract 'url'
					m = Regex.Match(m.Value, @"([""'](.+?)[""'])", RegexOptions.IgnoreCase);

					// Get Url and Add To List
					string url = m.Value.Substring(1, m.Value.Length - 2);
					urlList.Add(GetCompleteUrl(url));
				} catch {}
			}

			return(urlList.Count > 0 ? (string[]) urlList.ToArray(typeof(string)) : null);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Return The Page URL
		public string Url {
			get { return(this.url); }
		}
	}
}
