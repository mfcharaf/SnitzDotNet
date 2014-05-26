/*
####################################################################################################################
##
## Snitz.IDAL - Factory
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

using System.Configuration;
using System.Reflection;

namespace Snitz.IDAL
{
    /// <summary>
    /// Class to load the relevent Database library  
    /// </summary>
    /// <typeparam name="T">Object class to instantiate</typeparam>
    public class Factory<T>
    {
        /// <summary>
        /// Loads a dataclass using the Snitz core data access interfaces
        /// </summary>
        /// <param name="name">Name of the Class to load</param>
        /// <returns>Instance of the data access class</returns>
        public static T Create(string name)
        {
            // Look up the DAL implementation we should be using
            string dal = "Snitz." + ConfigurationManager.AppSettings["SnitzDAL"];
            string className = dal + "." + name;
            return (T)Assembly.Load(dal).CreateInstance(className);
        }

        /// <summary>
        /// Loads a dataclass using custom data access interfaces
        /// </summary>
        /// <param name="name">Name of the Class to load</param>
        /// <param name="config">Name of the appsettings key which points to the data library</param>
        /// <returns>Instance of the data access class</returns>
        public static T Create(string name, string config)
        {
            // Look up the DAL implementation we should be using
            string dal = ConfigurationManager.AppSettings[config];
            string className = dal + "." + name;
            return (T)Assembly.Load(dal).CreateInstance(className);
        }
    }
}
