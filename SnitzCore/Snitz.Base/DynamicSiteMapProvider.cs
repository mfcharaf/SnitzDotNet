/*	
 * DynamicSiteMapProvider 
 * 
 * Created by Simon Harriyott http://harriyott.com
 * More details at http://tinyurl.com/2t4olk
 * 
 * DynamicSiteMapProvider is a class that adds new
 * items to a site map at runtime. The filename of 
 * the existing xml sitemap is set in the Web.Config
 * file. This file is loaded, and extra nodes can
 * be added to the xml (by you) before the new xml is
 * converted into a site map node structure, and passed
 * back to the calling object.
 * 
 * Feel free to use and modify this class for personal, 
 * non-profit or commercial use, without charge. I would
 * appreciate you leaving this comment here. I'd also 
 * appreciate a link to my blog from your website, but 
 * I'll leave that up to you.
 * 
 */


using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using ModConfig;
using Snitz.Entities;
using SnitzCommon;


namespace SnitzBase
{
    /// <summary>
    /// Adds new items to an existing xml site map at runtime.
    /// Modify this class to add your own items.
    /// </summary>
    public class DynamicSiteMapProvider : StaticSiteMapProvider
    {
        public DynamicSiteMapProvider()
            : base()
        {

        }

        private String _siteMapFileName;
        private SiteMapNode _rootNode = null;
        private readonly object _siteMapLock = new object(); 

        // Return the root node of the current site map.
        public override SiteMapNode RootNode
        {
            get
            {
                return BuildSiteMap();
            }
        }

        /// <summary>
        /// Pull out the filename of the site map xml.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
        {
            base.Initialize(name, attributes);
            _siteMapFileName = attributes["siteMapFile"];
        }

        private const String SiteMapNodeName = "siteMapNode";

        public override SiteMapNode BuildSiteMap()
        {
            lock (_siteMapLock)
            {
                if (_rootNode != null)
                {
                    this.EnableLocalization = true;
                    return _rootNode;

                }
                base.Clear();
                this.EnableLocalization = true;
                
                // Load the sitemap's xml from the file.
                XmlDocument siteMapXml = LoadSiteMapXml();

                // Create the first site map item from the top node in the xml.
                XmlElement rootElement = (XmlElement)siteMapXml.GetElementsByTagName(SiteMapNodeName)[0];

                // This is the key method - add the dynamic nodes to the xml
                AddDynamicNodes(rootElement);

                // Now build up the site map structure from the xml
                GenerateSiteMapNodes(rootElement);
                return _rootNode;
            }
            
        }
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            //If Security Trimming is not enabled return true
            if (!SecurityTrimmingEnabled)
                return true; 

            //If there are no roles defined for the page
            //return true or false depending on your authorization scheme (when true pages with
            //no roles are visible to all users, when false no user can access these pages)
            if (node.Roles == null || node.Roles.Count == 0)
                return true; 

