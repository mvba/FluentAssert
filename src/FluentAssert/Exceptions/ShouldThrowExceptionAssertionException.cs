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
using System.Runtime.Serialization;

namespace FluentAssert.Exceptions
{
	[Serializable]
	public class ShouldThrowExceptionAssertionException : AssertionException
	{
		protected ShouldThrowExceptionAssertionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ShouldThrowExceptionAssertionException(string errorMessage)
			: base(errorMessage)
		{
		}

		public ShouldThrowExceptionAssertionException(Type type)
			: base(CreateMessage(type, null))
		{
		}

		public ShouldThrowExceptionAssertionException(Type expectedType, Exception actualException)
			: base(CreateMessage(expectedType, actualException))
		{
			throw new NotImplementedException();
		}

		public static string CreateMessage(Type type, Exception actualException)
		{
			var message = "  Should have thrown " + type.Name + Environment.NewLine;
			if (actualException != null)
			{
				message += "  But threw " + actualException.GetType().Name + ": " + actualException.Message + Environment.NewLine;
			}
			return message;
		}
	}
}