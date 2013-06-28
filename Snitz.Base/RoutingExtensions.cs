using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

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