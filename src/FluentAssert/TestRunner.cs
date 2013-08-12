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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FluentAssert.Exceptions;

namespace FluentAssert
{
	[DebuggerNonUserCode]
	[DebuggerStepThrough]
	internal class TestStepExecutor
	{
		public static void Verify(StringBuilder scenarioDescription, ITestStep testStep, ExceptionConfiguration exceptionConfiguration)
		{
			scenarioDescription.Append(testStep.Description);
			try
			{
				testStep.Action();
				scenarioDescription.AppendLine(testStep.SuccessSuffix);
			}
			catch (Exception e)
			{
				if (!exceptionConfiguration.ExpectException)
				{
					scenarioDescription.AppendLine(testStep.FailureSuffix);
					Console.Error.WriteLine(scenarioDescription.ToString());
					throw;
				}

				var exceptionType = e.GetType();
				if (exceptionType != exceptionConfiguration.ExpectedExceptionType)
				{
					scenarioDescription.AppendLine(testStep.FailureSuffix);
					Console.Error.WriteLine(scenarioDescription.ToString());
					throw new AssertionException(String.Format("Expected exception of type {0} but caught type {1}",
						exceptionConfiguration.ExpectedExceptionType,
						exceptionType),
						e);
				}

				if (exceptionConfiguration.ExpectExceptionMessage)
				{
					if (e.Message != exceptionConfiguration.ExpectedExceptionMessage)
					{
						scenarioDescription.AppendLine(testStep.FailureSuffix);
						Console.Error.WriteLine(scenarioDescription.ToString());
						throw new AssertionException(String.Format("Expected exception message '{0}' but had '{1}'",
							exceptionConfiguration.ExpectedExceptionMessage,
							e.Message),
							e);
					}
				}

				exceptionConfiguration.CaughtExpectedException();
				scenarioDescription.AppendLine(testStep.SuccessSuffix);
			}
		}
	}

	[DebuggerNonUserCode]
	[DebuggerStepThrough]
	internal static class TestRunner
	{
		public static void Verify(string actionDescription,
			IEnumerable<IParameterActionWrapper> parameterActions,
			Action action,
			IEnumerable<IDependencyActionWrapper> dependencyActions,
			IEnumerable<IAssertionActionWrapper> assertions)
		{
			var steps = parameterActions
				.Select(arrange => new WithTestStep(arrange.Setup, arrange.Description))
				.Cast<ITestStep>()
				.ToList();

			steps.AddRange(
				dependencyActions
					.Select(assertion => new ExpectTestStep(assertion.Verify, assertion.Description))
					.Cast<ITestStep>()
				);

			steps.Add(new WhenTestStep(action, actionDescription));

			steps.AddRange(
				assertions.Select(assertion => new ShouldTestStep(assertion.Verify, assertion.Description))
					.Cast<ITestStep>()
				);

			new TestVerifyClause().Verify(steps);
		}
	}
}