            //check each role, if the user is in any of the roles return true
            foreach (string role in node.Roles)
            {
                if (context.User.IsInRole(role) || String.Equals(role, "*", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            } 

            return false;
        }


        /// <summary>
        /// Open the site map file as an xml document.
        /// </summary>
        /// <returns>The contents of the site map file.</returns>
        private XmlDocument LoadSiteMapXml()
        {
            XmlDocument siteMapXml = new XmlDocument();
            siteMapXml.Load(HttpContext.Current.Server.MapPath(_siteMapFileName));
            return siteMapXml;
        }

        /// <summary>
        /// Creates the site map nodes from the root of 
        /// the xml document.
        /// </summary>
        /// <param name="rootElement">The top-level sitemap element from the XmlDocument loaded with the site map xml.</param>
        private void GenerateSiteMapNodes(XmlElement rootElement)
        {
            _rootNode = GetSiteMapNodeFromElement(rootElement);
            AddNode(_rootNode);
            CreateChildNodes(rootElement, _rootNode);
        }

        /// <summary>
        /// Recursive method! This finds all the site map elements
        /// under the current element, and creates a SiteMapNode for 
        /// them.  On each of these, it calls this method again to 
        /// create it's new children, and so on.
        /// </summary>
        /// <param name="parentElement">The xml element to iterate through.</param>
        /// <param name="parentNode">The site map node to add the new children to.</param>
        private void CreateChildNodes(XmlElement parentElement, SiteMapNode parentNode)
        {
            foreach (XmlNode xmlElement in parentElement.ChildNodes)
            {
                if (xmlElement.Name == SiteMapNodeName)
                {
                    SiteMapNode childNode = GetSiteMapNodeFromElement((XmlElement)xmlElement);
                    
                    AddNode(childNode, parentNode);
                    CreateChildNodes((XmlElement)xmlElement, childNode);
                }
            }
        }


        /// <summary>
        /// The key method. You can add your own code in here
        /// to add xml nodes to the structure, from a 
        /// database, file on disk, or just from code.
        /// To keep the example short, I'm just adding from code.
        /// </summary>
        /// <param name="rootElement"></param>
        private void AddDynamicNodes(XmlElement rootElement)
        {
            //Add any mod menu items
            var menuitems = ConfigHelper.GetMenuItems();
            foreach (ModMenuItem element in menuitems)
            {
                AddDynamicChildElement(rootElement, element);
            }

        }

        private void AddDynamicChildElement(XmlElement parentElement, ModMenuItem element)
        {
            XmlNode copy = parentElement.OwnerDocument.ImportNode(element.MenuXml.GetXmlElement(), true);
            if (!String.IsNullOrEmpty(element.Parent))
            {
                XmlNode parentNode =
                    parentElement.OwnerDocument.SelectSingleNode("descendant::siteMapNode[@key='" +
                                                                 element.Parent + "']");
                if (!String.IsNullOrEmpty(element.InsertAfter))
                {
                    XmlNode afterNode =
                    parentElement.SelectSingleNode("descendant::siteMapNode[@key='" +
                                                                 element.InsertAfter + "']");
                    parentNode.InsertAfter(copy, afterNode);
                }else
                    parentNode.AppendChild(copy);
            }
            else if (!String.IsNullOrEmpty(element.InsertAfter))
            {
                XmlNode afterNode =
                    parentElement.OwnerDocument.SelectSingleNode("descendant::siteMapNode[@key='" +
                                                                 element.InsertAfter + "']");
                parentElement.InsertAfter(copy, afterNode);
            }
            else
            {
                parentElement.AppendChild(copy);
            }
        }

        private static XmlElement AddDynamicChildElement(XmlElement parentElement, String url, String title, String description, String roles)
        {
            // Create new element from the parameters
            XmlElement childElement = parentElement.OwnerDocument.CreateElement(SiteMapNodeName);
            if(url != null)
                childElement.SetAttribute("url", url);
            childElement.SetAttribute("title",  title);
            childElement.SetAttribute("description", description);
            if (roles != null)
            {
                childElement.SetAttribute("roles", roles);
            }

            // Add it to the parent
            parentElement.AppendChild(childElement);
            return childElement;
        }

        private SiteMapNode GetSiteMapNodeFromElement(XmlElement rootElement)
        {
            SiteMapNode newSiteMapNode;
            String key = rootElement.GetAttribute("key");
            String url = rootElement.GetAttribute("url");
            String title = rootElement.GetAttribute("title");
            String description = rootElement.GetAttribute("description");
            var explicitResourceKeys = new System.Collections.Specialized.NameValueCollection(); 

            if (title.StartsWith("$resources:"))
            {
                string[] localization = title.Replace("$resources:", "").Split(',');

                explicitResourceKeys.Add("title", localization[0]);
                explicitResourceKeys.Add("title", localization[1]);
                explicitResourceKeys.Add("title", localization[2]);
                title = localization[2];
            }
            if (description.StartsWith("$resources:"))
            {
                string[] localization = description.Replace("$resources:", "").Split(',');

                explicitResourceKeys.Add("Description", localization[0]);
                explicitResourceKeys.Add("Description", localization[1]);
                explicitResourceKeys.Add("Description", localization[2]);
                description = localization[2];
            }
            if (String.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString();
            }
            newSiteMapNode = new SiteMapNode(this,key, url, title, description, null, null, explicitResourceKeys, null);
            foreach (XmlAttribute attribute in rootElement.Attributes)
            {
                switch (attribute.Name)
                {
                    case "resourceKey":
                        newSiteMapNode.ResourceKey = null; 
                        break; 
                    case "roles":
                        newSiteMapNode.Roles = attribute.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); 
                            break;
                    case "description":
                    case "title":
                    case "siteMapFile":             
                    case "securityTrimmingEnabled":             
                    case "provider":             
                    case "url":             
                    default:                 
                    break;
                }
            } 
            return newSiteMapNode;
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return RootNode;
        }

        // Empty out the existing items.
        protected override void Clear()
        {
            lock (this)
            {
                _rootNode = null;
                base.Clear();
            }
        }
    }
}


