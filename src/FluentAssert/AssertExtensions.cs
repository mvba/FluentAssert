//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using FluentAssert.Exceptions;

namespace FluentAssert
{
	[DebuggerNonUserCode]
	[DebuggerStepThrough]
	public static class AssertExtensions
	{
		private static KeyValuePair<bool, object> FindMissingItem(this IEnumerable list, IEnumerable expected)
		{
			var listList = new ArrayList();
			foreach (var item in list)
			{
				listList.Add(item);
			}
			foreach (var item in expected)
			{
				var index = listList.IndexOf(item);
				if (index == -1)
				{
					return new KeyValuePair<bool, object>(true, item);
				}
				listList.RemoveAt(index);
			}

			return new KeyValuePair<bool, object>(false, null);
		}

		private static bool IsNull<T>(T expected)
		{
			object objectExpected = expected;
			return objectExpected == null;
		}

		public static IList<T> ShouldBeEmpty<T>(this IList<T> list)
		{
			list.ShouldNotBeNull();
			list.Count.ShouldBeEqualTo(0);
			return list;
		}

		public static T ShouldBeEqualTo<T>(this T item, T expected)
		{
			return ShouldBeEqualTo(item, expected, () => ShouldBeEqualAssertionException.CreateMessage(item, expected));
		}

		public static T ShouldBeEqualTo<T>(this T item, T expected, string errorMessage)
		{
			return ShouldBeEqualTo(item, expected, () => errorMessage);
		}

		public static T ShouldBeEqualTo<T>(this T item, T expected, Func<string> getErrorMessage)
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (ReferenceEquals(item, expected))
			{
				return item;
			}

			var itemIsNull = IsNull(item);
			var expectedIsNull = IsNull(expected);

			if (itemIsNull && expectedIsNull)
			{
				return item;
			}
			if (itemIsNull || expectedIsNull)
			{
				throw new ShouldBeEqualAssertionException(getErrorMessage());
			}

			if (typeof(T) != typeof(string) &&
				typeof(IEnumerable).IsAssignableFrom(typeof(T)))
			{
				var itemContainer = new ArrayList();
				foreach (var value in (IEnumerable)item)
				{
					itemContainer.Add(value);
				}
				var expectedContainer = new ArrayList();
				foreach (var value in (IEnumerable)expected)
				{
					expectedContainer.Add(value);
				}

				itemContainer.Count.ShouldBeEqualTo(expectedContainer.Count, () => "  Expected " + expectedContainer.Count + " items but contained " + itemContainer.Count);

				var expectedItemMissingFromList = FindMissingItem(itemContainer, expectedContainer);
				if (expectedItemMissingFromList.Key)
				{
					throw new ShouldBeEqualAssertionException("  Expected list to contain: " + expectedItemMissingFromList.Value);
				}
			}
			else if (!item.Equals(expected))
			{
				throw new ShouldBeEqualAssertionException(getErrorMessage());
			}

			return item;
		}

		public static void ShouldBeFalse(this bool item)
		{
			ShouldBeFalse(item, ShouldBeFalseAssertionException.CreateMessage);
		}

		public static void ShouldBeFalse(this bool item, string errorMessage)
		{
			ShouldBeFalse(item, () => errorMessage);
		}

