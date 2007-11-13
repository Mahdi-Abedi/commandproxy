/*
    The MIT License

    Copyright (c) 2007 Mike Chambers

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml;
using System.IO;

namespace CommandProxy.Commands
{
    class ScreenshotCommand : IProxyCommand
    {
/*
<screenshot format="">
	<path></path>
</screenshot>
*/
        public XmlDocument Exec(XmlDocument requestDocument, XmlDocument responseDocument)
        {
            ImageFormat format = ImageFormat.Png;

            XmlNode commandNode = requestDocument.FirstChild.SelectSingleNode("screenshot");

            XmlAttribute formatAt = commandNode.Attributes["format"];

            if (formatAt != null)
            {
                switch (formatAt.Value)
                {
                    case "png":
                    {
                        format = ImageFormat.Png;
                        break;
                    }
                    case "icon":
                    {
                        format = ImageFormat.Icon;
                        break;
                    }
                    case "jpg":
                    {
                        format = ImageFormat.Jpeg;
                        break;
                    }
                    case "gif":
                    {
                        format = ImageFormat.Gif;
                        break;
                    }
                }
            }

            XmlNode pathNode = commandNode.SelectSingleNode("path");

            string path;
            if (pathNode == null)
            {
                path = Path.GetTempFileName();
            }
            else
            {
                path = pathNode.InnerXml;
            }

            try
            {
                TakeScreenShot(path, format);
            }
            catch (Exception)
            {
                throw new Exception("Error taking screenshot");
            }

            XmlTextReader xmlReader = new XmlTextReader(new StringReader("<path>" + path + "</path>"));
            XmlNode pathElement = responseDocument.ReadNode(xmlReader);
            responseDocument.FirstChild.AppendChild(pathElement);

            return responseDocument;
        }

        private void TakeScreenShot(string path, ImageFormat format)
        {
            try
            {
                Rectangle bounds = Screen.GetBounds(Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    bitmap.Save(path, format);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
