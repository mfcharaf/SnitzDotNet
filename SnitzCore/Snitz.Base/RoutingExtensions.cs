/*
####################################################################################################################
##
## SnitzBase - RoutingExtensions
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


using System;
using System.Collections.Generic;

namespace SnitzCommon
{
	public static class RoutingExtensions
	{
		/// <summary>
		/// Generates a sequence of elements while <paramref name="predicate"/> is <see cref="true"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The initial value.</param>
		/// <param name="predicate">A predicate to control continuation of the sequence.</param>
		/// <param name="selector">A function to retrieve the next element.</param>
		/// <returns>Returns a sequence of elements.</returns>
		public static IEnumerable<T> For<T>( this T value, Predicate<T> predicate, Func<T, T> selector )
		{
			while ( predicate != null && predicate( value ) )
			{
				yield return value;

				value = ( selector != null ) ? selector( value ) : default( T );
			}
		}
	}
}