		public static void ShouldBeFalse(this bool item, Func<string> getErrorMessage)
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (item)
			{
				throw new ShouldBeFalseAssertionException(getErrorMessage());
			}
		}

		public static T ShouldBeGreaterThan<T>(this T item, T other) where T : IComparable
		{
			return ShouldBeGreaterThan(item,
				other,
				() => ShouldBeGreaterThanAssertionException.CreateMessage(ExpectedMessageBuilder.ToDisplayableString(other),
					ExpectedMessageBuilder.ToDisplayableString(item)));
		}

		public static T ShouldBeGreaterThan<T>(this T item, T other, string errorMessage) where T : IComparable
		{
			return ShouldBeGreaterThan(item, other, () => errorMessage);
		}

		public static T ShouldBeGreaterThan<T>(this T item, T other, Func<string> getErrorMessage) where T : IComparable
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			object obj = item;
			obj.ShouldNotBeNull();

			if (item.CompareTo(other) != 1)
			{
				throw new ShouldBeGreaterThanAssertionException(getErrorMessage());
			}
			return item;
		}

		public static T ShouldBeGreaterThanOrEqualTo<T>(this T item, T other) where T : IComparable
		{
			return ShouldBeGreaterThanOrEqualTo(item,
				other,
				() => ShouldBeGreaterThanOrEqualToAssertionException.CreateMessage(ExpectedMessageBuilder.ToDisplayableString(other),
					ExpectedMessageBuilder.ToDisplayableString(item)));
		}

		public static T ShouldBeGreaterThanOrEqualTo<T>(this T item, T other, string errorMessage) where T : IComparable
		{
			return ShouldBeGreaterThanOrEqualTo(item, other, () => errorMessage);
		}

		public static T ShouldBeGreaterThanOrEqualTo<T>(this T item, T other, Func<string> getErrorMessage) where T : IComparable
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			object obj = item;
			obj.ShouldNotBeNull();

			if (item.CompareTo(other) == -1)
			{
				throw new ShouldBeGreaterThanOrEqualToAssertionException(getErrorMessage());
			}
			return item;
		}

		public static T ShouldBeInRangeInclusive<T>(this T item, T lowEndOfRange, T highEndOfRange)
			where T : IComparable
		{
			item.ShouldBeGreaterThanOrEqualTo(lowEndOfRange);
			item.ShouldBeLessThanOrEqualTo(highEndOfRange);
			return item;
		}

		public static T ShouldBeLessThan<T>(this T item, T other) where T : IComparable
		{
			return ShouldBeLessThan(item,
				other,
				() => ShouldBeLessThanAssertionException.CreateMessage(ExpectedMessageBuilder.ToDisplayableString(other),
					ExpectedMessageBuilder.ToDisplayableString(item)));
		}

		public static T ShouldBeLessThan<T>(this T item, T other, string errorMessage) where T : IComparable
		{
			return ShouldBeLessThan(item, other, () => errorMessage);
		}

		public static T ShouldBeLessThan<T>(this T item, T other, Func<string> getErrorMessage) where T : IComparable
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			object obj = item;
			obj.ShouldNotBeNull();

			if (item.CompareTo(other) != -1)
			{
				throw new ShouldBeLessThanAssertionException(getErrorMessage());
			}
			return item;
		}

		public static T ShouldBeLessThanOrEqualTo<T>(this T item, T other) where T : IComparable
		{
			return ShouldBeLessThanOrEqualTo(item,
				other,
				() => ShouldBeLessThanOrEqualToAssertionException.CreateMessage(ExpectedMessageBuilder.ToDisplayableString(other),
					ExpectedMessageBuilder.ToDisplayableString(item)));
		}

		public static T ShouldBeLessThanOrEqualTo<T>(this T item, T other, string errorMessage) where T : IComparable
		{
			return ShouldBeLessThanOrEqualTo(item, other, () => errorMessage);
		}

		public static T ShouldBeLessThanOrEqualTo<T>(this T item, T other, Func<string> getErrorMessage) where T : IComparable
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			object obj = item;
			obj.ShouldNotBeNull();

			if (item.CompareTo(other) == 1)
			{
				throw new ShouldBeLessThanOrEqualToAssertionException(getErrorMessage());
			}
			return item;
		}

		public static T ShouldBeNull<T>(this T item) where T : class
		{
			return ShouldBeNull(item, () => ShouldBeNullAssertionException.CreateMessage(ExpectedMessageBuilder.ToDisplayableString(item)));
		}

		public static T? ShouldBeNull<T>(this T? item) where T : struct
		{
			return ShouldBeNull(item, () => ShouldBeNullAssertionException.CreateMessage(ExpectedMessageBuilder.ToDisplayableString(item)));
		}

		public static T ShouldBeNull<T>(this T item, string errorMessage) where T : class
		{
			return ShouldBeNull(item, () => errorMessage);
		}

		public static T ShouldBeNull<T>(this T item, Func<string> getErrorMessage) where T : class
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (item != null)
			{
				throw new ShouldBeNullAssertionException(getErrorMessage());
			}
			return item;
		}

		public static T? ShouldBeNull<T>(this T? item, string errorMessage) where T : struct
		{
			return ShouldBeNull(item, () => errorMessage);
		}

		public static T? ShouldBeNull<T>(this T? item, Func<string> getErrorMessage) where T : struct
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (item.HasValue)
			{
				throw new ShouldBeNullAssertionException(getErrorMessage());
			}
			return item;
		}

		public static object ShouldBeOfType<TType>(this object item)
		{
			item.ShouldBeOfType<TType>("");
			return item;
		}

		public static object ShouldBeOfType<TType>(this object item, string errorMessage)
		{
			(item is TType).ShouldBeTrue(errorMessage);
			return item;
		}

		public static T ShouldBeSameInstanceAs<T>(this T item, T other)
		{
			ReferenceEquals(item, other).ShouldBeTrue();
			return item;
		}

		public static void ShouldBeTrue(this bool item)
		{
			ShouldBeTrue(item, ShouldBeTrueAssertionException.CreateMessage);
		}

		public static void ShouldBeTrue(this bool item, string errorMessage)
		{
			ShouldBeTrue(item, () => errorMessage);
		}

		public static void ShouldBeTrue(this bool item, Func<string> getErrorMessage)
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (!item)
			{
				throw new ShouldBeTrueAssertionException(getErrorMessage());
			}
		}

		public static string ShouldContain(this string item, string expectedSubstring)
		{
			item.Contains(expectedSubstring).ShouldBeTrue();
			return item;
		}

		public static IList<T> ShouldContainAll<T>(this IList<T> list, IEnumerable<T> expected)
		{
			var result = FindMissingItem(list, expected);
			if (!result.Key)
			{
				return list;
			}
			throw new ShouldBeTrueAssertionException("Collection does not contain '" + result.Value + "'");
		}

		public static IEnumerable<T> ShouldContainAllInOrder<T>(this IEnumerable<T> list, IEnumerable<T> expected) where T : IEquatable<T>
		{
// ReSharper disable once PossibleMultipleEnumeration
			var indexedSource = list.Select((x, i) => new
			                                          {
				                                          Item = x,
				                                          Index = i
			                                          }).ToList();
			var indexedExpected = expected.Select((x, i) => new
			                                                {
				                                                Item = x,
				                                                Index = i
			                                                }).ToList();
			indexedSource.Count.ShouldBeEqualTo(indexedExpected.Count);
			foreach (var index in Enumerable.Range(0, indexedSource.Count))
			{
				indexedSource[index].Item.ShouldBeEqualTo(indexedExpected[index].Item, "at offset " + index);
			}
// ReSharper disable once PossibleMultipleEnumeration
			return list;
		}

		public static string ShouldEndWith(this string item, string expected)
		{
			item.EndsWith(expected).ShouldBeTrue();
			return item;
		}

		public static string ShouldEndWith(this string item, string expected, string errorMessage)
		{
			item.EndsWith(expected).ShouldBeTrue(errorMessage);
			return item;
		}

		public static string ShouldNotBeEmpty(this string item)
		{
			item.Length.ShouldBeGreaterThan(0);
			return item;
		}

		public static string ShouldNotBeEmpty(this string item, string errorMessage)
		{
			item.Length.ShouldBeGreaterThan(0, errorMessage);
			return item;
		}

		public static string ShouldNotBeEmpty(this string item, Func<string> getErrorMessage)
		{
			item.Length.ShouldBeGreaterThan(0, getErrorMessage);
			return item;
		}

		public static T ShouldNotBeEqualTo<T>(this T item, T expected)
		{
			return ShouldNotBeEqualTo(item, expected, () => ShouldNotBeEqualAssertionException.CreateMessage(item, expected));
		}

		public static T ShouldNotBeEqualTo<T>(this T item, T expected, string errorMessage)
		{
			return ShouldNotBeEqualTo(item, expected, () => errorMessage);
		}

		public static T ShouldNotBeEqualTo<T>(this T item, T expected, Func<string> getErrorMessage)
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (ReferenceEquals(item, expected))
			{
				throw new ShouldNotBeEqualAssertionException(getErrorMessage());
			}

			var itemIsNull = IsNull(item);
			var expectedIsNull = IsNull(expected);

			if (itemIsNull && expectedIsNull ||
				!itemIsNull && item.Equals(expected))
			{
				throw new ShouldNotBeEqualAssertionException(getErrorMessage());
			}

			return item;
		}

		public static T ShouldNotBeNull<T>(this T item) where T : class
		{
			return ShouldNotBeNull(item, ShouldNotBeNullAssertionException.CreateMessage);
		}

		public static T? ShouldNotBeNull<T>(this T? item) where T : struct
		{
			return ShouldNotBeNull(item, ShouldNotBeNullAssertionException.CreateMessage);
		}

		public static T ShouldNotBeNull<T>(this T item, string errorMessage) where T : class
		{
			return ShouldNotBeNull(item, () => errorMessage);
		}

		public static T ShouldNotBeNull<T>(this T item, Func<string> getErrorMessage) where T : class
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (item == null)
			{
				throw new ShouldNotBeNullAssertionException(getErrorMessage());
			}
			return item;
		}

		public static T? ShouldNotBeNull<T>(this T? item, string errorMessage) where T : struct
		{
			return ShouldNotBeNull(item, () => errorMessage);
		}

		public static T? ShouldNotBeNull<T>(this T? item, Func<string> getErrorMessage) where T : struct
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			if (!item.HasValue)
			{
				throw new ShouldNotBeNullAssertionException(getErrorMessage());
			}
			return item;
		}

		public static string ShouldNotBeNullOrEmpty(this string item)
		{
			item.ShouldNotBeNull();
			item.ShouldNotBeEmpty();
			return item;
		}

		public static string ShouldNotBeNullOrEmpty(this string item, string errorMessage)
		{
			item.ShouldNotBeNull(errorMessage);
			item.ShouldNotBeEmpty(errorMessage);
			return item;
		}

		public static T ShouldNotBeSameInstanceAs<T>(this T item, T other)
		{
			ReferenceEquals(item, other).ShouldBeFalse();
			return item;
		}

		public static string ShouldNotContain(this string item, string expectedSubstring)
		{
			item.Contains(expectedSubstring).ShouldBeFalse();
			return item;
		}

		public static string ShouldNotEndWith(this string item, string expected)
		{
			item.EndsWith(expected).ShouldBeFalse();
			return item;
		}

		public static string ShouldNotEndWith(this string item, string expected, string errorMessage)
		{
			item.EndsWith(expected).ShouldBeFalse(errorMessage);
			return item;
		}

		public static string ShouldNotStartWith(this string item, string expected)
		{
			item.StartsWith(expected).ShouldBeFalse();
			return item;
		}

		public static string ShouldNotStartWith(this string item, string expected, string errorMessage)
		{
			item.StartsWith(expected).ShouldBeFalse(errorMessage);
			return item;
		}

		public static string ShouldStartWith(this string item, string expected)
		{
			item.StartsWith(expected).ShouldBeTrue();
			return item;
		}

		public static string ShouldStartWith(this string item, string expected, string errorMessage)
		{
			item.StartsWith(expected).ShouldBeTrue(errorMessage);
			return item;
		}

		public static void ShouldThrow<TException>(Action action) where TException : Exception
		{
			try
			{
				action();
			}
			catch (TException)
			{
				return;
			}
			catch (ShouldThrowExceptionAssertionException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ShouldThrowExceptionAssertionException(typeof(TException), exception);
			}
			throw new ShouldThrowExceptionAssertionException(typeof(TException));
		}

		public static void ShouldThrow<TException>(Action action, string errorMessage) where TException : Exception
		{
			ShouldThrow<TException>(action, () => errorMessage);
		}

		public static void ShouldThrow<TException>(Action action, Func<string> getErrorMessage) where TException : Exception
		{
			if (getErrorMessage == null)
			{
				throw new ArgumentNullException("getErrorMessage", "the method used to get the error message cannot be null");
			}

			try
			{
				action();
			}
			catch (TException)
			{
				return;
			}
			catch (ShouldThrowExceptionAssertionException)
			{
				throw;
			}
			catch (Exception)
			{
				throw new ShouldThrowExceptionAssertionException(getErrorMessage());
			}
			throw new ShouldThrowExceptionAssertionException(getErrorMessage());
		}

		public static Exception ShouldThrowAnException<T>(this T item, Func<T, object> methodToCall) where T : Exception
		{
			try
			{
				methodToCall(item);
			}
			catch (ShouldThrowExceptionAssertionException)
			{
				throw;
			}
			catch (Exception exception)
			{
				return exception;
			}
			throw new ShouldThrowExceptionAssertionException(typeof(T));
		}
	}
}