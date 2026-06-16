using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace OWASP.WebGoat.NET
{
    public partial class XPathInjection : System.Web.UI.Page
    {
        // Make into actual lesson
        private string xml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><sales><salesperson><name>David Palmer</name><city>Portland</city><state>or</state><ssn>123-45-6789</ssn></salesperson><salesperson><name>Jimmy Jones</name><city>San Diego</city><state>ca</state><ssn>555-45-6789</ssn></salesperson><salesperson><name>Tom Anderson</name><city>New York</city><state>ny</state><ssn>444-45-6789</ssn></salesperson><salesperson><name>Billy Moses</name><city>Houston</city><state>tx</state><ssn>333-45-6789</ssn></salesperson></sales>";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["state"] != null)
            {
                FindSalesPerson(Request.QueryString["state"]);
            }
        }

        private void FindSalesPerson(string state)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

            XPathExpression expr = XPathExpression.Compile("//salesperson[state=$state]");
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("state", string.Empty, state);
            expr.SetContext(new XPathVariableContext(args));

            XmlNodeList list = xDoc.SelectNodes(expr);
            if (list.Count > 0)
            {

            }

        }

        private sealed class XPathVariableContext : XsltContext
        {
            private readonly XsltArgumentList _args;

            public XPathVariableContext(XsltArgumentList args)
            {
                _args = args;
            }

            public override bool Whitespace
            {
                get { return true; }
            }

            public override int CompareDocument(string baseUri, string nextbaseUri)
            {
                return 0;
            }

            public override bool PreserveWhitespace(XPathNavigator node)
            {
                return true;
            }

            public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
            {
                return null;
            }

            public override IXsltContextVariable ResolveVariable(string prefix, string name)
            {
                return new XPathVariable(_args, name);
            }
        }

        private sealed class XPathVariable : IXsltContextVariable
        {
            private readonly XsltArgumentList _args;
            private readonly string _name;

            public XPathVariable(XsltArgumentList args, string name)
            {
                _args = args;
                _name = name;
            }

            public bool IsLocal
            {
                get { return false; }
            }

            public bool IsParam
            {
                get { return true; }
            }

            public XPathResultType VariableType
            {
                get { return XPathResultType.Any; }
            }

            public object Evaluate(XsltContext xsltContext)
            {
                return _args.GetParam(_name, string.Empty);
            }
        }
    }
}

