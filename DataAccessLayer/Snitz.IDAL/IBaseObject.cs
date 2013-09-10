/*
####################################################################################################################
##
## Snitz.IDAL - IBaseObject
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

using System.Collections.Generic;


namespace Snitz.IDAL
{
    public interface IBaseObject<T>
    {
        /// <summary>
        /// Get a record by it's id
        /// </summary>
        /// <param name="id">id of the record to fetch</param>
        /// <returns>Object Entity</returns>
        T GetById(int id);

        /// <summary>
        /// Find an object by name (or main text field like subject etc)
        /// </summary>
        /// <param name="name">string to look for</param>
        /// <returns>an eneumerable list of matching records</returns>
        IEnumerable<T> GetByName(string name);
        
        /// <summary>
        /// Adds an object to the database
        /// </summary>
        /// <param name="obj">Object to add</param>
        /// <returns>Id of new object</returns>
        int Add(T obj);

        /// <summary>
        /// Update an object in the database
        /// </summary>
        /// <param name="obj">Object to update</param>
        void Update(T obj);

        /// <summary>
        /// Delete an object from the database
        /// </summary>
        /// <param name="obj">Object to delete</param>
        void Delete(T obj);

        /// <summary>
        /// A function to dispose of resources used by the dal
        /// </summary>
        void Dispose();
    }
}
